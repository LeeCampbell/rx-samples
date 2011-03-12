using System;
using System.Concurrency;

namespace RxSamples.Testing
{
    public static class TestSchedulerExtensions
    {
        /// <summary>
        /// Runs the scheduler from now to the given TimeSpan.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="interval"></param>
        public static void RunNext(this TestScheduler scheduler, TimeSpan interval)
        {
            var tickInterval = scheduler.FromTimeSpan(interval);
            scheduler.RunTo(scheduler.Ticks + tickInterval + 1);
        }

        public static void RunTo(this TestScheduler scheduler, TimeSpan interval)
        {
            var tickInterval = scheduler.FromTimeSpan(interval);
            scheduler.RunTo(tickInterval);
        }

        public static void Step(this TestScheduler scheduler)
        {
            scheduler.RunTo(scheduler.Ticks + 1);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }
    }
}