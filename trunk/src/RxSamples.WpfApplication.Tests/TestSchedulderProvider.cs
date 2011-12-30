using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace RxSamples.WpfApplication.Tests
{
    public class TestSchedulderProvider : ISchedulerProvider
    {
        private readonly TestScheduler _currentThread = new TestScheduler();
        private readonly TestScheduler _dispatcher = new TestScheduler();
        private readonly TestScheduler _immediate = new TestScheduler();
        private readonly TestScheduler _newThread = new TestScheduler();
        private readonly TestScheduler _taskPool = new TestScheduler();
        private readonly TestScheduler _threadPool = new TestScheduler();

        public TestScheduler CurrentThread
        {
            get { return _currentThread; }
        }

        public TestScheduler Dispatcher
        {
            get { return _dispatcher; }
        }

        public TestScheduler Immediate
        {
            get { return _immediate; }
        }

        public TestScheduler NewThread
        {
            get { return _newThread; }
        }

        public TestScheduler TaskPool
        {
            get { return _taskPool; }
        }

        public TestScheduler ThreadPool
        {
            get { return _threadPool; }
        }

        #region Implementation of ISchedulerProvider

        IScheduler ISchedulerProvider.CurrentThread
        {
            get { return CurrentThread; }
        }

        IScheduler ISchedulerProvider.Dispatcher
        {
            get { return Dispatcher; }
        }

        IScheduler ISchedulerProvider.Immediate
        {
            get { return Immediate; }
        }

        IScheduler ISchedulerProvider.NewThread
        {
            get { return NewThread; }
        }

        IScheduler ISchedulerProvider.TaskPool
        {
            get { return TaskPool; }
        }

        IScheduler ISchedulerProvider.ThreadPool
        {
            get { return ThreadPool; }
        }

        #endregion
    }
}