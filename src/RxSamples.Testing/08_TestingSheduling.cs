using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RxSamples.Testing
{
    [TestClass]
    public class TestingSheduling
    {
        [TestMethod]
        public void Testing_with_real_scheduler()
        {
            Console.WriteLine("This test will take 5 seconds to run.");
            var timer = Stopwatch.StartNew();
            var interval = Observable
              .Interval(TimeSpan.FromSeconds(1))
              .Take(5);

            bool isComplete = false;
            interval
                .Timeout(TimeSpan.FromSeconds(3))
                .Subscribe(Console.WriteLine, () => isComplete = true);

            while (!isComplete)
            {

            }
            Console.WriteLine("Test completed in {0}.", timer.Elapsed);
        }

        [TestMethod]
        public void Testing_with_real_scheduler2()
        {
            Console.WriteLine("This test will take 5 seconds to run.");
            var timer = Stopwatch.StartNew();
            var interval = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Take(5);

            interval
                .Timeout(TimeSpan.FromSeconds(3))
                .ForEach(Console.WriteLine);

            Console.WriteLine("Test completed in {0}.", timer.Elapsed);
        }

        [TestMethod]
        public void Scheduling_with_the_TestScheduler()
        {
            var scheduler = new TestScheduler();
            var wasExecuted = false;

            scheduler.Schedule(() => wasExecuted = true);   //no offset or duetime specified so it should be marshalled for immediate.
            Assert.IsFalse(wasExecuted);
            scheduler.AdvanceBy(1);         //executing one tick of queued actions is effectively the same as executing the first action on the scheduler.
            Assert.IsTrue(wasExecuted);
        }

        [TestMethod]
        public void Cancelling_scheduled_action_with_the_TestScheduler()
        {
            var scheduler = new TestScheduler();
            var wasExecuted = false;

            var token = scheduler.Schedule(() => wasExecuted = true);   //no offset or duetime specified so it should be marshalled for immediate.
            Assert.IsFalse(wasExecuted);
            token.Dispose();
            scheduler.AdvanceBy(1);     //executing one tick of queued actions is effectively the same as executing the first action on the scheduler.
            Assert.IsFalse(wasExecuted);
        }

        [TestMethod]
        public void Scheduling_at_duplicate_points_in_time_with_the_TestScheduler()
        {
            var scheduler = new TestScheduler();
            long dueTime = 4L;

            TimeSpan.FromTicks(dueTime);

            scheduler.Schedule(() => Console.WriteLine("1"), TimeSpan.FromTicks(dueTime));
            scheduler.Schedule(() => Console.WriteLine("2"), TimeSpan.FromTicks(dueTime));
            scheduler.Schedule(() => Console.WriteLine("3"), TimeSpan.FromTicks(dueTime + 1));
            scheduler.Schedule(() => Console.WriteLine("4"), TimeSpan.FromTicks(dueTime + 1));
            Console.WriteLine("AdvanceTo(dueTime)");
            scheduler.AdvanceTo(dueTime);
            Console.WriteLine("Start()");
            scheduler.Start();
            /* Output:
            RunTo(dueTime)
            1
            2
            Run()
            3
            4
            */
        }

        [TestMethod]
        public void Testing_with_test_scheduler()
        {
            Console.WriteLine("This test should take less than a second to run.");
            var timer = Stopwatch.StartNew();
            var scheduler = new TestScheduler();
            var interval = Observable
                .Interval(TimeSpan.FromSeconds(1), scheduler)
                .Take(5);

            bool isComplete = false;
            interval.Subscribe(Console.WriteLine, () => isComplete = true);

            scheduler.Start();

            Assert.IsTrue(isComplete);
            Console.WriteLine("Test completed in {0}.", timer.Elapsed);
        }

        [TestMethod]
        public void Testing_timeouts_badly()
        {
            var stream = Observable.Never<int>();
            var exceptionThrown = false;
            try
            {
                stream.Timeout(TimeSpan.FromSeconds(5))
                    .ForEach(i => Console.WriteLine("This will never run."));
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
        [TestMethod]
        public void Testing_timeouts_with_the_TestScheduler()
        {
            var scheduler = new TestScheduler();
            var stream = Observable.Never<int>();

            var exceptionThrown = false;
            //stream.Timeout(TimeSpan.FromMinutes(1))//If no scheduler is passed then Scheduler.ThreadPool is used as the default
            stream.Timeout(TimeSpan.FromMinutes(1), scheduler)
                .Subscribe(
                    i => Console.WriteLine("This will never run."),
                    ex => exceptionThrown = true);

            scheduler.Start();
            Assert.IsTrue(exceptionThrown);
        }


        [TestMethod]
        public void Example_for_sunghyun7()
        {
            //http://social.msdn.microsoft.com/Forums/en/rx/thread/e5173644-69f5-462c-b8c6-aee21277049f

            var i = 0;
            var testScheduler = new TestScheduler();
            testScheduler.Schedule(() => i = 1);    //Schedule immediately
            testScheduler.Schedule(() => i = 2, TimeSpan.FromSeconds(10));    //Schedule in 10s
            testScheduler.Schedule(() => i = 3, TimeSpan.FromMinutes(30));    //Schedule in 30min

            //Note that the new api is leaky and I am pretty sure this is a bug.
            //Users have to know that it uses ticks internally. ie pass in 1 or a Ticks value.
            //Give us back access to TestScheduler.ToRelative(TimeSpan) please!
            Assert.AreEqual(0, i);
            testScheduler.AdvanceTo(1);
            Assert.AreEqual(1, i);
            testScheduler.AdvanceTo(TimeSpan.FromSeconds(10).Ticks);
            Assert.AreEqual(2, i);
            testScheduler.AdvanceTo(TimeSpan.FromMinutes(30).Ticks);
            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void RxForum_Beats_21_nov_2011()
        {
            //http://social.msdn.microsoft.com/Forums/en/rx/thread/e5173644-69f5-462c-b8c6-aee21277049f

            var sched = new TestScheduler();
            var input = sched.CreateColdObservable(
                new Recorded<Notification<int>>(1, Notification.CreateOnNext(205)),
                new Recorded<Notification<int>>(10, Notification.CreateOnNext(305)),
                new Recorded<Notification<int>>(100, Notification.CreateOnNext(405)),
                new Recorded<Notification<int>>(1000, Notification.CreateOnNext(505)),
                new Recorded<Notification<int>>(10000, Notification.CreateOnNext(605)),
                new Recorded<Notification<int>>(11000, Notification.CreateOnCompleted<int>())
                );

            int i = 0;

            var windows = input.Window(
                // We're going to start a window every 100 milliseconds..
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), sched)
                            .Take(7),
                // ..and then close it 50ms later.
                x => Observable.Timer(TimeSpan.FromMilliseconds(50), sched));

            windows.Timestamp(sched).Subscribe(obs =>
            {
                int current = ++i;
                Console.WriteLine("Started Observable {0} at {1}ms", current, obs.Timestamp.Millisecond);
                // Subscribe to the inner Observable and print its items
                obs.Value.Subscribe(
                item => Console.WriteLine(" {0} at {1}ms", item, sched.Now.Millisecond),
                () => Console.WriteLine("Ended Observable {0} at {1}ms\n",
                current, sched.Now.Millisecond));
            });

            sched.Start();
            /*
            Started Observable 1 at 0ms
                205 at 0ms
                305 at 0ms
                405 at 0ms
                505 at 0ms
                605 at 1ms
            Ended Observable 1 at 50ms

            Started Observable 2 at 100ms
            Ended Observable 2 at 150ms

            Started Observable 3 at 200ms
            Ended Observable 3 at 250ms

            Started Observable 4 at 300ms
            Ended Observable 4 at 350ms

            Started Observable 5 at 400ms
            Ended Observable 5 at 450ms

            Started Observable 6 at 500ms
            Ended Observable 6 at 550ms

            Started Observable 7 at 600ms
            Ended Observable 7 at 650ms


            1 passed, 0 failed, 0 skipped, took 0.78 seconds (MSTest 10.0).
             */
        }

    }



}
