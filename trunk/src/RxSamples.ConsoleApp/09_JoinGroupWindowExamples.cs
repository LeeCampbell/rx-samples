using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;

namespace RxSamples.ConsoleApp
{
    public class JoinGroupWindowExamples
    {
        void Main()
        {
            //A window will be opened each time this publishes
            var windowOpens = Observable.Interval(TimeSpan.FromSeconds(1)).Take(3);

            //This is the stream from which values will be taken from and put into the window
            var windowValues = Observable.Interval(TimeSpan.FromMilliseconds(250));

            //Each time the "windowOpens" publishes, the func will return an observable. When this publishes or completes the window will close.
            var windowDuration = Observable.Timer(TimeSpan.FromSeconds(1));

            //Each time a window value is published, the func will return an Observable. When this publishes or completes that window value will be revmoed from the cache.
            //ie Use Observable.Return/Empty to only allow live window values, use Never to allow all windows to see all values, or use a timer to prove a timed cache.
            var windowValueLifetime = Observable.Timer(TimeSpan.FromSeconds(0.7));

            Observable.GroupJoin(
                windowOpens,
                windowValues,
                _ => windowDuration,
                _ => windowValueLifetime,
                (lhs, rhs) =>
                    {
                        //Console.WriteLine("Left pumped");
                        //return rhs;//.Do(r=>Console.WriteLine("{0}, {1}", lhs, r), ()=>Console.WriteLine("{0} completed", lhs));
                        //return new {Left=lhs, Right=rhs};
                        //return Observable.Return(lhs).CombineLatest(rhs, (l,r)=>new {Left=l, Right=r});
                        return Observable.Return(lhs).CombineLatest(rhs, (l, r) => string.Format("{0} {1}", l, r));
                    }
                ).Merge();
            //.Dump();
        }


        //void Main()
        //{
        //    var numbers = Observable.Interval(TimeSpan.FromMilliseconds(500));
        //    var letters = Observable.Interval(TimeSpan.FromMilliseconds(1200)).Select(i => Char.ConvertFromUtf32((int)i + 65));

        //    //letters.Dump();

        //    var myJoin = numbers.Join(letters, number=>)
        //                 //join letter in letters on 1 equals 1
        //                 join number2 in numbers.Delay(TimeSpan.FromMilliseconds(100)) on number equals number2
        //                 select 1;

        //    var myJoin = from number in numbers
        //                 //join letter in letters on 1 equals 1
        //                 join number2 in numbers.Delay(TimeSpan.FromMilliseconds(100)) on number equals number2
        //                 select 1;
        //    myJoin.Subscribe(Console.WriteLine);

        //}

        public void OrderMatchingSample()
        {
            var orders = CreateOrderStream();
            var prices = GetPriceStream().Publish();
            prices.Connect();

            Observable.Join(
                    orders,
                    prices,
                    order => order.Terminated(),
                    tick => prices,                 //Chur! When the next price comes then this one is stale!
                    (order, tick) => new { Order = order, Tick = tick }
                )
                .Where(pair => pair.Order.TryExecute(pair.Tick))
                .Subscribe(
                    pair => Console.WriteLine("Match Order{0} on price {1} ({2})", pair.Order.Id, pair.Tick.Price, pair.Tick.Id),
                    Console.WriteLine,
                    () => Console.WriteLine("Completed")
                );
        }

        public void VWAPSample()
        {
            var trades = GetTradeStream();
            var vwapParameter = TimeSpan.FromMilliseconds(1000);

            //Should see 4.5 (36/8) for first value
            Observable.Interval(TimeSpan.FromMilliseconds(125))
                .VWAP(i => i, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(125), Scheduler.ThreadPool)
                .Subscribe(Console.WriteLine);

        }

        private IObservable<Trade> GetTradeStream()
        {
            var prices = ObservableFactory.OscilatingStream(1.0m, 1.0m, 1000000.5m, 0m, TimeSpan.FromMilliseconds(250));
            var volumes = ObservableFactory.OscilatingStream(100000, 1, 999999999, 50000, TimeSpan.FromMilliseconds(250));

            return prices.Zip(volumes, (price, vol) => new Trade { Price = price, Volume = (int)vol });
        }

        private IObservable<Tick> GetPriceStream()
        {
            //I want to have prices wobble around 1.57-1.59 for 4 seconds. This will let Order2 expire. 
            //If the liquidity at the end goes up to 100k then
            //Order 4 can fill when the liquidity lets it.
            var ticks = new[]
                            {
                                new Tick {Id = 1, Price = 1.57m, Liquidity = 50000},
                                new Tick {Id = 2, Price = 1.58m, Liquidity = 100000},
                                new Tick {Id = 3, Price = 1.57m, Liquidity = 100000},
                                new Tick {Id = 4, Price = 1.58m, Liquidity = 10000},
                                new Tick {Id = 5, Price = 1.59m, Liquidity = 4000},
                                new Tick {Id = 6, Price = 1.58m, Liquidity = 100000},
                                new Tick {Id = 7, Price = 1.59m, Liquidity = 10000},
                                new Tick {Id = 8, Price = 1.60m, Liquidity = 60000},
                                new Tick {Id = 9, Price = 1.60m, Liquidity = 100000},
                                new Tick {Id = 10, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 11, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 12, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 13, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 14, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 15, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 16, Price = 1.59m, Liquidity = 100000},
                                new Tick {Id = 17, Price = 1.61m, Liquidity = 100000},
                                new Tick {Id = 18, Price = 1.59m, Liquidity = 100000},
                };
            return Observable.Interval(TimeSpan.FromMilliseconds(500))
                .Take(ticks.Count())
                .Select(i => ticks[i]);
        }

        private IObservable<Order> CreateOrderStream()
        {
            var orders = new[]
                             {
                                 new Order {Id = "Order1", Limit = 1.6m, Volume = 200000, Expiry = DateTimeOffset.Now.AddSeconds(300)},
                                 new Order {Id = "Order2", Limit = 1.6m, Volume = 5000, Expiry = DateTimeOffset.Now.AddSeconds(3)},
                                 new Order {Id = "Order3", Limit = 1.6m, Volume = 10000, Expiry = DateTimeOffset.Now.AddSeconds(10)},
                                 new Order {Id = "Order4", Limit = 1.59m, Volume = 100000, Expiry = DateTimeOffset.Now.AddSeconds(300)},
                                 new Order {Id = "Order3 Copy", Limit = 1.6m, Volume = 10000, Expiry = DateTimeOffset.Now.AddSeconds(10)},
                                 new Order {Id = "Order5", Limit = 1.55m, Volume = 1000, Expiry = DateTimeOffset.Now.AddSeconds(10)},
            };
            return orders.SkipLast(1).ToObservable()
                .Concat(Observable.Timer(TimeSpan.FromSeconds(3)).Select(_ => orders.Last()));
        }

    }

    public class Trade
    {
        public Decimal Price { get; set; }
        public int Volume { get; set; }
    }

    public class Order
    {
        private readonly Subject<Unit> _execution = new Subject<Unit>();
        public string Id { get; set; }
        public decimal Limit { get; set; }
        public int Volume { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public bool TryExecute(Tick tick)
        {
            if (!tick.IsStale
                && tick.Price >= Limit
                && tick.Liquidity >= Volume)
            {
                tick.IsStale = true;
                _execution.OnCompleted();
                return true;
            }
            return false;
        }

        public IObservable<Unit> Terminated()
        {
            return Observable.Amb(
                Observable.Timer(Expiry).Select(_ => new Unit()),
                _execution);
        }
    }

    public class Tick
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Liquidity { get; set; }
        public bool IsStale { get; set; }
    }

    public static class ObservableFactory
    {
        public static IObservable<decimal> OscilatingStream(decimal initial, decimal min, decimal max, decimal step, TimeSpan period)
        {
            var isIncreasing = initial < max;
            return Observable.GenerateWithTime(
                initial,
                _ => true,
                price =>
                {
                    if (isIncreasing)
                    {
                        if ((price += step) > max) isIncreasing = false;
                    }
                    else
                    {
                        if ((price -= step) < min) isIncreasing = true;

                    }
                    return price;
                },
                    price => price,
            _ => period);
        }

        public static IObservable<decimal> VWAP<T>(this IObservable<T> source, Func<T, decimal> valueExpression, TimeSpan windowTime)
        {
            return VWAP(source, valueExpression, windowTime, Scheduler.ThreadPool);
        }
        public static IObservable<decimal> VWAP<T>(this IObservable<T> source, Func<T, decimal> valueExpression, TimeSpan windowTime, IScheduler scheduler)
        {
            //These all  create contiguous windows, we want overlapped windows.
            //return source.WindowWithTime(windowTime)
            //    .Select(window=>window.Select(valueExpression).Average())
            //    .Merge();

            //return source.Window(() => Observable.Timer(windowTime))
            //    .Select(window => window.Select(valueExpression).Average())
            //    .Merge();

            return Observable.GroupJoin(
                source.Do(_ => Console.WriteLine("Pumped value {0}", _)),   //Defines the start events ie things that open a window
                source,                                                     //Defines the stream that is observed during the window
                _ => Observable.Timer(windowTime, scheduler),               //Defines when the window is closed from the first arg.
                _ => Observable.Return(new Unit()),                         //No fucking idea.
                (initialValue, subsequentValues) => 
                    subsequentValues
                        .Select(valueExpression)
                        .Do(i => Console.WriteLine("   {0} of {1} @ {2}", i, initialValue, scheduler.Now), () => Console.WriteLine("   x of {0} @ {1}", initialValue, scheduler.Now))
                        .Average())
                //(_, window) => window.Select(valueExpression))
                //.Do(i=> Console.WriteLine("count {0}, sum {1}", i.Count(), i.Sum()))
                //.Select(w=>w.Average())
                .Merge();
        }

        public static IObservable<decimal> VWAP<T>(this IObservable<T> source, Func<T, decimal> valueExpression, TimeSpan windowTime, TimeSpan samplePeriod, IScheduler scheduler)
        {
            return Observable.GroupJoin(
                Observable.Interval(samplePeriod, scheduler),   //Defines the start events ie things that open a window
                source,                                                     //Defines the stream that is observed during the window
                _ => Observable.Timer(windowTime, scheduler),               //Defines when the window is closed from the first arg.
                _ => Observable.Return(new Unit()),                         //No fucking idea.
                (initialValue, subsequentValues) =>
                    subsequentValues
                        .Select(valueExpression)
                        //.Do(i => Console.WriteLine("---{0} of {1} @ {2}", i, initialValue, scheduler.Now), () => Console.WriteLine("   x of {0} @ {1}", initialValue, scheduler.Now))
                        .Average().Catch<decimal, InvalidOperationException>(ex => Observable.Empty<decimal>())
                        //.Do(i => Console.WriteLine("==={0} of {1} @ {2}", i, initialValue, scheduler.Now))
                        )
                .Merge();
        }
    }
}
