using System;

namespace RxSamples.ConsoleApp.TestingRx
{
    public interface IMyService
    {
        IObservable<decimal> PriceStream(string symbol);
    }
}