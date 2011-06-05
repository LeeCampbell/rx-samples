using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace RxSamples.Testing
{
  public static class TestSchedulerExtensions
  {
    ///// <summary>
    ///// Runs the scheduler from now to the given TimeSpan. Advances relative to it's <c>Now</c> value.
    ///// </summary>
    ///// <param name="scheduler"></param>
    ///// <param name="interval"></param>
    //public static void RunNext(this TestScheduler scheduler, TimeSpan interval)
    //{
    //  var tickInterval = scheduler.FromTimeSpan(interval);
    //  scheduler.RunTo(scheduler.Ticks + tickInterval + 1);
    //}

    //public static void RunTo(this TestScheduler scheduler, TimeSpan interval)
    //{
    //  var tickInterval = scheduler.FromTimeSpan(interval);
    //  scheduler.RunTo(tickInterval);
    //}

    //public static void Step(this TestScheduler scheduler)
    //{
    //  scheduler.RunTo(scheduler.Ticks + 1);
    //}

    /// <summary>
    /// Allow me to call Schedule(Action) like I used to :)
    /// </summary>
    public static IDisposable Schedule(this IScheduler scheduler, Action action)
    {
      return scheduler.Schedule(action, TimeSpan.Zero);
    }

    public static IDisposable Schedule(this IScheduler scheduler, Action action, TimeSpan dueTime)
    {

      return scheduler.Schedule(Unit.Default, dueTime, (s, _) =>
                                                         {
                                                           //var token = new BooleanDisposable();
                                                           //if(!token.IsDisposed)
                                                           //{
                                                           action();
                                                           //}
                                                           //return token;
                                                           return Disposable.Create(() =>
                                                                                      {
                                                                                        //When does this actually get called?
                                                                                        Console.WriteLine("Action is beeing disposed");
                                                                                      });
                                                         });
    }

    /// <summary>
    /// Provides a fluent interface so that you can write<c>7.Seconds()</c> instead of <c>TimeSpan.FromSeconds(7)</c>.
    /// </summary>
    /// <param name="seconds">A number of seconds</param>
    /// <returns>Returns a System.TimeSpan to represents the specified number of seconds.</returns>
    public static TimeSpan Seconds(this int seconds)
    {
      return TimeSpan.FromSeconds(seconds);
    }
  }
}