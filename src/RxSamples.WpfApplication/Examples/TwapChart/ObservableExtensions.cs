using System;
using System.Reactive.Linq;

namespace RxSamples.WpfApplication.Examples.TwapChart
{
    public static class ObservableExtensions
    {
        public static TimeSpan Milliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        public static IObservable<decimal> OverlappingWindowAverage<T>
            (
            this IObservable<T> source,
            TimeSpan windowPeriod,
            TimeSpan accuracy,
            Func<T, Decimal> resultSelector
            )
        {
            //We will subscribe twice to this so Publish and refcount to ensure we share the stream.
            var sourcePublished = source.Publish().RefCount();

            var ticks =
                //Open a new window every period defined by the accuracy argument
                Observable.Interval(accuracy)
                    //Stop ticking if the source completes
                    .TakeUntil(sourcePublished.IgnoreElements().Materialize());

            return sourcePublished.Window
                (
                    ticks,
                    //Close window after windowPeriod
                    windowOpening => Observable.Timer(windowPeriod)
                )
                //Average the values and return the single result as an IObservable<Decimal>
                .Select(windowValues =>
                            {
                                return windowValues.Select(resultSelector)
                                    .Average()
                                    //Empty windows will throw InvalidOperationException from a call to Average
                                    .Catch((InvalidOperationException ex) =>
                                               {
                                                   //Ignoring empty stream for average
                                                   return Observable.Empty<Decimal>();
                                               });
                            }
                )
                //At this point in time I will have an IObservable<IObservable<decimal>>
                //     ie each of the averages will be in their own child IObservable

                //Merge all of the Average child IObservable in to a flattened IObservable
                .Merge();
        }

    }
}