using System.Concurrency;
using RxSamples.ConsoleApp.TestingRx;

namespace RxSamples.Testing
{
    public sealed class ImmediateSchedulers : ISchedulerProvider
    {
        public IScheduler CurrentThread { get { return Scheduler.Immediate; } }

        public IScheduler Dispatcher { get { return Scheduler.Immediate; } }

        public IScheduler Immediate { get { return Scheduler.Immediate; } }

        public IScheduler NewThread { get { return Scheduler.Immediate; } }

        public IScheduler ThreadPool { get { return Scheduler.Immediate; } }
    }
}