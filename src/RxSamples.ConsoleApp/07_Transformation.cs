using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace RxSamples.ConsoleApp
{
    class TransformationExamples
    {
        public void Simple_SelectMany_to_expand()
        {
            //var source = Observable.Return(1);
            var source = Observable.Return(3);

            var result = source.SelectMany(i => Observable.Range(0, i));

            result.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public void SelectMany_to_expand_with_multivalues()
        {
            var source = new Subject<int>();
            var result = source.SelectMany(i => Observable.Range(0, i));
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
        }

        public void SelectMany_to_char()
        {
            var source = Observable.Return(1);
            Func<int, char> letter = i => (char)(i + 64);
            var result = source.SelectMany(i => Observable.Return(letter(i)));
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void SelectMany_to_char_with_filter()
        {
            var source = Observable.Range(1, 30);
            Func<int, char> letter = i => (char)(i + 64);
            var result = source.SelectMany(
                i =>
                {
                    if (0 < i && i < 27)
                    {
                        return Observable.Return(letter(i));
                    }
                    else
                    {
                        return Observable.Empty<char>();
                    }
                });
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void SelectMany_to_make_where()
        {
            var source = Observable.Range(1, 30);

            var result = source.MyWhere(i => i % 2 == 0);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void SelectMany_to_make_Skip()
        {
            var source = Observable.Range(1, 20);

            var result = source.MySkip(1);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void SelectMany_to_make_Take()
        {
            var source = Observable.Range(1, 20);

            var result = source.MyTake(10);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void Cast_takes_objects_to_T()
        {
            var source = new Subject<object>();
            var result = source.Cast<int>();
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnCompleted();
        }

        public void Cast_fails_on_invalid_cast()
        {
            var source = new Subject<object>();
            var result = source.Cast<int>();
            result.Subscribe(
                Console.WriteLine,
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnNext("3"); //Fail!
        }

        public void OfType_takes_objects_to_T_safely()
        {
            var source = new Subject<object>();
            var result = source.OfType<int>();
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnNext("3"); //Ignored
            source.OnNext(4);
            source.OnCompleted();
        }

        public void TimeStamp()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                    .Take(3);
            var result = source.Timestamp();
            result.Subscribe(
                ts => Console.WriteLine(ts),
                //ts => Console.WriteLine("{0}@{1}", ts.Value, ts.Timestamp),
                () => Console.WriteLine("Completed"));

        }

        public void TimeInterval()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                    .Take(5);
            var result = source.TimeInterval();
            result.Subscribe(
                //ti => Console.WriteLine(ti),
                ti => Console.WriteLine("{0}@{1}", ti.Value, ti.Interval),
                () =>Console.WriteLine("Completed"));

        }
        
        //No difference really.
        public void TimeInterval_with_StringBuilder()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                    .Take(5);
            var result = source.TimeInterval();
            var sb = new StringBuilder();
            result.Subscribe(
                //ti => Console.WriteLine(ti),
                //ti => Console.WriteLine("{0}@{1}", ti.Value, ti.Interval),
                ti =>
                {
                    sb.AppendFormat("{0}@{1}", ti.Value, ti.Interval);
                    sb.AppendLine();
                },
                () =>
                {
                    Console.Write(sb.ToString());
                    Console.WriteLine("Completed");
                });
        }

        //No difference really.
        public void TimeInterval_with_StringBuilder_and_OwnThread()
        {
            var generatorScheduler = new EventLoopScheduler();
            var watcherScheduler = new EventLoopScheduler();

            var source = Observable.Interval(TimeSpan.FromSeconds(1), generatorScheduler)
                    .Take(5);
            var result = source.TimeInterval(watcherScheduler);
            var sb = new StringBuilder();
            result.Subscribe(
                //ti => Console.WriteLine(ti),
                //ti => Console.WriteLine("{0}@{1}", ti.Value, ti.Interval),
                ti =>
                {
                    sb.AppendFormat("{0}@{1}", ti.Value, ti.Interval);
                    sb.AppendLine();
                },
                () =>
                {
                    Console.Write(sb.ToString());
                    Console.WriteLine("Completed");
                });
        }

        public void Materialize_OnCompleted()
        {
            var source = new Subject<int>();
            var result = source.Materialize();
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnCompleted();
        }

        public void Materialize_OnError()
        {
            var source = new Subject<int>();
            var result = source.Materialize();
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnError(new Exception("Fail?"));
        }

        public void Dematerialize_OnCompleted()
        {
            var source = new Subject<int>();
            var materialized = source.Materialize();
            materialized.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            materialized.Dematerialize().Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnCompleted();
        }
    }

    public static class TransformExtensions
    {
        public static IObservable<T> MyWhere<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return source.SelectMany(
                item =>
                {
                    if (predicate(item))
                    {
                        return Observable.Return(item);
                    }
                    else
                    {
                        return Observable.Empty<T>();
                    }
                });
        }

        public static IObservable<T> MySkip<T>(this IObservable<T> source, int elementsToSkip)
        {
            var count = 0;
            return source.SelectMany(
                item =>
                {
                    if (elementsToSkip < ++count)
                    {
                        return Observable.Return(item);
                    }
                    else
                    {
                        return Observable.Empty<T>();
                    }
                });
        }
        public static IObservable<T> MyTake<T>(this IObservable<T> source, int elementsToTake)
        {
            var count = elementsToTake;
            return source.SelectMany(
                item =>
                {
                    if (0 < count--)
                    {
                        return Observable.Return(item);
                    }
                    else
                    {
                        return Observable.Empty<T>();
                    }
                });
        }

        public static IObservable<TResult> Select<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TResult> selector)
        {
            //return source.SelectMany(value => Observable.Return(selector(value)));

            return Observable.Create<TResult>(o => source.Subscribe(
                x =>
                {
                    try
                    {
                        var y = selector(x);
                        o.OnNext(y);
                    }
                    catch (Exception e)
                    {
                        o.OnError(e);
                    }
                },
                o.OnError,
                o.OnCompleted));
        }
    }
}
