using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
// ReSharper disable InconsistentNaming
namespace RxSamples.ConsoleApp
{
  class StaticAndExtensionMethodsExamples : ExamplesBase
  {
    public void Empty_is_like_an_empty_completed_ReplaySubject()
    {
      var subject = new ReplaySubject<string>();
      subject.OnCompleted();
      WriteStreamToConsole(subject, "ReplaySubject");

      //Or
      var empty = Observable.Empty<string>();
      WriteStreamToConsole(empty, "empty");
    }

    public void Return_is_like_a_completed_BehaviourSubject()
    {
      var subject = new ReplaySubject<string>();
      subject.OnNext("Value");
      subject.OnCompleted();
      WriteStreamToConsole(subject, "subject");
      //Or
      var obReturn = Observable.Return("Value");
      WriteStreamToConsole(obReturn, "Observable.Return");
    }

    public void Never_is_like_a_Subject()
    {
      var subject = new Subject<string>();
      WriteStreamToConsole(subject, "subject");
      //Or
      var never = Observable.Never<string>();
      WriteStreamToConsole(never, "never");
    }

    public void Throw_is_like_ReplaySubject_that_throws_when_subsribed_to()
    {
      var subject = new ReplaySubject<string>();
      subject.OnError(new Exception());
      WriteStreamToConsole(subject, "subject");
      //Or
      var throws = Observable.Throw<string>(new Exception());
      WriteStreamToConsole(throws, "throws");
      //See the next few methods regarding Observable.Create for a more realistic way of reproducing.
    }

    #region Observable Create. Compare blocking vs Non-blocking
    private IObservable<string> BlockingMethod()
    {
      var subject = new ReplaySubject<string>();
      subject.OnNext("a");
      subject.OnNext("b");
      subject.OnCompleted();
      Thread.Sleep(1000);
      return subject;
    }
    private IObservable<string> NonBlocking()
    {
      return Observable.Create<string>(
          observable =>
          {
            observable.OnNext("a");
            observable.OnNext("b");
            observable.OnCompleted();
            Thread.Sleep(1000);
            return () => Console.WriteLine("Observer has unsubscribed");
          });
    }
    public void Blocking_vs_NonBlocking_via_Observable_Create()
    {
      Console.WriteLine("Requesting observable via blocking call @ {0:o}", DateTime.Now);
      var blocked = BlockingMethod();
      Console.WriteLine("Recieved observable via blocking call @ {0:o}", DateTime.Now);
      WriteStreamToConsole(blocked, "blocked");

      Console.WriteLine("Requesting observable via nonblocking call @ {0:o}", DateTime.Now);
      var nonblocked = NonBlocking();
      Console.WriteLine("Recieved observable via nonblocking call @ {0:o}", DateTime.Now);
      WriteStreamToConsole(nonblocked, "nonblocked");

    }

    public void Defer_to_make_a_blocking_non_blocking_like_Observable_Create()
    {
      Console.WriteLine("Requesting observable via blocking call @ {0:o}", DateTime.Now);
      var blocked = BlockingMethod();
      Console.WriteLine("Recieved observable via blocking call @ {0:o}", DateTime.Now);
      WriteStreamToConsole(blocked, "blocked");

      Console.WriteLine("Requesting observable via defer call @ {0:o}", DateTime.Now);
      var defer = Observable.Defer<string>(BlockingMethod);
      Console.WriteLine("Recieved observable via defer call @ {0:o}", DateTime.Now);
      WriteStreamToConsole(defer, "defer");

    }
    #endregion

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
    public void Inveral_example()
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

    public void ToObservable_can_convert_IEnumables()
    {
      var enumT = new List<string>();
      enumT.Add("a");
      enumT.Add("b");
      enumT.Add("c");
      var fromEnum = enumT.ToObservable();

      WriteStreamToConsole(fromEnum, "fromEnum");
    }
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
      var generated = Observable.Generate(5, i => i < 15, i => i.ToString(), i => i + 3);
      WriteStreamToConsole(generated, "generated");

      //You could also use. I find the Generate method signature confusing, and it has changed twice, so maybe it confuses Microsoft too?
      // The Select extension method is the same as the IEnumerable<T> version. It is discussed 
      var generatedByRange = Observable.Range(0, 4)
        .Select(i => (i * 3) + 5);
      WriteStreamToConsole(generatedByRange, "generatedByRange");
    }

    #region Existence checks

    public void Any_checks_for_existence_and_returns_Observable_of_bool()
    {
      var range = Observable.Range(10, 15);
      range.Any().Subscribe(Console.WriteLine);
      range.Any(i => i > 100).Subscribe(Console.WriteLine);
      Observable.Empty<int>().Any().Subscribe(Console.WriteLine);
    }

    public void Any_will_publish_true_once_its_predicate_is_true()
    {
      //This will return true when the Interval has published the value 4 (in 4 seconds).
      Observable.Interval(TimeSpan.FromSeconds(1)).Any(i => i > 3).Subscribe(Console.WriteLine);
    }

    public void Any_will_publish_false_once_completed_is_published_and_no_values_meets_the_predicate()
    {
      //This will return false when the Interval completes in 3 seconds (it wont produce a value over 3).
      Observable.Interval(TimeSpan.FromSeconds(1))
        .Take(2)
        .Any(i => i > 3).Subscribe(Console.WriteLine);
    }

    public void Contains_is_a_more_specific_version_of_any()
    {
      var range = Observable.Range(10, 15);
      range.Contains(15).Subscribe(Console.WriteLine);
      Observable.Empty<int>().Contains(5).Subscribe(Console.WriteLine);
    }

    public void IsEmpty_returns_once_a_value_or_OnComplete_is_published()
    {
      var range = Observable.Range(10, 15);
      range.IsEmpty().Subscribe(Console.WriteLine);
      var subject = new ReplaySubject<int>();
      subject.OnNext(1);
      subject.IsEmpty().Subscribe(Console.WriteLine);
      Observable.Empty<int>().IsEmpty().Subscribe(Console.WriteLine);
    }

    public void Contains_Any_and_IsEmpty_rethrow_errors()
    {
      var error = Observable.Throw<int>(new Exception("Example exceptions"));
      WriteStreamToConsole(error.Any(i => i > 3), "Any rethrows Error");
      WriteStreamToConsole(error.Contains(3), "Contains rethrows Error");
      WriteStreamToConsole(error.IsEmpty(), "IsEmpty rethrows Error");
    }
    #endregion

    #region Filtereing and Aggregating

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

    #region Scan and Aggregate
    public void Scan_allows_custom_aggregation()
    {
      var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
      var scan = interval.Scan(new Averages(), (acc, i) => new Averages(acc, i));
      WriteStreamToConsole(scan, "scan");
    }

    /// <summary>
    /// I am not sure what the practical difference is between Aggregate and Scan.
    /// </summary>
    public void Aggergate_can_be_used_to_create_your_own_aggregates()
    {
      var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
      var aggregate = interval.Aggregate(new Averages(), (acc, i) => new Averages(acc, i));
      WriteStreamToConsole(aggregate, "aggregate");
    }
    private class Averages
    {
      public Averages()
      {
        CurrentValue = 0;
        LastValues = new List<long>();
      }
      public Averages(Averages accumulator, long currentValue)
      {
        CurrentValue = currentValue;
        LastValues = new List<long>(accumulator.LastValues);
        if (LastValues.Count > 5)
        {
          LastValues.RemoveAt(0);
        }
        LastValues.Add(accumulator.CurrentValue);
      }
      public long CurrentValue { get; private set; }
      public double TrailingAverage()
      {
        return LastValues.Average();
      }
      public List<long> LastValues { get; private set; }
      public override string ToString()
      {
        return string.Format("{0} (Avg of last 5 = {1})", CurrentValue, TrailingAverage());
      }
    }
    #endregion

    public void Take_will_complete_the_stream_after_the_given_number_of_publications()
    {
      var range = Observable.Range(10, 5);
      var take2 = range.Take(2);
      WriteStreamToConsole(range, "range");
      WriteStreamToConsole(take2, "take2");
    }

    public void Skip_will_ignore_the_first_number_of_publications()
    {
      var range = Observable.Range(10, 5);
      var skip2 = range.Skip(2);
      WriteStreamToConsole(range, "range");
      WriteStreamToConsole(skip2, "skip2");
    }

    public void DistinctUntilChanged_will_only_publish_values_that_are_different_to_the_last_published_value()
    {
      var subject = new Subject<string>();
      var distinct = subject.DistinctUntilChanged();
      WriteStreamToConsole(subject, "subject");
      WriteStreamToConsole(distinct, "distinct");
      subject.OnNext("a");
      subject.OnNext("a");
      subject.OnNext("a");
      subject.OnNext("b");
      subject.OnNext("b");
      subject.OnNext("b");
      subject.OnNext("b");
      subject.OnNext("c");
      subject.OnNext("c");
      subject.OnNext("c");
    }
    
    #endregion

    #region Buffering and Timeshifting
    public void BufferWithCount_will_publish_enumerables_when_the_specified_number_of_values_is_published()
    {
      var range = Observable.Range(10, 15);
      range.BufferWithCount(4).Subscribe(
          enumerable =>
          {
            Console.WriteLine("--Buffered values");
            enumerable.ToList().ForEach(Console.WriteLine);
          }, () => Console.WriteLine("Completed"));
    }

    public void BufferWithTime_will_publish_enumerables_when_the_specified_timespan_elapses()
    {
      var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
      interval.BufferWithTime(TimeSpan.FromSeconds(1)).Subscribe(
          enumerable =>
          {
            Console.WriteLine("--Buffered values");
            enumerable.ToList().ForEach(Console.WriteLine);
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
