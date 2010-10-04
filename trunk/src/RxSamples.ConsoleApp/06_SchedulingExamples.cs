using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Threading;

namespace RxSamples.ConsoleApp
{
  class SchedulingExamples
  {
    //public void IntroToScheduling()
    //{
    //  Scheduler.CurrentThread;
    //  Scheduler.Dispatcher;
    //  Scheduler.Immediate;
    //  Scheduler.NewThread;
    //  Scheduler.TaskPool;
    //  Scheduler.ThreadPool;
    //}

    private static void ScheduleTasks(IScheduler scheduler)
    {
      Action leafAction = () => Console.WriteLine("leafAction.");
      Action innerAction = () =>
                             {
                               Console.WriteLine("innerAction start.");
                               scheduler.Schedule(leafAction);
                               Console.WriteLine("innerAction end.");
                             };
      Action outerAction = () =>
                             {
                               Console.WriteLine("outer start.");
                               scheduler.Schedule(innerAction);
                               Console.WriteLine("outer end.");
                             };
      scheduler.Schedule(outerAction);
    }

    public void Scheduling_on_Immediate_is_sequential()
    {
      ScheduleTasks(Scheduler.Immediate);
      Console.ReadLine();
      /*Output:
       * outer start.
       * outer end.
       * innerAction start.
       * innerAction end.
       * leafAction.
       */
    }

    public void Scheduling_on_the_CurrentThread_is_dispatched_to_a_Trampoline()
    {
      ScheduleTasks(Scheduler.CurrentThread);
      Console.ReadLine();
      /*Output:
       * outer start.
       * outer end.
       * innerAction start.
       * innerAction end.
       * leafAction.
       */
    }

    public void DefaultSchedulingOnObservableCreate()
    {
      var stream = Observable.Create<int>(
        o =>
          {
            Console.WriteLine("Subscription on thread{0}", Thread.CurrentThread.ManagedThreadId);
            o.OnNext(1);
            o.OnNext(2);
            o.OnNext(3);
            return () => { };
          });

      Console.WriteLine("Main thread {0}", Thread.CurrentThread.ManagedThreadId);
      stream
        .SubscribeOn(Scheduler.NewThread)
        .ObserveOn(Scheduler.NewThread)
        .Subscribe(
          i => Console.WriteLine("Value={0} on thread:{1}", i, Thread.CurrentThread.ManagedThreadId),
          () => Console.WriteLine("Oncomplete on thread:{0}", Thread.CurrentThread.ManagedThreadId));
    }

    /// <summary>
    /// Use this method to see how Scheduling is performed. The sheduler and thread will written to the console on subscription and publication to the Observer.
    /// </summary>
    /// <param name="subscribeOnScheduler"></param>
    /// <param name="observeOnScheduler"></param>
    public void SchedulingOnObservableCreate(IScheduler subscribeOnScheduler, IScheduler observeOnScheduler)
    {
      Console.WriteLine("Subscribing on {0} and Observing on {1}", subscribeOnScheduler.GetType().Name, observeOnScheduler.GetType().Name);
      var stream = Observable.Create<int>(
        o =>
          {
            Console.WriteLine("Subscription on thread{0}", Thread.CurrentThread.ManagedThreadId);
            o.OnNext(1);
            o.OnNext(2);
            o.OnNext(3);
            return () => { };
          });

      Console.WriteLine("Main thread {0}", Thread.CurrentThread.ManagedThreadId);
      stream
        .SubscribeOn(subscribeOnScheduler)
        .ObserveOn(observeOnScheduler)
        .Subscribe(
          i => Console.WriteLine("Value={0} on thread:{1}", i, Thread.CurrentThread.ManagedThreadId),
          () => Console.WriteLine("Oncomplete on thread:{0}", Thread.CurrentThread.ManagedThreadId));
    }

    public void Rx_can_still_Deadlock()
    {
      var stream = new Subject<int>();
      Console.WriteLine("Next line should dead lock the system.");
      var value = stream.First();
      stream.OnNext(1);
      Console.WriteLine("I can never execute....");
    }
  }
}