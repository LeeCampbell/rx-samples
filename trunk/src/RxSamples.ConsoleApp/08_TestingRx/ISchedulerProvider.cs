using System.Concurrency;

namespace RxSamples.ConsoleApp.TestingRx
{
    public interface ISchedulerProvider  //Implementation would just be a wrapper around Scheduler and the static properties it exposes.
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
        //IScheduler TaskPool { get; }
    }
}