using System;
using System.Concurrency;
using System.Diagnostics;
using System.Linq;
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
        public void Testing_with_test_scheduler()
        {
            Console.WriteLine("This test should take less than a second to run.");
            var timer = Stopwatch.StartNew();
            var scheduler = new TestScheduler();
            var interval = Observable
              .Interval(TimeSpan.FromSeconds(1), scheduler)
              .Take(5);

            bool isComplete = false;
            interval
                .Timeout(TimeSpan.FromSeconds(3))
                .Subscribe(Console.WriteLine, () => isComplete = true);

            scheduler.Run();

            while (!isComplete)
            {

            }
            Console.WriteLine("Test completed in {0}.", timer.Elapsed);
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
                    i => Console.WriteLine("This will ever run."),
                    ex => exceptionThrown = true);
            scheduler.Run();
            Assert.IsTrue(exceptionThrown);
        }
    }
}
