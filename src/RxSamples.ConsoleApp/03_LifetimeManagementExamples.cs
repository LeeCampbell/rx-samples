using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
// ReSharper disable InconsistentNaming
namespace RxSamples.ConsoleApp
{
  class LifetimeManagementExamples : ExamplesBase
  {
    public void Dispose_of_a_subscription_to_unsubscribe()
    {
      var interval = Observable.Interval(TimeSpan.FromMilliseconds(500));
      var firstSubscription = interval.Subscribe(value=>Console.WriteLine("1st subscription recieved value {0}", value));
      var secondSubscription = interval.Subscribe(value => Console.WriteLine("2nd subscription recieved value {0}", value));
      Thread.Sleep(2000);
      firstSubscription.Dispose();
      using (new ConsoleColor(System.ConsoleColor.DarkGray))
      {
        Console.WriteLine("1st subscription disposed/unsubscribed");
        Console.WriteLine("Press any key to dispose of 2nd subscription");
      }
      Console.ReadKey();

      secondSubscription.Dispose();
      using (new ConsoleColor(System.ConsoleColor.DarkGray))
      {
        Console.WriteLine("2nd subscription disposed/unsubscribed");
      }
    }

    public void Multiple_subscriptions_to_the_same_interval_are_actually_independent_streams()
    {
      var interval = Observable.Interval(TimeSpan.FromMilliseconds(500));
      var firstSubscription = interval.Subscribe(value => Console.WriteLine("1st subscription recieved value {0}", value));
      Thread.Sleep(600);
      var secondSubscription = interval.Subscribe(value => Console.WriteLine("2nd subscription recieved value {0}", value));
      Thread.Sleep(2000);
      firstSubscription.Dispose();
      using (new ConsoleColor(System.ConsoleColor.DarkGray))
      {
        Console.WriteLine("1st subscription disposed/unsubscribed");
        Console.WriteLine("Press any key to dispose of 2nd subscription");
      }
      Console.ReadKey();

      secondSubscription.Dispose();
      using (new ConsoleColor(System.ConsoleColor.DarkGray))
      {
        Console.WriteLine("2nd subscription disposed/unsubscribed");
      }
    }

    public void OnCompleted_signifies_the_end_of_a_stream_and_OnNext_is_ignored()
    {
      var subject = new Subject<int>();
      WriteStreamToConsole(subject, "subject");
      subject.OnNext(1);
      subject.OnNext(2);
      subject.OnCompleted();
      subject.OnNext(3);//This is ignored.
    }

    public void OnError_signifies_the_end_of_a_stream_and_OnNext_is_ignored()
    {
      var subject = new Subject<int>();
      WriteStreamToConsole(subject, "subject");
      subject.OnNext(1);
      subject.OnNext(2);
      subject.OnError(new Exception("Test exception"));
      subject.OnNext(3);//This is ignored.
    }
  }
}
