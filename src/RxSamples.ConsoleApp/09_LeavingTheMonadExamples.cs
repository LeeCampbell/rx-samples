using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace RxSamples.ConsoleApp
{
    class LeavingTheMonadExamples
    {
        public void ForEach_is_Blocking()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(8);
            source.ForEach(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));
            Console.WriteLine("completed @ {0}", DateTime.Now);
        }

        public void Subscribe_is_not_Blocking()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(8);
            source.Subscribe(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));
            Console.WriteLine("after Subscribe @ {0}", DateTime.Now);
        }

        public void ForEach_with_TryCatch()
        {
            var source = Observable.Throw<int>(new Exception("Fail"));
            try
            {
                source.ForEach(Console.WriteLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("errored @ {0} with {1}", DateTime.Now, ex.Message);
            }
            finally
            {
                Console.WriteLine("completed @ {0}", DateTime.Now);    
            }
        }

        public void WhatElse()
        {
            var source = new Subject<int>();

            //source.Latest();
            //source.MostRecent();
            //source.Next();

            //source.ToEnumerable();
            //source.ToArray();
            //source.ToList();
            //source.ToDictionary();
            //source.ToLookup(); //More than one value per key ie IEnumerable<TValue> not just TValue.
            //source.ToTask();

            //source.ToEvent().OnNext //returns an IEventSource<T> which has the single OnNext event.
        }
    }
}
