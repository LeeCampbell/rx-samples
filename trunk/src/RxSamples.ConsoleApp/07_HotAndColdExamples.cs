using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using System.Linq;
using System.Threading;

namespace RxSamples.ConsoleApp
{
    class HotAndColdExamples : ExamplesBase
    {
        public void Subject_is_hot_so_starts_regardless_of_subscription()
        {
            var subject = new Subject<int>();
            Console.WriteLine("publishing 0");
            subject.OnNext(0);
            Console.WriteLine("publishing 1");
            subject.OnNext(1);
            Console.WriteLine("Subscribing");
            subject.Subscribe(i => Console.WriteLine(i)); //Can be refactored to subject.Subscribe(Console.WriteLine)
            Console.WriteLine("publishing 2");
            subject.OnNext(2);
            Console.WriteLine("publishing 3");
            subject.OnNext(3);
            Console.ReadKey();
            /*
             * Output 
             * publishing 0
             * publishing 1
             * Subscribing
             * publishing 2
             * 2
             * publishing 3
             * 3
             */
        }

        public void Interval_is_cold_so_starts_on_subscription_and_does_not_share_stream()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period);
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            /* Ouput:
             first subscription : 0
             first subscription : 1
             second subscription : 0
             first subscription : 2
             second subscription : 1
             first subscription : 3
             second subscription : 2   
             */
        }

        //TODO: Breaking change 1.0.2838.104. What used to be Publish is now Multicast.
        public void Publish_shares_stream_and_Connect_makes_cold_observables_hot()
        {
            var period = TimeSpan.FromSeconds(1);
            //var observable = Observable.Interval(period).Publish();
            var observable = Observable.Interval(period).Multicast(new Subject<long>());        //TODO: Hmmm. The unnesscary new Subject<T> is yuck.
            observable.Connect();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

            /* Ouput:
             first subscription : 0
             first subscription : 1
             second subscription : 1
             first subscription : 2
             second subscription : 2   
             */
        }

        //TODO: Breaking change 1.0.2838.104. Publish --> Multicast.
        public void Connections_can_be_disposed_and_reconnected()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Multicast(new Subject<long>());
            observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Press enter to connect, esc to exit.");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    var connection = observable.Connect();
                    Console.WriteLine("Press any key to dispose of connection.");
                    Console.ReadKey();
                    connection.Dispose();
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    exit = true;
                }
            }
            /* Ouput:
             Press enter to connect, esc to exit.
             Press any key to dispose of connection.
             subscription : 0
             subscription : 1
             subscription : 2
             Press enter to connect, esc to exit.
             Press any key to dispose of connection.
             subscription : 0
             subscription : 1
             subscription : 2
             Press enter to connect, esc to exit.   
             */
        }

        //TODO: Breaking change 1.0.2838.104. Publish --> Multicast.
        public void Connected_publishes_regardless_of_subscribers()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
              .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
              .Multicast(new Subject<long>());
            observable.Connect();
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();

            Console.WriteLine("Press any key to exit.");
            /* Ouput:
             Press any key to subscribe
             Publishing 0
             Publishing 1
             Press any key to unsubscribe.
             Publishing 2
             subscription : 2
             Publishing 3
             subscription : 3
             Press any key to exit.
             Publishing 4
             Publishing 5
             */
        }

        //TODO: Breaking change 1.0.2838.104. Publish --> Multicast.
        public void RefCount_only_publishes_once_subscribed_to()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
              .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
              .Multicast(new Subject<long>())
              .RefCount();
            //observable.Connect(); Use RefCount instead now
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();

            Console.WriteLine("Press any key to exit.");
            /* Ouput:
             Press any key to subscribe
             Press any key to unsubscribe.
             Publishing 0
             subscription : 0
             Publishing 1
             subscription : 1
             Publishing 2
             subscription : 2
             Press any key to exit.
             */
        }

        //TODO: Breaking change 1.0.2838.104. Publish --> Multicast.
        public void RefCount_is_a_shared_stream_and_unsubscribes_to_underlying_when_no_more_subscribers()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
              .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
              .Multicast(new Subject<long>())
              .RefCount();
            //observable.Connect(); Use RefCount instead now
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription1 : {0}", i));

            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription2 = observable.Subscribe(i => Console.WriteLine("subscription2 : {0}", i));

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription2.Dispose();

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();
            /* Ouput:
             Press any key to subscribe
             Press any key to unsubscribe.
             Publishing 0
             subscription : 0
             Publishing 1
             subscription : 1
             Publishing 2
             subscription : 2
             Press any key to exit.
             */
        }

        //TODO: Breaking change 1.0.2838.104. Prune is gone!! Now use Multicast(new AsyncSubject<long>());
        public void Prune_will_subscribe_and_return_the_last_value_like_AsyncSubject()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
              .Take(5)
              .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
              .Multicast(new AsyncSubject<long>());
            observable.Connect();
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();
            /* Ouput:
             Press any key to subscribe
             Publishing 0
             Publishing 1
             Press any key to unsubscribe.
             Publishing 2
             Publishing 3
             Publishing 4
             subscription : 4
             Press any key to exit.
             */
        }

        //TODO: Breaking change 1.0.2838.104. Replay is gone!! Now use Multicast(new ReplaySubject<long>());
        public void Replay_wraps_underlying_in_ReplaySubject()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
              .Take(3)
              .Multicast(new ReplaySubject<long>());
            observable.Connect();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

            Console.ReadKey();
            observable.Subscribe(i => Console.WriteLine("third subscription : {0}", i));
            /* Ouput:
             first subscription : 0
             second subscription : 0
             first subscription : 1
             second subscription : 1
             first subscription : 2
             second subscription : 2   
             third subscription : 0
             third subscription : 1
             third subscription : 2
             */
        }

        //TODO: Breaking change 1.0.2838.104. Replay is gone!! Now use Multicast(new ReplaySubject<long>());
        public void ReplayOnHotExample()
        {
            var period = TimeSpan.FromSeconds(1);
            var hot = Observable.Interval(period)
              .Take(3)
              .Multicast(new Subject<long>());
            hot.Connect();
            Thread.Sleep(period); //Run hot and ensure a value is lost.
            var observable = hot.Multicast(new ReplaySubject<long>());
            observable.Connect();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

            Console.ReadKey();
            observable.Subscribe(i => Console.WriteLine("third subscription : {0}", i));
            Console.ReadKey();

            /* Ouput:
             first subscription : 1
             second subscription : 1
             first subscription : 2
             second subscription : 2   
             third subscription : 1
             third subscription : 2
             */
        }

        public void AnonQuestion20100917()
        {
            var s = Observable.CreateWithDisposable<int>(
            observer =>
            {
                for (int i = 0; i < 100; i++)
                {
                    observer.OnNext(i);
                    Thread.Sleep(300);
                }

                observer.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("--Disposed--"));
            });

            s
                //.SubscribeOn(Scheduler.CurrentThread)     //--Add this in to have the thread sleep happen on the thread pool.
                .Subscribe(Console.WriteLine);

            //MyEnumerable t = new MyEnumerable();
            //t.ForEach(i => Console.WriteLine(i));
            Console.WriteLine("app continues running while async enumerable is processing");
            Console.ReadLine();
        }

        public void AnonQuestion20100917_using_Inteval()
        {
            var s = Observable.Interval(TimeSpan.FromMilliseconds(300))
                .Take(8) //Else it will run forever
                .Finally(() => Console.WriteLine("--Disposed--"));

            s.Subscribe(Console.WriteLine);

            Console.WriteLine("app continues running while async enumerable is processing");
            Console.ReadLine();
        }
    }
}