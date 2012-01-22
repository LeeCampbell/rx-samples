using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Timers;


// ReSharper disable InconsistentNaming
namespace RxSamples.ConsoleApp
{
    class StaticAndExtensionMethodsExamples : ExamplesBase
    {
        

        

        public void Range_is_like_Create_that_returns_a_range_of_integers()
        {
            var create = Observable.Create<int>(o =>
                                             {
                                                 o.OnNext(10);
                                                 o.OnNext(11);
                                                 o.OnNext(12);
                                                 o.OnNext(13);
                                                 o.OnNext(14);
                                                 return () => { };
                                             });
            WriteStreamToConsole(create, "create");

            var enumRange = Enumerable.Range(10, 5).ToObservable();
            WriteStreamToConsole(enumRange, "enumRange");

            var range = Observable.Range(10, 5);
            WriteStreamToConsole(range, "range");

        }

        /// <summary>
        /// Running this will never stop.
        /// </summary>
        public void Interval_example()
        {

            var interval = Observable.Interval(TimeSpan.FromMilliseconds(250));
            WriteStreamToConsole(interval, "interval");
        }

        /// <summary>
        /// In this example I use the Observable.Interval method to fake ticking prices
        /// </summary>
        public void Interval_for_evenly_spaced_publications()
        {
            var rnd = new Random();
            var lastPrice = 100.0;
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Select(i =>
                {
                    var variation =
                        rnd.NextDouble() - 0.5;
                    lastPrice += variation;
                    return lastPrice;
                });

            WriteStreamToConsole(interval, "interval");
        }

        public void Start_using_Action_overload()
        {
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
            });
            WriteStreamToConsole(start, "start");
        }

        public void Start_using_Func_overload()
        {
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
                return "Published value";
            });
            WriteStreamToConsole(start, "start");
        }

        #region FromEvent samples
        public void FromNotifyPropertyChanged(INotifyPropertyChanged instance)
        {
            var propChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => instance.PropertyChanged += h,
                h => instance.PropertyChanged -= h);
        }
        #endregion

      


       
        public void AsObservable_can_protect_subjects_and_hide_underlying_type()
        {
            var subject = new ReplaySubject<int>();
            subject.OnNext(10);
            subject.OnNext(11);
            subject.OnNext(12);

            var hidden = subject.AsObservable();
            //hidden.OnNext(13);  //Wont compile

            WriteStreamToConsole(hidden, "hidden");
        }

        public void Generate_for_generating_streams_with_lambdas()
        {
            var generated = Observable.Generate(5, i => i < 15, i => i + 3, i => i.ToString());
            WriteStreamToConsole(generated, "generated");

            //You could also use. I find the Generate method signature confusing, and it has changed twice, so maybe it confuses Microsoft too?
            // The Select extension method is the same as the IEnumerable<T> version. It is discussed 
            var generatedByRange = Observable.Range(0, 4)
              .Select(i => (i * 3) + 5);
            WriteStreamToConsole(generatedByRange, "generatedByRange");
        }

        #region Buffering and Timeshifting
        public void BufferWithCount_will_publish_enumerables_when_the_specified_number_of_values_is_published()
        {
            var range = Observable.Range(10, 15);
            range.Buffer(4).Subscribe(
                enumerable =>
                {
                    Console.WriteLine("--Buffered values");
                    enumerable.ToList().ForEach(Console.WriteLine);
                }, () => Console.WriteLine("Completed"));
        }

        public void BufferWithTime_will_publish_enumerables_when_the_specified_timespan_elapses()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            interval.Buffer(TimeSpan.FromSeconds(1)).Subscribe(
                buffer =>
                {
                    Console.WriteLine("--Buffered values");
                    foreach (var value in buffer)
                    {
                        Console.WriteLine(value);
                    }
                }, () => Console.WriteLine("Completed"));

        }

        public void Delay_will_time_shift_the_stream_by_a_given_timespan()
        {
            var range = Observable.Range(10, 15);
            var delay = range.Delay(TimeSpan.FromSeconds(2));
            WriteStreamToConsole(delay, "delay");
        }

        public void Delay_will_time_shift_the_stream_by_a_given_DateTimeOffset()
        {
            var in3Seconds = DateTime.Now.AddSeconds(3);
            var range = Observable.Range(10, 15);
            var delay = range.Delay(in3Seconds);
            WriteStreamToConsole(delay, "delay");
        }

        public void Delay_will_not_time_shift_OnError()
        {
            var error = Observable.Throw<int>(new Exception("Test exception"));
            var delay = error.Delay(TimeSpan.FromSeconds(2));
            WriteStreamToConsole(delay, "delay");

            /* A more advanced version that uses yet to be discussed .concat
             * Note that the first few values are published, but as soon as the OnError 
             * is made in the underlying stream, it is published to the Delayed stream.
             */
            //var stream = Observable.Interval(TimeSpan.FromMilliseconds(100))
            //  .Take(20)
            //  .Concat(Observable.Throw<long>(new Exception("Test exception")))
            //  .Delay(TimeSpan.FromSeconds(1));
            //WriteStreamToConsole(stream, "stream");
        }

        public void Sample_will_only_publish_one_value_per_given_timespan()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            var sample = interval.Sample(TimeSpan.FromSeconds(1));
            WriteStreamToConsole(sample, "sample");
        }

        public void Throttle_wont_publish_values_if_they_are_produced_faster_than_the_throttle_period()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(100));
            var throttle = interval.Throttle(TimeSpan.FromMilliseconds(200));
            WriteStreamToConsole(throttle, "throttle");
        }

        public void Throttle_is_only_useful_for_when_time_between_publications_is_variable()
        {
            var randInterval = Observable.Create<int>(o =>
              {
                  for (int i = 0; i < 100; i++)
                  {
                      o.OnNext(i);
                      Thread.Sleep(i++ % 10 < 5 ? 100 : 300);
                  }
                  return () => { };
              });
            var throttle = randInterval.Throttle(TimeSpan.FromMilliseconds(200));
            WriteStreamToConsole(throttle, "throttle");

            /*This uses a feature called merge that will discussed in a later article*/
            //var interval = Observable.Interval(TimeSpan.FromMilliseconds(300))
            //  .Merge(Observable.Interval(TimeSpan.FromMilliseconds(350)));
            //var throttle = interval.Throttle(TimeSpan.FromMilliseconds(200));
            //WriteStreamToConsole(throttle, "throttle");
        }
        #endregion
    }
}
