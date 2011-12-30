using System.Reactive.Concurrency;

namespace RxSamples.WpfApplication
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler TaskPool { get; }
        IScheduler ThreadPool { get; }
    }

    public sealed class SchedulerProvider : ISchedulerProvider
    {
        #region Implementation of ISchedulerProvider

        public IScheduler CurrentThread
        {
            get { return Scheduler.CurrentThread; }
        }

        public IScheduler Dispatcher
        {
            //get { return Scheduler.Dispatcher; }
            get { return DispatcherScheduler.Instance; }
        }

        public IScheduler Immediate
        {
            get { return Scheduler.Immediate; }
        }

        public IScheduler NewThread
        {
            get { return Scheduler.NewThread; }
        }

        public IScheduler TaskPool
        {
            get { return Scheduler.TaskPool; }
        }

        public IScheduler ThreadPool
        {
            get { return Scheduler.ThreadPool; }
        }

        #endregion
    }
}