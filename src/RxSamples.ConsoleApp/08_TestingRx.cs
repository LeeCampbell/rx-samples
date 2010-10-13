using System;
using System.Collections.ObjectModel;
using System.Concurrency;
using System.Linq;

namespace RxSamples.ConsoleApp
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
                 .Subscribe(OnPriceUpdate);
        }

        private void OnPriceUpdate(decimal price)
        {
            _viewModel.Prices.Add(price);
        }
    }

    public interface IViewModel
    {
        ObservableCollection<decimal> Prices { get; }
    }

    public interface IMyService
    {
        IObservable<decimal> PriceStream(string symbol);
    }

    public interface ISchedulerService  //Implementation would just be a wrapper around Scheduler and the static properties it exposes.
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
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
