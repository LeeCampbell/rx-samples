using System;
using System.Concurrency;
using System.Linq;

namespace RxSamples.ConsoleApp.TestingRx
{

    public class MyPresenter
    {
        private readonly IMyService _myService;
        private readonly IViewModel _viewModel;
        private readonly ISchedulerService _schedulerService;

        public MyPresenter(IMyService myService, IViewModel viewModel, ISchedulerService schedulerService)
        {
            _myService = myService;
            _schedulerService = schedulerService;
            _viewModel = viewModel;
        }

        public void Show(string symbol)
        {
            _myService.PriceStream(symbol)
                        .SubscribeOn(_schedulerService.ThreadPool)
                        .ObserveOn(_schedulerService.Dispatcher)
                        .Timeout(TimeSpan.FromSeconds(10), _schedulerService.ThreadPool)
                        .Subscribe(OnPriceUpdate, ex =>
                                                        {
                                                            if (ex is TimeoutException)
                                                                _viewModel.IsConnected = false;
                                                        });
            _viewModel.IsConnected = true;
        }

        private void OnPriceUpdate(decimal price)
        {
            _viewModel.Prices.Add(price);
        }
    }


    //How the presenter may look if we did the disposal of the subscription better.
    public class MyPresenter_with_disposal : IDisposable
    {
        private readonly IMyService _myService;
        private readonly IViewModel _viewModel;
        private readonly ISchedulerService _schedulerService;
        private IDisposable _priceSubscription;

        public MyPresenter_with_disposal(IMyService myService, IViewModel viewModel, ISchedulerService schedulerService)
        {
            _myService = myService;
            _schedulerService = schedulerService;
            _viewModel = viewModel;
        }

        public void Show(string symbol)
        {
            if (_priceSubscription != null)
            {
                _priceSubscription.Dispose();
                _viewModel.Prices.Clear();
            }

            _priceSubscription = _myService.PriceStream(symbol)
                 .SubscribeOn(_schedulerService.ThreadPool)
                 .ObserveOn(_schedulerService.Dispatcher)
                 .Subscribe(OnPriceUpdate);
        }

        private void OnPriceUpdate(decimal price)
        {
            _viewModel.Prices.Add(price);
        }

        #region Implementation of IDisposable
        public void Dispose()
        {
            if (_priceSubscription != null)
            {
                _priceSubscription.Dispose();
            }
        }

        #endregion
    }
}