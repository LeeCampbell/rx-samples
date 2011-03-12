using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RxSamples.ConsoleApp;
using RxSamples.ConsoleApp.TestingRx;

namespace RxSamples.Testing
{
    [TestClass]
    public class _09_JoinGroupWindowTests
    {
        //[TestClass]
        //public class Given_a_known_stream_of_values
        //{
        //    [TestClass]
        //    public class When_VWAP_method_is_applied_to_stream
        //    {
        //        [TestMethod]
        //        public void Should_start_first_window_on_subscription()
        //        {
        //            Assert.Fail();
        //        }

        //        [TestMethod]
        //        public void Should_start_window_when_value_is_published()
        //        {
        //            Assert.Fail();
        //        }
        //    }   
        //}

        [TestMethod]
        public void VWAP_Should_return_average_price_for_overlapping_windows()
        {
            /*
            -->Actual values
            |---------|---------|---------|---------|---------|---------|---------|---------|---------|-->
            0--1---2--3---4----------------------------------------------------------------------------
            -->Expected values
            |---------1--2---3--3---4------------------------------------------------------------------
            |---------.---------.----------------------------------------------------------------------
            |---------5---------5----------------------------------------------------------------------
            */
            var values = new List<KeyValuePair<decimal, DateTimeOffset>>();
            var scheduler = new TestScheduler();
            var input = new Subject<decimal>();
            input.VWAP(i => i, 10.Seconds(), 1.Seconds(), scheduler)
                .Subscribe(avg => values.Add(new KeyValuePair<decimal, DateTimeOffset>(avg, scheduler.Now)));
            scheduler.Schedule(() => input.OnNext(0));
            scheduler.Schedule(() => input.OnNext(1), 3.Seconds());
            scheduler.Schedule(() => input.OnNext(2), 7.Seconds());
            scheduler.Schedule(() => input.OnNext(3), 10.Seconds());
            scheduler.Schedule(() => input.OnNext(4), 14.Seconds());
            
            scheduler.RunTo(30.Seconds());
            

            values.Run(kvp=>Console.WriteLine("{0} @ {1}", kvp.Key, kvp.Value));

            //1.5 @ 01/01/0001 00:00:10 +00:00
            //2   @ 01/01/0001 00:00:13 +00:00
            //3   @ 01/01/0001 00:00:17 +00:00
            //3.5 @ 01/01/0001 00:00:20 +00:00
            //4   @ 01/01/0001 00:00:24 +00:00

            Assert.AreEqual(1.5m, values[0].Key);
            Assert.AreEqual(10, values[0].Value.Second);
            
            Assert.AreEqual(2m, values[1].Key);
            Assert.AreEqual(13, values[1].Value.Second);
            
            Assert.AreEqual(3m, values[2].Key);
            Assert.AreEqual(17, values[2].Value.Second);

            Assert.AreEqual(3.5m, values[3].Key);
            Assert.AreEqual(20, values[3].Value.Second);

            Assert.AreEqual(4m, values[4].Key);
            Assert.AreEqual(24, values[4].Value.Second);
        }
    }
}
