using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using System.Linq;
using System.Text;
using System.Threading;

namespace RxSamples.ConsoleApp
{
    class JoinGroupWindowExamples : ExamplesBase
    {
        public void Buffer_With_Count_returns_IObservable_of_IList_of_T()
        {
            var bufferIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .BufferWithCount(3)
                .Subscribe(buffer =>
                {
                    var thisBufferIdx = bufferIdx++;
                    Console.WriteLine("--Buffer published");
                    WriteListToConsole(buffer, "Buffer" + thisBufferIdx);

                },
                () => Console.WriteLine("Completed"));
        }
        public void Buffer_With_Time_returns_IObservable_of_IList_of_T()
        {
            var bufferIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .BufferWithTime(TimeSpan.FromMilliseconds(500))
                .Subscribe(window =>
                {
                    var thisBufferIdx = bufferIdx++;
                    Console.WriteLine("--Buffer published");
                    WriteListToConsole(window, "Buffer" + thisBufferIdx);

                },
                () => Console.WriteLine("Completed"));
        }

        public void Window_With_Count_returns_IObservable_of_IObservable_of_T()
        {
            var windowIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .WindowWithCount(3)
                .Subscribe(window =>
                               {
                                   var thisWindowIdx = windowIdx++;
                                   Console.WriteLine("--Starting new window");
                                   WriteStreamToConsole(window, "Window" + thisWindowIdx);

                               },
                           () => Console.WriteLine("Completed"));
        }
        public void Window_With_Time_returns_IObservable_of_IObservable_of_T()
        {
            var windowIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .WindowWithTime(TimeSpan.FromMilliseconds(500))
                .Subscribe(window =>
                               {
                                   var thisWindowIdx = windowIdx++;
                                   Console.WriteLine("--Starting new window");
                                   WriteStreamToConsole(window, "Window" + thisWindowIdx);

                               },
                           () => Console.WriteLine("Completed"));
        }

        public void Switch_rolls_Windows_back_into_their_source()
        {
            //is the same as Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(10)
            var switchedWindow = Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .WindowWithTime(TimeSpan.FromMilliseconds(500))
                .Switch();

            WriteStreamToConsole(switchedWindow, "Switched window");
        }

        public void Join_with_no_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .Join(
                    right,
                    _ => Observable.Never<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "Join");

        }
        public void Join_with_immediate_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .Join(
                    right,
                    _ => Observable.Empty<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "Join");
        }

        public void Join_with_non_overlapping_left_windows()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6).Publish().RefCount();
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .Join(
                    right,
                    _ => left,
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "Join");
        }

        private static IObservable<TResult> MyCombineLatest<TLeft, TRight, TResult>(IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            var refcountedLeft = left.Publish().RefCount();
            var refcountedRight = right.Publish().RefCount();
            return Observable.Join(
                    refcountedLeft,
                    refcountedRight,
                    _ => refcountedLeft,
                    _ => refcountedRight,
                    resultSelector);
        }
        public void Join_as_CombineLatest()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(7);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65));
            var joinedStream = MyCombineLatest(left, right, (leftValue, rightValue) => new { Left = leftValue, Right = rightValue });
            WriteStreamToConsole(joinedStream, "Join as CombineLatest");
        }
        public void Actual_CombineLatest()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(7);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65));
            var joinedStream = Observable.CombineLatest(left, right, (leftValue, rightValue) => new { Left = leftValue, Right = rightValue });
            WriteStreamToConsole(joinedStream, "Join as CombineLatest");
        }

        public void GroupJoin_with_no_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .GroupJoin(
                    right,
                    _ => Observable.Never<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValues) =>
                    {
                        WriteStreamToConsole(rightValues, string.Format("Window{0}'s values", leftValue));
                        return rightValues.Aggregate((s1, s2) => s1 + s2);  //Just for fun return a concatenating string.
                    });
            WriteStreamToConsole(joinedStream.Merge(), "GroupJoin");
        }
        public void GroupJoin_with_no_closing_right()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .GroupJoin(
                    right,
                    _ => Observable.Empty<Unit>(),
                    _ => Observable.Never<Unit>(),
                    (leftValue, rightValues) =>
                    {
                        WriteStreamToConsole(rightValues, string.Format("Window{0}'s values", leftValue));
                        return rightValues.Aggregate((s1, s2) => s1 + s2)
                            .Catch((InvalidOperationException ex) => Observable.Empty<string>());
                    });
            WriteStreamToConsole(joinedStream.Merge(), "GroupJoin");
        }

        public IObservable<TResult> MyJoin<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(
            IObservable<TLeft> left,
            IObservable<TRight> right,
            Func<TLeft, IObservable<TLeftDuration>> leftDurationSelector,
            Func<TRight, IObservable<TRightDuration>> rightDurationSelector,
            Func<TLeft, TRight, TResult> resultSelector)
        {
            return Observable.GroupJoin
                (
                    left,
                    right,
                    leftDurationSelector,
                    rightDurationSelector,
                    (leftValue, rightValues) => rightValues.Select(rightValue => resultSelector(leftValue, rightValue))
                )
                .Merge();
        }
        public void MyJoin_with_no_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = MyJoin(
                    left,
                    right,
                    _ => Observable.Never<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "MyJoin");

        }


        private IObservable<IObservable<T>> MyWindowWithTime<T>(IObservable<T> source, TimeSpan windowPeriod)
        {
            return Observable.CreateWithDisposable<IObservable<T>>(o =>
                {
                    var windower = new Subject<long>();
                    var intervals = Observable.Concat(
                            Observable.Return(0L),
                            Observable.Interval(windowPeriod)
                        )
                        .Publish()
                        .RefCount();

                    var subscription = Observable.GroupJoin
                        (
                            windower,
                            source.Do(_ => { }, windower.OnCompleted),
                            _ => windower,
                            _ => Observable.Empty<Unit>(),
                            (left, sourceValues) => sourceValues
                        )
                        .Subscribe(o);
                    var intervalSubscription = intervals.Subscribe(windower);
                    return new CompositeDisposable(subscription, intervalSubscription);
                });
        }
        public void MyWindow_With_Time_returns_IObservable_of_IObservable_of_T()
        {
            var windowIdx = 0;
            var source = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(10);
            MyWindowWithTime(source, TimeSpan.FromMilliseconds(500))
                .Subscribe(window =>
                {
                    var thisWindowIdx = windowIdx++;
                    Console.WriteLine("--Starting new window");
                    WriteStreamToConsole(window, "Window" + thisWindowIdx);

                },
                () => Console.WriteLine("Completed"));
        }

        public void GarysOverLappingWindows()
        {
            var obs = Observable.Interval(TimeSpan.FromMilliseconds(200))
                                .Select(i => i * 100).Take(12);
            Func<long, IObservable<bool>> windowClosings =
                i => Observable.Interval(TimeSpan.FromMilliseconds(600))
                    .Select(_ => true);
            var movingWindowWithTime = obs.Window(obs, windowClosings);
            movingWindowWithTime.Select(s => s.ToList()).Subscribe(
                i => i.Subscribe(j =>
                {
                    foreach (var value in j) Console.Write("{0} ", value);
                }, () => Console.WriteLine()));
        }
public void SlightModsToGarysOverLappingWindows()
{
    var obs = Observable.Interval(TimeSpan.FromMilliseconds(200))
        .Select(i => i * 100)
        .Take(12);

    obs.Window(obs, i => Observable.Timer(TimeSpan.FromMilliseconds(600)))
        .Select(s => s.ToList())
        .Merge()
        .Subscribe(
            j =>
            {
                foreach (var value in j) Console.Write("{0} ", value);
                Console.WriteLine();
            });
}

        public void My_TWAP_or_VWAP_implementation()
        {
            /*
             * Create a stream that produces some values. 
             * Take overlapping windows and return the mean/average from each of the windows.
             * To use this for TWAP you will just return the prices from the resultSelector
             * To use this for VWAP you will just return the value (price * volume) from the resultSelector. This is not showen here.
             *  *TWAP is Time Weighted Average Price. ie You take all of the prices that happened 
             *    in a given time period and the TWAP is the average.
             *  *VWAP is Volume Weighted Average Price. ie You take all the Trades that happened 
             *    in a given time period and the VWAP is the average of the trade value 
             *    (trade volume * price for the trade). Eg a Trade to by 1,000 shares @ $1.3 has 
             *    a value of $1,300
Source |---1-1-3-6-2-335----7-------4221| Count  Sum    Mean  
Window0 ---1-1-3-6|                       4      11     2.75
Window1  --1-1-3-6-|                      4      11     2.75
Window2   -1-1-3-6-2|                     5      13     2.6
etc...     1-1-3-6-2-|                    5      13     2.6
            -1-3-6-2-3|                   5      15     3
             1-3-6-2-33|                  6      18     3
              -3-6-2-335|                 6      22     3.667
               3-6-2-335-|                6      22     3.667
                -6-2-335--|               5      19     3.8
                 6-2-335---|              5      19     3.8
                  -2-335----|             4      13     3.25
                   2-335----7|            5      20     4
                    -335----7-|           4      18     4.5
                     335----7--|          4      18     4.5
                      35----7---|         3      15     5
                       5----7----|        2      12     6
                        ----7-----|       1      7      7
                         ---7------|      1      7      7
                          --7-------|     1      7      7
                           -7-------4|    2      11     5.5
                            7-------42|   3      13     4.33
                             -------422|  3      8      2.67
                              ------4221| 4      9      2.25
                               -----4221| 4      9      2.25
                                ----4221| 4      9      2.25
                                 ---4221| 4      9      2.25
                                  --4221| 4      9      2.25
                                   -4221| 4      9      2.25
                                    4221| 4      9      2.25
                                     221| 3      5      1.667
                                      21| 2      3      1.5
                                       1| 1      1      1 
            */
            var values = new List<int?> { null, null, null, 1, null, 1, null, 3, null, 6, null, 2, null, 3, 3, 5, null, null, null, null, 7, null, null, null, null, null, null, null, 4, 2, 2, 1 };
            var valueStream = Observable.Interval(TimeSpan.FromMilliseconds(100))
                                                            .Select(i => (int)(i))
                                                            .Select(idx => values[idx])
                                                            .Take(values.Count)
                                                            .Where(value => value.HasValue)
                                                            .Select(value => value.Value);

            var twap = OverlappingWindowAverage_InitialAttempt(valueStream, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100), i => i);
            //var twap = OverlappingWindowAverage_Debug(valueStream, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100), i => i);
            //var twap = OverlappingWindowAverage(valueStream, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100), i => i);
            WriteStreamToConsole(twap, "TWAP");

        }

public IObservable<string> OverlappingWindowAverage_Debug<T>
(
    IObservable<T> source,
    TimeSpan windowPeriod,
    TimeSpan accuracy,
    Func<T, Decimal> resultSelector
)
{
    //We will subscribe twice to this so Publish and refcount to ensure we share the stream.
    var inputIdx = 0;
    var receivedIdx = 0;
    var sourcePublished = source.Publish().RefCount().Select(value => new { Idx = inputIdx++, Value = value });

    var ticks =
        //Open a new window every period defined by the accuracy argument
                        Observable.Interval(accuracy)
        //Stop ticking if the source completes
                                .TakeUntil(sourcePublished.IgnoreValues().Materialize());

    return sourcePublished.Window
        (
            ticks,
        //Close window after windowPeriod
            windowOpening => Observable.Timer(windowPeriod)
        )
        //Average the values and return the single result as an IObservable<Decimal>
        .Select(windowValues =>
                    {
                        var idx = receivedIdx++;
                        return windowValues.Select(pair => resultSelector(pair.Value))
                            .Average()
                            //Empty windows will throw InvalidOperationException from a call to Average
                            .Catch((InvalidOperationException ex) =>
                                        {
                                            Console.WriteLine(
                                                "  Ignoring empty stream for average {0}", ex);
                                            return Observable.Empty<Decimal>();
                                        })
                            .Replay().RefCount()
                            .Select(avg => string.Format("{0} {1}", idx, avg));
                    }
        )
        //At this point in time I will have an IObservable<IObservable<decimal>>
        //     ie each of the averages will be in their own child IObservable

        //Merge all of the Average child IObservable in to a flattened IObservable
        //.Merge();
        .Concat();   // just pumps the first value then get's its nickers in a twist /deadlock
    //.Merge(1);      //same as Concat()
    //.Switch();      //Junk??!
    //.Serialize();
}

public IObservable<decimal> OverlappingWindowAverage_InitialAttempt<T>
(
    IObservable<T> source,
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
                                .TakeUntil(sourcePublished.IgnoreValues().Materialize());

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

    public static class Extensions
    {
        public static IObservable<T> Serialize<T>(this IObservable<IObservable<T>> source)
        {
            return Observable.CreateWithDisposable<T>(o =>
            {
                var outerScheduler = new EventLoopScheduler("IObservable Serialize thread for outer");
                var queue = new BlockingCollection<IObservable<T>>();

                var resources = new CompositeDisposable(outerScheduler, queue);

                resources.Add(outerScheduler.Schedule(() =>
                {
                    var streams = queue.GetConsumingEnumerable();
                    foreach (var stream in streams)
                    {
                        //This is almost right. However this seems to only yeild when it completes. I need it pumps as soon as values arrive too.
                        stream.Run(value => o.OnNext(value), ex => o.OnError(ex));
                    }
                    o.OnCompleted();
                }));

                resources.Add(source.Subscribe(stream =>
                {
                    var replay = stream.Replay();
                    resources.Add(replay.Connect());
                    queue.Add(replay);
                },
                ex => queue.CompleteAdding(),
                () => queue.CompleteAdding()));

                return resources;
            });
        }
    }
}
