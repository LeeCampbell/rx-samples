using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using RxSamples.ConsoleApp.TestingRx;

//TODO: Breaking change 1.0.2838.104. BufferWithTime now returns IObservable<T> which screws with .DistinctUntilChanged(...)
//Basically need to rewrite once I figure out how to do it again.

namespace RxSamples.ConsoleApp
{
    //Really should be a broker (which Imagine could end up being completly generic and is actually probably just some extension method)
    //and seperately an EndPointProvider


    public class BrokerExample : IDisposable
    {
        public static readonly TimeSpan BufferPeriod = TimeSpan.FromMilliseconds(100);

        private readonly ISubscriptionProvider _subscriptionProvider;
        private readonly ISchedulerProvider _schedulerProvider;

        public BrokerExample(ISubscriptionProvider subscriptionProvider, ISchedulerProvider schedulerProvider)
        {
            
        }

        public IObservable<string> GetStreamByKey(string endpointKey)
        {
            return from endpoint in _subscriptionProvider.GetEndpoints(new[] {endpointKey})
                   from data in _subscriptionProvider.SubscribeToEndpoint(endpoint)
                   select data;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    //public class BrokerExample : IDisposable
    //{
    //    public static readonly TimeSpan BufferPeriod = TimeSpan.FromMilliseconds(100);

    //    private readonly ISubscriptionProvider _subscriptionProvider;
    //    private readonly ISchedulerProvider _schedulerProvider;
    //    private readonly Subject<string> _endPointsRequested = new Subject<string>();
    //    private readonly ISubject<Endpoint> _endPoints = new ReplaySubject<Endpoint>();
    //    private readonly IDisposable _endPointSubscription;

    //    //TODO: Breaking change 1.0.2838.104. BufferWithTime now returns IObservable<T> which screws with .DistinctUntilChanged(...)
    //    public BrokerExample(ISubscriptionProvider subscriptionProvider, ISchedulerProvider schedulerProvider)
    //    {
    //        _subscriptionProvider = subscriptionProvider;
    //        _schedulerProvider = schedulerProvider;
    //        //var bufferComparer = new ListComparer<string>();
    //        //_endPointSubscription = _endPointsRequested
    //        //    .BufferWithTime(BufferPeriod, _schedulerProvider.ThreadPool)
    //        //    //.DistinctUntilChanged(streamComparer)     //TODO: Validate this is still working correctly as IObservable
    //        //    .Where(batch => batch.Any().First())
    //        //    .Subscribe(batch => 
    //        //        _subscriptionProvider
    //        //            .GetEndpoints(batch.ToEnumerable().Distinct())
    //        //            .Run(_endPoints.OnNext));

    //        var endPointRequests = _endPointsRequested
    //            .BufferWithTime(BufferPeriod, _schedulerProvider.ThreadPool);

    //        var endPointQuery = from endpointBatch in endPointRequests
    //                            from endPoints in GetEndpoints(endpointBatch)
    //                            select endPoints;

    //        //var endPointQuery = from requestedBatch in _endPointsRequested
    //        //                        .BufferWithTime(BufferPeriod, _schedulerProvider.ThreadPool)
    //        //                    select requestedBatch.ToAsyncEnumerable();
    //        //                        //where requestedBatch.Any().Single()
    //        //                        let distinctRequests = requestedBatch.ToEnumerable().Distinct()
    //        //                        from endPoints in _subscriptionProvider.GetEndpoints(distinctRequests)
    //        //                        select endPoints;
    //        _endPointSubscription = endPointQuery.Subscribe(_endPoints);
    //    }

    //    public IObservable<string> GetStreamByKey(string endpointKey)
    //    {
    //        return Observable.CreateWithDisposable<string>(
    //            observer =>
    //                (
    //                    from endpoint in GetEndpoint(endpointKey)
    //                    from data in _subscriptionProvider.SubscribeToEndpoint(endpoint)
    //                    select data
    //                ).Subscribe(observer)
    //            );
    //    }

    //    private IObservable<Endpoint> GetEndpoint(string endpointKey)
    //    {
    //        return Observable.CreateWithDisposable<Endpoint>(observer =>
    //            {
    //                var subscription = _endPoints
    //                    .Where(ep => ep.Key == endpointKey)
    //                    .Subscribe(observer);
    //                _endPointsRequested.OnNext(endpointKey);
    //                return subscription;
    //            });
    //    }

    //    private IObservable<Endpoint> GetEndpoints(IObservable<string> endpointKeys)
    //    {
    //        return Observable.CreateWithDisposable<Endpoint>(
    //            o =>
    //            {
    //                var endpointBatch = new List<string>();
    //                var token = endpointKeys.Subscribe(
    //                    endpointBatch.Add,
    //                    () => _subscriptionProvider.GetEndpoints(endpointBatch)
    //                            .Subscribe(o));
    //                return token;
    //            });
    //    }

    //    #region Implementation of IDisposable

    //    /// <summary>
    //    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    //    /// </summary>
    //    /// <filterpriority>2</filterpriority>
    //    public void Dispose()
    //    {
    //        _endPointSubscription.Dispose();
    //    }

    //    #endregion
    //}

    public interface ISubscriptionProvider
    {
        //IEnumerable<Endpoint> GetEndpoints(IEnumerable<string> endpointKey);
        IObservable<Endpoint> GetEndpoints(IEnumerable<string> endpointKey);
        IObservable<string> SubscribeToEndpoint(Endpoint endpoint);
    }

    public class Endpoint
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public class ListComparer<T> : IEqualityComparer<IList<T>>
    {
        #region Implementation of IEqualityComparer<in IList<T>>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
        public bool Equals(IList<T> x, IList<T> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            //Neither are null now.
            if (x.Count != y.Count) return false;
            for (int i = 0; i < x.Count; i++)
            {
                if (!Equals(x[i], y[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(IList<T> obj)
        {
            return obj == null ? 0 : obj.Count;
        }

        #endregion
    }
}
