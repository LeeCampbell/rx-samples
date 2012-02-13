using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
namespace RxSamples.ConsoleApp
{
    class LeavingTheMonadExamples
    {
        #region ForEach
        public void ForEach_is_Blocking()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(8);
            source.ForEach(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));
            Console.WriteLine("completed @ {0}", DateTime.Now);
        }

        public void Subscribe_is_not_Blocking()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(8);
            source.Subscribe(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));
            Console.WriteLine("after Subscribe @ {0}", DateTime.Now);
        }

        public void ForEach_with_TryCatch()
        {
            var source = Observable.Throw<int>(new Exception("Fail"));
            try
            {
                source.ForEach(Console.WriteLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("errored @ {0} with {1}", DateTime.Now, ex.Message);
            }
            finally
            {
                Console.WriteLine("completed @ {0}", DateTime.Now);
            }
        }
        #endregion

        #region ToEnumerable, Array, ToList, ToDictionary and ToLookup

        public void Slow_consumer_spike()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Timestamp();

            source.Subscribe(i => WriteTS(i, period + period));
        }
        public void Slow_Enumerable_consumer_spike()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Timestamp()
                                   .ToEnumerable();
            foreach (var i in source)
            {
                WriteTS(i, period + period);
            }
        }

        private void WriteTS<T>(Timestamped<T> i, TimeSpan delay)
        {
            Thread.Sleep(delay);
            Console.WriteLine("received {0:o} {1} @ {2:o}", i.Timestamp, i.Value, DateTime.Now);
        }

        public void ToEnumerable_caches_and_then_blocks_when_cache_drained()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5);
            var result = source.ToEnumerable();
            foreach (var value in result)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("done");
        }

        public void ToEnumerable_Subscribes_immediately()
        {
            var source = Observable.Create<int>(o =>
                                                    {
                                                        Console.WriteLine("Subscribed");
                                                        o.OnNext(1);
                                                        o.OnCompleted();
                                                        return Disposable.Empty;
                                                    });
            Console.WriteLine("Calling ToEnumerable()...");
            var result = source.ToEnumerable();
            Thread.Sleep(1000);
            Console.WriteLine("returned from ToEnumerable()");
            foreach (var value in result)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("done");
        }

        public void ToEnumerable_OnError()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Concat(Observable.Throw<long>(new Exception("Fail!")));
            var result = source.ToEnumerable();

            try
            {
                foreach (var value in result)
                {
                    Console.WriteLine(value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("done");
        }

        public void ToArray_caches_all_values_then_notifies_the_whole_array()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5);
            var result = source.ToArray();
            result.Subscribe(
                arr =>
                {
                    Console.WriteLine("Received array");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                () => Console.WriteLine("Completed")
                );

            Console.WriteLine("Subscribed");
        }

        public void ToArray_OnError()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Concat(Observable.Throw<long>(new Exception("Fail!")));
            var result = source.ToArray();
            result.Subscribe(
                arr =>
                {
                    //Wont get any values as the sequence errored.
                    Console.WriteLine("Received array");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            Console.WriteLine("Subscribed");
        }

        public void ToList_caches_all_values_then_notifies_the_whole_array()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5);
            var result = source.ToList();
            result.Subscribe(
                list =>
                {
                    Console.WriteLine("Received list");
                    foreach (var value in list)
                    {
                        Console.WriteLine(value);
                    }
                },
                () => Console.WriteLine("Completed")
                );

            Console.WriteLine("Subscribed");
        }

        public void ToList_OnError()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Concat(Observable.Throw<long>(new Exception("Fail!")));
            var result = source.ToList();
            result.Subscribe(
                list =>
                {
                    //Wont get any values as the sequence errored.
                    Console.WriteLine("Received array");
                    foreach (var value in list)
                    {
                        Console.WriteLine(value);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            Console.WriteLine("Subscribed");
        }

        public void ToDictionary_caches_all_values_then_notifies_the_whole_dictionary()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5);
            var result = source.ToDictionary(i => i.ToString());
            result.Subscribe(
                arr =>
                {
                    Console.WriteLine("Received dictionary");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed")
                );

            Console.WriteLine("Subscribed");
        }

        public void ToDictionary_throws_on_duplicate_keys()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5);
            var result = source.ToDictionary(i => i % 2);
            result.Subscribe(
                arr =>
                {
                    Console.WriteLine("Received dictionary");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed")
                );

            Console.WriteLine("Subscribed");
        }

        public void ToDictionary_OnError()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Concat(Observable.Throw<long>(new Exception("Fail!")));
            var result = source.ToDictionary(i => i.ToString());
            result.Subscribe(
                arr =>
                {
                    Console.WriteLine("Received dictionary");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed")
                );

            Console.WriteLine("Subscribed");
        }

        public void ToLookup_caches_all_values_then_notifies_the_whole_lookup()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5);
            var result = source.ToLookup(i => i % 2);
            result.Subscribe(
                lookup =>
                {
                    Console.WriteLine("Received loopup");
                    var flat = from grouping in lookup
                               from value in grouping
                               select new { Key = grouping.Key, Value = value };
                    foreach (var entry in flat)
                    {
                        Console.WriteLine(entry);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            Console.WriteLine("Subscribed");
        }

        public void ToLookup_OnError()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                                   .Take(5)
                                   .Concat(Observable.Throw<long>(new Exception("Fail!")));
            var result = source.ToLookup(i => i % 2);
            result.Subscribe(
                arr =>
                {
                    //Wont get any values as the sequence errored.
                    Console.WriteLine("Received array");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            Console.WriteLine("Subscribed");
        }

        #endregion

        #region Latest, MostRecent & Next

        //Caches latest value, if no values since last value, will block
        public void Latest_result_will_cache_the_last_Notification()
        {
            Console.WriteLine("{0:o} Starting", DateTime.Now);
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(5);
            var result = source.Latest();
            //var result = source.MostRecent(-1);
            //var result = source.Next(); //Never breaks?
            foreach (var element in result)
            {
                Console.WriteLine("{0:o} {1}", DateTime.Now, element);
                //Thread.Sleep(2000);
            }
            Console.WriteLine("{0:o} done", DateTime.Now);
        }

        //Caches latest value/or default, if no values since last value, will return cached value.
        public void MostRecent_result_has_a_default_value_and_never_blocks()
        {
            Console.WriteLine("{0:o} Starting", DateTime.Now);
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(5);
            var result = source.MostRecent(-1);
            foreach (var element in result)
            {
                Console.WriteLine("{0:o} {1}", DateTime.Now, element);
                //Thread.Sleep(2000);
            }
            Console.WriteLine("{0:o} done", DateTime.Now);
        }

        //Will block until the source notifies. Can miss the OnError or OnCompleted
        public void Next_result_will_block_on_MoveNext_until_source_notifies()
        {
            Console.WriteLine("{0:o} Starting", DateTime.Now);
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(4);
            var result = source.Next();
            foreach (var element in result)
            {
                Console.WriteLine("{0:o} {1}", DateTime.Now, element);
                Thread.Sleep(2000);
                //Misses 4 and the OnCompleted
            }
            //Never gets called.
            Console.WriteLine("{0:o} done", DateTime.Now);
            /*
            Output:
            2012-01-01T12:00:00.0000000 Starting
            2012-01-01T12:00:01.1000000 1
            2012-01-01T12:00:03.1000000 3
            */
        }

        public void Next_can_lose_Error()
        {
            Console.WriteLine("{0:o} Starting", DateTime.Now);
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(4)
                .Concat(Observable.Throw<long>(new Exception()));
            var result = source.Next();

            try
            {
                foreach (var element in result)
                {
                    Console.WriteLine("{0:o} {1}", DateTime.Now, element);
                    Thread.Sleep(2000);
                    //Misses 4 and the OnError
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Never gets called.
            Console.WriteLine("{0:o} done", DateTime.Now);
            /*
            Output:
            2012-01-01T12:00:00.0000000 Starting
            2012-01-01T12:00:01.1000000 1
            2012-01-01T12:00:03.1000000 3
            */
        }

        #endregion

        #region ToTask
        public void ToTask_returns_the_last_value_on_completion()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(5);
            var result = source.ToTask();
            //Wil arrive in 5 seconds.
            Console.WriteLine(result.Result);
        }

        public void ToTask_OnError()
        {
            var source = Observable.Throw<long>(new Exception("Fail!"));
            var result = source.ToTask();
            try
            {
                Console.WriteLine(result.Result);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }
        #endregion

        #region ToEvent
        public void ToEvent()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(5);
            var result = source.ToEvent();
            result.OnNext +=
                val => Console.WriteLine(val);
        }
        public void ToEvent_OnError()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(1)
                .Concat(Observable.Throw<long>(new Exception("Fail!")));

            try
            {
                var result = source.ToEvent();
                try
                {
                    result.OnNext +=
                        val => Console.WriteLine(val);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class MyEventArgs : EventArgs
        {
            private readonly long _value;

            public MyEventArgs(long value)
            {
                _value = value;
            }

            public long Value
            {
                get { return _value; }
            }
        }
        public void ToEventPattern()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(i => new EventPattern<MyEventArgs>(this, new MyEventArgs(i)))
                .Take(5);
            var result = source
                .ToEventPattern<MyEventArgs>();
            result.OnNext += (sender, eventArgs) 
                => Console.WriteLine(eventArgs.Value);
        }

        #endregion

        public void WhatElse()
        {
            var source = new Subject<int>();

            //source.ToEnumerable();
            //source.ToArray();
            //source.ToList();
            //source.ToDictionary();
            //source.ToLookup(); //More than one value per key ie IEnumerable<TValue> not just TValue.

            //source.Latest();
            //source.MostRecent();
            //source.Next();

            //source.ToTask();

            //source.ToEvent().OnNext //returns an IEventSource<T> which has the single OnNext event.
        }
    }
}
// ReSharper restore InconsistentNaming