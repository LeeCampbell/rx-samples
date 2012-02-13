using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace RxSamples.ConsoleApp
{
    class FlowControlExamples : ExamplesBase
    {
        public void Simple_Subscribe_will_rethrow_Errors()
        {
            try
            {
                var subject = new Subject<int>();
                subject.Subscribe(Console.WriteLine);
                subject.OnNext(1);
                subject.OnError(new Exception("Test Exception"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catching exception using SEH(Sturcted Exception Handling)");
                Console.WriteLine(ex.Message);
            }
        }

        private IObservable<int> ErrorsAfter3()
        {
            return Observable.Create<int>(
              o =>
              {
                  o.OnNext(1);
                  o.OnNext(2);
                  o.OnNext(3);
                  o.OnError(new NotImplementedException("Test Exception"));
                  return () => { };
              });
        }

        #region Catch examples

        public void Catch_to_swallow_exceptions()
        {
            var source = new Subject<int>();
            var result = source.Catch(Observable.Empty<int>());

            result.Subscribe(
                Console.WriteLine,
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnError(new Exception("Fail!"));
        }

        public void Catch_to_swallow_specific_exceptions()
        {
            var source = new Subject<int>();
            var result = source.Catch<int, TimeoutException>(tx => Observable.Return(-1));

            result.Subscribe(
                Console.WriteLine,
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnError(new TimeoutException());
        }

        public void Catch_to_ignore_wrong_exception_type()
        {
            var source = new Subject<int>();
            var result = source.Catch<int, TimeoutException>(tx => Observable.Return(-1));

            result.Subscribe(
                Console.WriteLine,
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Completed"));

            source.OnNext(1);
            source.OnNext(2);
            source.OnError(new ArgumentException("Fail!"));
        }

        //public void Catch_is_the_same_as_OnErrorResumeNext_when_no_error_type_is_specified()
        //{
        //    var first = ErrorsAfter3();
        //    var second = Observable.Return(10);
        //    var catchStream = first.Catch(second);
        //    WriteStreamToConsole(catchStream, "catchStream");
        //}
        //public void Catch_will_try_another_stream_on_a_specific_exception()
        //{
        //    var first = ErrorsAfter3();
        //    var second = Observable.Return(10);
        //    var catchStream = first.Catch((NotImplementedException ex) => second);
        //    WriteStreamToConsole(catchStream, "catchStream");
        //}
        //public void Catch_will_OnError_if_OnError_not_the_specific_exception()
        //{
        //    var first = ErrorsAfter3();
        //    var second = Observable.Return(10);
        //    var catchStream = first.Catch((ArgumentOutOfRangeException ex) => second);
        //    WriteStreamToConsole(catchStream, "catchStream");
        //}

        #endregion

        #region Finally examples for book
        public void Finally_OnCompleted()
        {
            var source = new Subject<int>();
            var result = source.Finally(() => Console.WriteLine("Finally"));
            result.Subscribe(
                Console.WriteLine,
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnCompleted();
        }

        public void Finally_OnError()
        {
            var source = new Subject<int>();
            var result = source.Finally(() => Console.WriteLine("Finally"));
            result.Subscribe(
                Console.WriteLine,
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnError(new Exception("Fail"));
        }

        public void Finally_dispose_subscription()
        {
            var source = new Subject<int>();
            var result = source.Finally(() => Console.WriteLine("Finally"));
            var subscription = result.Subscribe(
                Console.WriteLine,
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            subscription.Dispose();
        }

        public void Finally_OnError_No_handler()
        {
            var source = new Subject<int>();
            var result = source.Finally(() => Console.WriteLine("Finally"));
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnError(new Exception("Fail"));
        }

        public void Custom_Finally_OnComplete()
        {
            var source = new Subject<int>();

            var result = source.MyFinally(() => Console.WriteLine("MyFinally"));
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnCompleted();
        }

        public void Custom_Finally_OnError()
        {
            var source = new Subject<int>();

            var result = source.MyFinally(() => Console.WriteLine("MyFinally"));
            result.Subscribe(
                Console.WriteLine,
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnError(new Exception("Fail"));
        }

        public void Custom_Finally_OnError_No_handler()
        {
            var source = new Subject<int>();

            var result = source.MyFinally(() => Console.WriteLine("MyFinally"));
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnError(new Exception("Fail"));
        }

        public void Custom_Finally_dispose_Subscription()
        {
            var source = new Subject<int>();

            var result = source.MyFinally(() => Console.WriteLine("MyFinally"));
            var subscription = result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            subscription.Dispose();
        }
        #endregion

        //Using
        public void Normal_using_keyword()
        {
            using (new TimeIt("Outer SpinWait"))
            {
                using (new TimeIt("Inner SpinWait"))
                {
                    Thread.SpinWait(10000);
                }
                Thread.SpinWait(10000);
            }
        }
        public void RxUsing_to_bind_resource_lifetime_to_subscription_lifetime()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1));
            var result = Observable.Using(
                () => new TimeIt("Subscription Timer"),
                timeIt => source);
            result.Take(5)
                .Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }

        //OnErrorResumeNext
        public void OnErrorResumeNext_will_try_another_stream_OnError()
        {
            var first = ErrorsAfter3();
            var second = Observable.Return(10);
            var onErrorResumeNext = first.OnErrorResumeNext(second);
            WriteStreamToConsole(onErrorResumeNext, "onErrorResumeNext");
        }

        //Retry
        public void Retry_will_retry_on_error_forever()
        {
            var errorAfter3 = ErrorsAfter3();

            var result = errorAfter3.Retry();
            var subscription = result
                .Take(15)
                .Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
            Console.ReadLine();
            subscription.Dispose();
        }

        public void Retry_will_retry_on_error_forever_MyFix()
        {
            var errorAfter3 = ErrorsAfter3();

            var result = errorAfter3.MyRetry();
            result
                .Take(15)
                .Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }

        public void Retry_with_CurrentScheduler_still_broke()
        {
            var source = ErrorsAfter3();

            var result = source.Retry();
            result
                .Take(15)
                .SubscribeOn(Scheduler.CurrentThread)
                .ObserveOn(Scheduler.CurrentThread)
                .Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }
        public void Retry_with_TaskPoolScheduler_works()
        {
            var source = ErrorsAfter3();

            var result = source.Retry();
            result
                .Take(15)
                .SubscribeOn(Scheduler.TaskPool)
                .ObserveOn(Scheduler.TaskPool)
                .Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }

        public void Retry_with_Schedule_in_create()
        {
            var source = Observable.Create<int>(o =>
                    {
                        Scheduler.CurrentThread.Schedule(()=>o.OnNext(1));
                        Scheduler.CurrentThread.Schedule(()=>o.OnNext(2));
                        Scheduler.CurrentThread.Schedule(()=>o.OnNext(3));
                        Scheduler.CurrentThread.Schedule(()=>o.OnError(new Exception("Fail!")));
                        return Disposable.Empty;
                    });

            var result = source.Retry();
            result
                .Take(15)
                .Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }

        public void Retry_with_argument_of_2_will_retry_once()
        {
            var errorAfter3 = ErrorsAfter3();
            var result = errorAfter3.Retry(2);
            result.Subscribe(
                    Console.WriteLine,
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }


        //Repeat --> Nice segway to Combining streams. :)


    }

    public static class ExampleExtensions
    {
        /// <summary>
        /// Logs implicit notifications to console. Showcase the Materialise, Dematerialise and Do extension methods.
        /// </summary>
        /// <example>
        /// <code>myStream.Log().Subscribe(....);</code>
        /// </example>
        public static IObservable<T> Log<T>(this IObservable<T> stream)
        {
            return stream.Materialize()
                .Do(n => Console.WriteLine((object)n))
                .Dematerialize();
        }


        public static IObservable<T> MyFinally<T>(this IObservable<T> source, Action finallyAction)
        {
            return Observable.Create<T>(o =>
                                            {
                                                var finallyOnce = Disposable.Create(finallyAction);
                                                var subscription = source.Subscribe(
                                                    o.OnNext,
                                                    ex =>
                                                    {
                                                        try { o.OnError(ex); }
                                                        finally { finallyOnce.Dispose(); }
                                                    },
                                                    () =>
                                                    {
                                                        try { o.OnCompleted(); }
                                                        finally { finallyOnce.Dispose(); }
                                                    });

                                                return new CompositeDisposable(subscription, finallyOnce);

                                            });
        }

        private static IEnumerable<T> RepeatInfinite<T>(T value)
        {
            while (true)
                yield return value;
        }

        public static IObservable<TSource> MyRetry<TSource>(this IObservable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatInfinite(source).MyCatch();
        }


        public static IObservable<TSource> MyRepeat<TSource>(this IObservable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatInfinite(source).Concat();
        }

        public static IObservable<TSource> MyCatch<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");
            return Observable.Create<TSource>(
                observer =>
                    {
                        var gate = new AsyncLock();
                        bool isDisposed = false;    //Hmm maybe needs to be set??
                        var e = sources.GetEnumerator();
                        var subscription = new SerialDisposable();
                        Exception lastException = null;
                        //Seems to be a bug. Never escapes to allow disposal.
                        //IDisposable scheduledAction = Scheduler.Immediate.Schedule(
                        IDisposable scheduledAction = Scheduler.CurrentThread.Schedule(
                            self => gate.Wait(
                                () =>
                                    {
                                        IObservable<TSource> current = null;
                                        bool hasValue = false;
                                        Exception error = null;
                                        if (isDisposed)
                                            return;
                                        try
                                        {
                                            hasValue = e.MoveNext();
                                            if (hasValue)
                                                current = e.Current;
                                            else
                                                e.Dispose();
                                        }
                                        catch (Exception ex)
                                        {
                                            error = ex;
                                            e.Dispose();
                                        }
                                        if (error != null)
                                            observer.OnError(error);
                                        else if (!hasValue)
                                        {
                                            if (lastException != null)
                                                observer.OnError(lastException);
                                            else
                                                observer.OnCompleted();
                                        }
                                        else
                                        {
                                            subscription.Disposable = current.Subscribe(observer.OnNext, exception => self(), observer.OnCompleted);
                                        }
                                    }));
                        return (IDisposable)new CompositeDisposable(new[]
                                                                        {
                                                                            Disposable.Create(()=>isDisposed=true),
                                                                            subscription,
                                                                            scheduledAction,
                                                                            Disposable.Create(() => gate.Wait(e.Dispose))
                                                                        });
                    });
        }

        public static IObservable<TSource> MyConcat<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");
            return Observable.Create<TSource>(
                observer =>
                    {
                        bool isDisposed = false;
                        var e = sources.GetEnumerator();
                        var subscription = new SerialDisposable();
                        var gate = new AsyncLock();
                        IDisposable scheduledAction = Scheduler.CurrentThread.Schedule(
                            self => gate.Wait(
                                () =>
                                    {
                                        IObservable<TSource> nextSequence = null;
                                        bool hasValue = false;
                                        Exception error = null;
                                        if (isDisposed)
                                            return;
                                        try
                                        {
                                            hasValue = e.MoveNext();
                                            if (hasValue)
                                                nextSequence = e.Current;
                                            else
                                                e.Dispose();
                                        }
                                        catch (Exception ex)
                                        {
                                            error = ex;
                                            e.Dispose();
                                        }
                                        if (error != null)
                                            observer.OnError(error);
                                        else if (!hasValue)
                                        {
                                            observer.OnCompleted();
                                        }
                                        else
                                        {
                                            subscription.Disposable = nextSequence.Subscribe(
                                                observer.OnNext, 
                                                observer.OnError, 
                                                self);
                                        }
                                    }));
                        return new CompositeDisposable(new[]
                                                           {
                                                               subscription,
                                                               scheduledAction,
                                                               Disposable.Create(() => gate.Wait(e.Dispose))
                                                           });
                    });
        }

        internal sealed class AsyncLock
        {
            private Queue<Action> queue = new Queue<Action>();
            private bool isAcquired;
            private bool hasFaulted;

            public void Wait(Action action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                bool flag = false;
                lock (this.queue)
                {
                    if (!this.hasFaulted)
                    {
                        this.queue.Enqueue(action);
                        flag = !this.isAcquired;
                        this.isAcquired = true;
                    }
                }
                if (!flag)
                    return;
                while (true)
                {
                    Action action1 = (Action)null;
                    lock (this.queue)
                    {
                        if (this.queue.Count > 0)
                        {
                            action1 = this.queue.Dequeue();
                        }
                        else
                        {
                            this.isAcquired = false;
                            break;
                        }
                    }
                    try
                    {
                        action1();
                    }
                    catch
                    {
                        lock (this.queue)
                        {
                            this.queue.Clear();
                            this.hasFaulted = true;
                        }
                        throw;
                    }
                }
            }
        }
    }
}