using System;
using System.IO;
using System.Linq;

namespace RxSamples.WpfApplication.Examples.TwapChart
{
    public interface IPriceService
    {
        IObservable<Price> Open { get; }
        IObservable<Price> High { get; }
        IObservable<Price> Low { get; }
        IObservable<Price> Close { get; }
    }


    /// <summary>
    /// Gets static data from a file and returns it in an endless loop.
    /// </summary>
    public class PriceService : IPriceService
    {
        private readonly IObservable<Price> _open;
        private readonly IObservable<Price> _high;
        private readonly IObservable<Price> _low;
        private readonly IObservable<Price> _close;

        //<TICKER>,<DATE>,<TIME>,<OPEN>,<LOW>,<HIGH>,<CLOSE>
        private const int TickerIdx =0;
        private const int DateIdx =1;
        private const int TimeIdx =2;
        private const int OpenIdx=3;
        private const int LowIdx=4;
        private const int HighIdx=5;
        private const int CloseIdx=6;

        public PriceService()
        {
            //Linq over IEnumerable
            var tableData = File.ReadAllLines(@"Examples\TWAPChart\XAUAUD_day.csv")
                .Skip(1)                        //Skip the Header row.
                .Select(line=> line.Split(',')) //Return the row as a comma delimited array 
                .Repeat();                      //Make this data repeat forever

            //Linq over IObservable with some IEnumerable
            var ticks = Observable.Interval(100.Milliseconds())
                .Select(i => (int)i)            //Observable.Interval produces IObservable<long>, we only want int's.
                .Select(i => tableData.ElementAt(i))
                .Publish();

            _open = ticks.Select(row => CreatePrice(row[OpenIdx]));
            _high = ticks.Select(row => CreatePrice(row[HighIdx]));
            _low = ticks.Select(row => CreatePrice(row[LowIdx]));
            _close = ticks.Select(row => CreatePrice(row[CloseIdx]));

            ticks.Connect();
        }

        private static Price CreatePrice(string strValue)
        {
            var value = Decimal.Parse(strValue);
            return new Price(DateTime.Now, value);
        }

        #region Implementation of IPriceService

        public IObservable<Price> Open { get { return _open; } }
        public IObservable<Price> High { get { return _high; } }
        public IObservable<Price> Low { get { return _low; } }
        public IObservable<Price> Close { get { return _close; } }

        #endregion
    }
}