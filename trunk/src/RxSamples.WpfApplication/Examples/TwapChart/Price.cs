using System;

namespace RxSamples.WpfApplication.Examples.TwapChart
{
    public class Price
    {
        public Price(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date { get; private set; }
        public decimal Value { get; private set; }
    }
}