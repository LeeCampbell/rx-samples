using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace RxSamples.WpfApplication.Examples.TwapChart
{
    public class PricesViewModel
    {
        private const int MaxPoints = 50;
        private readonly MaxLengthObservableCollection<Price> _open = new MaxLengthObservableCollection<Price>(MaxPoints);
        private readonly MaxLengthObservableCollection<Price> _high = new MaxLengthObservableCollection<Price>(MaxPoints);
        private readonly MaxLengthObservableCollection<Price> _low = new MaxLengthObservableCollection<Price>(MaxPoints);
        private readonly MaxLengthObservableCollection<Price> _close = new MaxLengthObservableCollection<Price>(MaxPoints);
        private readonly MaxLengthObservableCollection<Price> _twap = new MaxLengthObservableCollection<Price>(MaxPoints);

        public PricesViewModel(IPriceService priceService)
        {
            priceService.Open
                .ObserveOnDispatcher()
                .Subscribe(Open.Add);

            priceService.High
                .ObserveOnDispatcher()
                .Subscribe(High.Add);

            priceService.Low
                .ObserveOnDispatcher()
                .Subscribe(Low.Add);

            priceService.Close
                .ObserveOnDispatcher()
                .Subscribe(Close.Add);

            Observable.Merge(
                    priceService.Low,
                    priceService.High
                )
                .OverlappingWindowAverage(5.Seconds(), 100.Milliseconds(), price => price.Value)
                .ObserveOnDispatcher()
                .Subscribe(avg => Twap.Add(new Price(DateTime.Now, avg)));
        }

        public ObservableCollection<Price> Open { get { return _open; } }

        public ObservableCollection<Price> High { get { return _high; } }

        public MaxLengthObservableCollection<Price> Low { get { return _low; } }

        public MaxLengthObservableCollection<Price> Close { get { return _close; } }

        public ObservableCollection<Price> Twap { get { return _twap; } }
    }
}
