using System.Concurrency;
using RxSamples.ConsoleApp.TestingRx;

namespace RxSamples.Testing
{
    public sealed class TestSchedulers : ISchedulerService
    {
        private readonly TestScheduler _currentThread = new TestScheduler();
        private readonly TestScheduler _dispatcher = new TestScheduler();
        private readonly TestScheduler _immediate = new TestScheduler();
        private readonly TestScheduler _newThread = new TestScheduler();
        private readonly TestScheduler _threadPool = new TestScheduler();

        #region Implementation of ISchedulerService
        IScheduler ISchedulerService.CurrentThread { get { return _currentThread; } }

        IScheduler ISchedulerService.Dispatcher { get { return _dispatcher; } }

        IScheduler ISchedulerService.Immediate { get { return _immediate; } }

        IScheduler ISchedulerService.NewThread { get { return _newThread; } }

        IScheduler ISchedulerService.ThreadPool { get { return _threadPool; } }
        #endregion

        public TestScheduler CurrentThread { get { return _currentThread; } }

        public TestScheduler Dispatcher { get { return _dispatcher; } }

        public TestScheduler Immediate { get { return _immediate; } }

        public TestScheduler NewThread { get { return _newThread; } }

        public TestScheduler ThreadPool { get { return _threadPool; } }
    }
}