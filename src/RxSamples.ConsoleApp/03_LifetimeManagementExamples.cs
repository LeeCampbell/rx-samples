using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

// ReSharper disable InconsistentNaming
namespace RxSamples.ConsoleApp
{
    class LifetimeManagementExamples : ExamplesBase
    {
        public void Dispose_of_a_subscription_to_unsubscribe()
        {
            var values = new Subject<int>();
            var firstSubscription = values.Subscribe(value => Console.WriteLine("1st subscription recieved {0}", value));
            var secondSubscription = values.Subscribe(value => Console.WriteLine("2nd subscription recieved {0}", value));
            values.OnNext(0);
            values.OnNext(1);
            values.OnNext(2);
            values.OnNext(3);
            firstSubscription.Dispose();
            Console.WriteLine("Disposed of 1st subscription");
            values.OnNext(4);
            values.OnNext(5);
            secondSubscription.Dispose();
        }

        public void SEH_is_not_valid_for_Rx()
        {
            var values = new Subject<int>();
            try
            {
                values.Subscribe(value => Console.WriteLine("1st subscription recieved {0}", value));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wont catch anything here!");
            }
            
            values.OnNext(0);
            //Exception will be thrown here.
            values.OnError(new Exception("Dummy exception"));
        }

        public void Handling_OnError()
        {
            var values = new Subject<int>();
            
            values.Subscribe(
                value => Console.WriteLine("1st subscription recieved {0}", value),
                ex=>Console.WriteLine("Caught an exception : {0}", ex));

            values.OnNext(0);
            values.OnError(new Exception("Dummy exception"));
        }

        public void No_GC_of_subscriptions()
        {
            var values = new Subject<int>();

            values.Subscribe(
                value => Console.WriteLine("1st subscription recieved {0}", value),
                ex => Console.WriteLine("Caught an exception : {0}", ex));

            values.OnNext(0);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            values.OnNext(1);
        }


        public void Dispose_of_a_subscription_to_unsubscribe_old()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(500));
            var firstSubscription = interval.Subscribe(value => Console.WriteLine("1st subscription recieved value {0}", value));
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
