using System;
using System.Linq;

namespace RxSamples.ConsoleApp.TestingRx
{
    public class MyPresenter
    {
        private readonly IMyService _myService;
        private readonly IViewModel _viewModel;
        private readonly ISchedulerProvider _schedulerProvider;

        public MyPresenter(IMyService myService, IViewModel viewModel, ISchedulerProvider schedulerProvider)
        {
            _myService = myService;
            _schedulerProvider = schedulerProvider;
            _viewModel = viewModel;
        }

        public void Show(string symbol)
        {
            _myService.PriceStream(symbol)
                        .SubscribeOn(_schedulerProvider.ThreadPool)
                        .ObserveOn(_schedulerProvider.Dispatcher)
                        .Timeout(TimeSpan.FromSeconds(10), _schedulerProvider.ThreadPool)
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
        private readonly ISchedulerProvider _schedulerProvider;
        private IDisposable _priceSubscription;

        public MyPresenter_with_disposal(IMyService myService, IViewModel viewModel, ISchedulerProvider schedulerProvider)
        {
            _myService = myService;
            _schedulerProvider = schedulerProvider;
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
                 .SubscribeOn(_schedulerProvider.ThreadPool)
                 .ObserveOn(_schedulerProvider.Dispatcher)
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