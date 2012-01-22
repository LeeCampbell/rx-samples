using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
    class AggregationExamples
    {
        public void a()
        {
            var subject = new Subject<int>();
            //IO<T> --> IO<T>
            subject.Average();
            subject.Count();
            subject.LongCount();
            //subject.FirstAsync();     //v1.1 but not flagged as experimental
            subject.Max();
            subject.Min();
            subject.Sum();
            subject.Aggregate((last, current) => last + current);
            subject.Scan((last, current) => last + current);

            //IO<T> --> T
            subject.First();
            subject.FirstOrDefault();
            subject.Last();
            subject.LastOrDefault();
            subject.Single();
            subject.SingleOrDefault();
        }

        public void Count()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("subject Completed"));
            var count = subject.Count();
            count.Subscribe(
                i => Console.WriteLine("count is {0}", i),
                () => Console.WriteLine("count Completed"));

            var myCount = subject.Aggregate((acc, _) => acc + 1);
            myCount.Subscribe(
                i => Console.WriteLine("myCount is {0}", i),
                () => Console.WriteLine("myCount Completed"));

            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public void LongCount()
        {
            Console.WriteLine("Would take 7 hours to run on my little PC. Range(0, 1million) takes 13seconds :(");
            var timer = new TimeIt("LongCount");
            var lotsOfValues = Observable.Range(0, int.MaxValue);
            var count = lotsOfValues.Count();
            count.Subscribe(
                i => Console.WriteLine("count is {0}", i),
                () => { Console.WriteLine("count Completed"); timer.Dispose(); });
        }

        public void MinAndAverage()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("subject Completed"));
            var min = subject.Min();
            min.Subscribe(
                i => Console.WriteLine("min is {0}", i),
                () => Console.WriteLine("min Completed"));
            var average = subject.Average();
            average.Subscribe(
                i => Console.WriteLine("avg is {0}", i),
                () => Console.WriteLine("avg Completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public void Min_on_null_custom_type()
        {
            var subject = new Subject<FilteringExamples.Account>();
            var min = subject.Min();
            min.Subscribe(
                i => Console.WriteLine("min is {0}", i),
                () => Console.WriteLine("min Completed"));
            subject.OnNext(null);
            subject.OnCompleted();
        }

        public void Sum()
        {
            //TODO :
            throw new NotImplementedException();
        }

        //-----------------------------------------//
        public void First_Is_a_blocking_call()
        {
            var interval = Observable.Interval(TimeSpan.FromSeconds(3));
            Console.WriteLine("Will block for 3s before returning");
            Console.WriteLine(interval.First());
        }

        public void First_will_throw_if_stream_contains_no_elements()
        {
            //InvalidOperationException("Sequence contains no elements.")
            try
            {
                Console.WriteLine(Observable.Empty<int>().First());
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
            }
        }

        public void First_and_BehaviorSubject()
        {
            var subject = new BehaviorSubject<int>(0);
            Console.WriteLine(subject.First());
            subject.OnCompleted();
            Console.WriteLine(subject.First());

        }

        public void FirstOrDefault()
        {
            //TODO :
            throw new NotImplementedException();
        }
        public void Last()
        {
            //TODO :
            throw new NotImplementedException();
        }
        public void LastOrDefault()
        {
            //TODO :
            throw new NotImplementedException();
        }
        public void Single()
        {
            var subject = new ReplaySubject<int>();
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();
            Console.WriteLine(subject.Single());

        }
        public void SingleOrDefault()
        {
            var subject = new ReplaySubject<int>();
            subject.OnCompleted();
            Console.WriteLine(subject.SingleOrDefault());
        }


        //------------------------------------------------------//
        public void Aggregate_with_one_value()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var agr = subject.Aggregate((last, current) => last + current)
                .Subscribe(
                    i => Console.WriteLine("agr is {0}", i),
                    () => Console.WriteLine("agr Completed"));

            subject.OnNext(1);
            subject.OnCompleted();
        }

        public void Aggregate_as_Sum()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var sum = subject.Aggregate((acc, current) =>
                                            {
                                                Console.WriteLine("inside the accumulator with ({0},{1})", acc, current);
                                                return acc + current;
                                            })
                .Subscribe(
                    i => Console.WriteLine("agr is {0}", i),
                    () => Console.WriteLine("agr Completed"));

            subject.OnNext(1);
            //subject.OnNext(2);
            //subject.OnNext(3);
            subject.OnCompleted();
        }

        public void Aggregate_with_empty_sequence_throws()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var sum = subject.Aggregate((acc, current) => acc + current);
            sum.Subscribe(
                i => Console.WriteLine("agr is {0}", i),
                ex => Console.WriteLine("arg OnError {0}", ex),
                () => Console.WriteLine("agr Completed"));

            subject.OnCompleted();
        }

        public void Aggregate_seed_with_empty_sequence_returns_seed()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var sum = subject.Aggregate(0, (acc, current) => acc + current);
            sum.Subscribe(
                i => Console.WriteLine("agr is {0}", i),
                () => Console.WriteLine("agr Completed"));

            subject.OnCompleted();
        }

        public void Aggregate_as_Min()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));

            var min = subject.MyMin();
            min.Subscribe(
                i => Console.WriteLine("min is {0}", i),
                () => Console.WriteLine("min Completed"));

            var max = subject.MyMax();
            max.Subscribe(
                i => Console.WriteLine("max is {0}", i),
                () => Console.WriteLine("max Completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }



        public void Aggregate_as_Median()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var median = subject.Aggregate(
                                new List<int>(),
                                (list, current) =>
                                {
                                    list.Add(current);
                                    return list;
                                })
                            .Select(list =>
                                        {
                                            list.Sort();
                                            var midIndex = (list.Count - 1) / 2;
                                            return list[midIndex];
                                        });

            median.Subscribe(
                i => Console.WriteLine("median is {0}", i),
                () => Console.WriteLine("median Completed"));

            subject.OnNext(3);
            //subject.OnNext(5);
            //subject.OnNext(1);
            //subject.OnNext(2);
            //subject.OnNext(4);
            subject.OnCompleted();
        }
        public void Aggregate_as_Mode()
        {
            throw new NotImplementedException();
            //var subject = new Subject<int>();
            //subject.Subscribe(
            //        Console.WriteLine,
            //        () => Console.WriteLine("subject Completed"));
            //var sum = subject.Aggregate(
            //                    new Dictionary<int, int>(),
            //                    (values, current) => values..Add(current))
            //                .Select()

            //    .Subscribe(
            //        i => Console.WriteLine("agr is {0}", i),
            //        () => Console.WriteLine("agr Completed"));

            //subject.OnNext(1);
            //subject.OnNext(2);
            //subject.OnNext(3);
            //subject.OnCompleted();
        }

        public void Scan()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var scan = subject.Scan(0, (acc, current) => acc + current);
            scan.Subscribe(
                i => Console.WriteLine("scan is {0}", i),
                () => Console.WriteLine("scan Completed"));



            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public void Scan_and_Distinct_for_runningMin()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("subject Completed"));
            var min = subject.RunningMin();
            min.Subscribe(
                i => Console.WriteLine("min is {0}", i),
                () => Console.WriteLine("min Completed"));

            var max = subject.RunningMax();
            max.Subscribe(
                i => Console.WriteLine("max is {0}", i),
                () => Console.WriteLine("max Completed"));



            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(3);
            subject.OnNext(3);
            subject.OnNext(5);
            subject.OnNext(0);
            subject.OnCompleted();

        }


        //public void Scan_allows_custom_aggregation()
        //{
        //    var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
        //    var scan = interval.Scan(new Averages(), (acc, i) => new Averages(acc, i));
        //    WriteStreamToConsole(scan, "scan");
        //}

        ///// <summary>
        ///// I am not sure what the practical difference is between Aggregate and Scan.
        ///// </summary>
        //public void Aggergate_can_be_used_to_create_your_own_aggregates()
        //{
        //    var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
        //    var aggregate = interval.Aggregate(new Averages(), (acc, i) => new Averages(acc, i));
        //    WriteStreamToConsole(aggregate, "aggregate");
        //}

        //private class Averages
        //{
        //    public Averages()
        //    {
        //        CurrentValue = 0;
        //        LastValues = new List<long>();
        //    }
        //    public Averages(Averages accumulator, long currentValue)
        //    {
        //        CurrentValue = currentValue;
        //        LastValues = new List<long>(accumulator.LastValues);
        //        if (LastValues.Count > 5)
        //        {
        //            LastValues.RemoveAt(0);
        //        }
        //        LastValues.Add(accumulator.CurrentValue);
        //    }
        //    public long CurrentValue { get; private set; }
        //    public double TrailingAverage()
        //    {
        //        return LastValues.Average();
        //    }
        //    public List<long> LastValues { get; private set; }
        //    public override string ToString()
        //    {
        //        return String.Format("{0} (Avg of last 5 = {1})", CurrentValue, TrailingAverage());
        //    }
        //}
    }

    public class TimeIt : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _watch;

        public TimeIt(string name)
        {
            _name = name;
            _watch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _watch.Stop();
            Console.WriteLine("{0} took {1}", _name, _watch.Elapsed);
        }
    }

    public static class AggExtensions
    {
        public static IObservable<bool> MyAny<T>(this IObservable<T> source)
        {
            return Observable.Create<bool>(
                o =>
                {
                    var hasValues = false;
                    return source
                        .Take(1)
                        .Subscribe(
                            _ => hasValues = true,
                            o.OnError,
                            () =>
                            {
                                o.OnNext(hasValues);
                                o.OnCompleted();
                            });
                });
        }

        public static IObservable<bool> MyAny<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).MyAny();
        }

        public static IObservable<T> MyMin<T>(this IObservable<T> source)
        {
            return source.Aggregate(
                (min, current) => Comparer<T>
                    .Default
                    .Compare(min, current) > 0
                        ? current
                        : min);
        }

        public static IObservable<T> MyMax<T>(this IObservable<T> source)
        {
            var comparer = Comparer<T>.Default;
            Func<T, T, T> max =
                (x, y) =>
                {
                    if (comparer.Compare(x, y) < 0)
                    {
                        return y;
                    }
                    return x;
                };
            return source.Aggregate(max);
        }

        public static IObservable<T> RunningMin<T>(this IObservable<T> source)
        {
            var comparer = Comparer<T>.Default;
            Func<T, T, T> min = (x, y) => comparer.Compare(x, y) < 0 ? x : y;
            return source.Scan(min)
                .DistinctUntilChanged();
        }

        public static IObservable<T> RunningMax<T>(this IObservable<T> source)
        {
            return source.Scan(MaxOf)
                .DistinctUntilChanged();
        }
        private static T MaxOf<T>(T x, T y)
        {
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(x, y) < 0)
            {
                return y;
            }
            return x;
        }
    }
}
