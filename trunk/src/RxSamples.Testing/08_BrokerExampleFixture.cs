using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RxSamples.ConsoleApp;

namespace RxSamples.Testing
{
    [TestClass]
    public class BrokerExampleFixture
    {
        private TestSchedulers _testSchedulers;
        private static readonly string[] _keys = new[] { "AlphaEndpoint", "betaEndpoint", "deltaEndpoint" };
        private static readonly Endpoint _alphaEndpoint = new Endpoint { Key = _keys[0], Name = "AlphaEP" };
        private static readonly Endpoint _betaEndpoint = new Endpoint { Key = _keys[1], Name = "BetaEP" };
        private static readonly Endpoint _deltaEndpoint = new Endpoint { Key = _keys[2], Name = "DeltaEP" };

        [TestInitialize]
        public void SetUp()
        {
            _testSchedulers = new TestSchedulers();
        }

        [TestMethod]
        public void Should_translate_endpoint_to_stream()
        {
            var subscriptionProviderMock = new Mock<ISubscriptionProvider>();
            var sut = new BrokerExample(subscriptionProviderMock.Object, _testSchedulers);
        }

        [TestMethod]
        public void Should_batch_multiple_requests_within_BufferPeriod_to_get_stream()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_publish_only_unique_endpoints_in_request_when_duplicate_requests_are_made()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_share_subscriptions_when_duplicate_requests_are_made()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_unsubcscribe_when_Observable_is_disposed()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_get_new_subcscription_when_previous_duplicate_has_been_disposed()
        {
            Assert.Inconclusive();
        }
    }

    //[TestClass]
    //public class BrokerExampleFixture
    //{
    //    private TestSchedulers _testSchedulers;
    //    private static readonly string[] _keys = new[] { "AlphaEndpoint", "betaEndpoint", "deltaEndpoint" };
    //    private static readonly Endpoint _alphaEndpoint = new Endpoint { Key = _keys[0], Name = "AlphaEP" };
    //    private static readonly Endpoint _betaEndpoint = new Endpoint { Key = _keys[1], Name = "BetaEP" };
    //    private static readonly Endpoint _deltaEndpoint = new Endpoint { Key = _keys[2], Name = "DeltaEP" };

    //    [TestInitialize]
    //    public void SetUp()
    //    {
    //        _testSchedulers = new TestSchedulers();
    //    }

    //    [TestMethod]
    //    public void Should_translate_endpoint_to_stream()
    //    {
    //        var key = _alphaEndpoint.Key;
    //        var endpoint = _alphaEndpoint;
    //        var endpoints = Observable.Return(endpoint);
    //        var expected = "My result";
    //        var subscriptionProviderMock = new Mock<ISubscriptionProvider>();
    //        subscriptionProviderMock.Setup(
    //            sp => sp.GetEndpoints(Match.Create<IEnumerable<string>>(col => col.Single() == key)))
    //            .Returns(endpoints);

    //        subscriptionProviderMock.Setup(sp => sp.SubscribeToEndpoint(endpoint)).Returns(Observable.Return(expected));
    //        var sut = new BrokerExample(subscriptionProviderMock.Object, _testSchedulers);

    //        var stream = sut.GetStreamByKey(key);
    //        string actual = null;
    //        stream.Subscribe(value => actual = value);

    //        _testSchedulers.ThreadPool.RunTo(BrokerExample.BufferPeriod.Ticks);
    //        _testSchedulers.ThreadPool.RunTo(BrokerExample.BufferPeriod.Ticks * 2);
    //        _testSchedulers.Dispatcher.Run();

    //        Assert.AreEqual(expected, actual);
    //    }

    //    [TestMethod]
    //    public void Should_batch_multiple_requests_within_BufferPeriod_to_get_stream()
    //    {
    //        var endpoints = new[] { _alphaEndpoint, _betaEndpoint, _deltaEndpoint };

    //        var subscriptionProviderMock = new Mock<ISubscriptionProvider>();
    //        subscriptionProviderMock
    //            .Setup(sp => sp.GetEndpoints(It.IsAny<IEnumerable<string>>()))
    //            .Returns<IEnumerable<string>>(inputs =>
    //            {
    //                //if (inputs.SequenceEqual(_keys)) return endpoints;
    //                if (inputs.SequenceEqual(_keys.Take(2))) return endpoints.Take(2).ToObservable();
    //                if (inputs.SequenceEqual(_keys.Skip(2))) return endpoints.Skip(2).ToObservable();
    //                return null;
    //            });

    //        subscriptionProviderMock.Setup(sp => sp.SubscribeToEndpoint(endpoints[0])).Returns(Observable.Return("Stream 0"));
    //        subscriptionProviderMock.Setup(sp => sp.SubscribeToEndpoint(endpoints[1])).Returns(Observable.Return("Stream 1"));
    //        subscriptionProviderMock.Setup(sp => sp.SubscribeToEndpoint(endpoints[2])).Returns(Observable.Return("Stream 2"));


    //        var sut = new BrokerExample(subscriptionProviderMock.Object, _testSchedulers);

    //        var stream0 = sut.GetStreamByKey(_keys[0]);
    //        var stream1 = sut.GetStreamByKey(_keys[1]);
    //        var stream2 = sut.GetStreamByKey(_keys[2]);
    //        string actual0 = null, actual1 = null, actual2 = null;
    //        stream0.Subscribe(value => actual0 = value);
    //        stream1.Subscribe(value => actual1 = value);

    //        _testSchedulers.ThreadPool.RunNext(BrokerExample.BufferPeriod);

    //        Assert.AreEqual("Stream 0", actual0);
    //        Assert.AreEqual("Stream 1", actual1);

    //        stream2.Subscribe(value => actual2 = value);
    //        _testSchedulers.ThreadPool.RunNext(BrokerExample.BufferPeriod);

    //        Assert.AreEqual("Stream 2", actual2);
    //    }

    //    [TestMethod]
    //    public void Should_publish_only_unique_endpoints_in_request_when_duplicate_requests_are_made()
    //    {
    //        var endpoints = new[] { _alphaEndpoint, _betaEndpoint, _deltaEndpoint };

    //        var subscriptionProviderMock = new Mock<ISubscriptionProvider>();
    //        subscriptionProviderMock
    //            .Setup(sp => sp.GetEndpoints(It.IsAny<IEnumerable<string>>()))
    //            .Returns<IEnumerable<string>>(inputs =>
    //            {
    //                if (inputs.SequenceEqual(_keys.Take(1))) return endpoints.Take(1).ToObservable();
    //                return null;
    //            });
    //        subscriptionProviderMock.Setup(sp => sp.SubscribeToEndpoint(endpoints[0])).Returns(Observable.Return("Stream 0"));

    //        var sut = new BrokerExample(subscriptionProviderMock.Object, _testSchedulers);

    //        var stream0a = sut.GetStreamByKey(_keys[0]);
    //        var stream0b = sut.GetStreamByKey(_keys[0]);

    //        string actual0 = null, actual1 = null;
    //        stream0a.Subscribe(value => actual0 = value);
    //        stream0b.Subscribe(value => actual1 = value);

    //        _testSchedulers.ThreadPool.RunNext(BrokerExample.BufferPeriod);

    //        Assert.AreEqual("Stream 0", actual0);
    //        Assert.AreEqual("Stream 0", actual1);

    //        subscriptionProviderMock
    //            .Verify(sp => sp.GetEndpoints(It.IsAny<IEnumerable<string>>()), Times.Once());
    //    }

    //    [TestMethod]
    //    public void Should_share_subscriptions_when_duplicate_requests_are_made()
    //    {
    //        string expected = "Stream 0";
    //        var endpoints = new[] { _alphaEndpoint, _betaEndpoint, _deltaEndpoint };
    //        var subscriptionProviderMock = new Mock<ISubscriptionProvider>();
    //        subscriptionProviderMock
    //            .Setup(sp => sp.GetEndpoints(It.IsAny<IEnumerable<string>>()))
    //            .Returns<IEnumerable<string>>(inputs =>
    //            {
    //                if (inputs.SequenceEqual(_keys.Take(1))) return endpoints.Take(1).ToObservable();
    //                return null;
    //            });

    //        subscriptionProviderMock.Setup(sp => sp.SubscribeToEndpoint(endpoints[0])).Returns(Observable.Return(expected));

    //        var sut = new BrokerExample(subscriptionProviderMock.Object, _testSchedulers);

    //        var stream0a = sut.GetStreamByKey(_keys[0]);
    //        var stream0b = sut.GetStreamByKey(_keys[0]);

    //        string actual0 = null, actual1 = null;
    //        stream0a.Subscribe(value => actual0 = value);
    //        _testSchedulers.ThreadPool.RunNext(BrokerExample.BufferPeriod);
    //        stream0b.Subscribe(value => actual1 = value);
    //        _testSchedulers.ThreadPool.RunNext(BrokerExample.BufferPeriod);

    //        Assert.AreEqual(expected, actual0);
    //        Assert.AreEqual(expected, actual1);

    //        subscriptionProviderMock
    //            .Verify(sp => sp.GetEndpoints(It.IsAny<IEnumerable<string>>()), Times.Once());
    //    }

    //    [TestMethod]
    //    public void Should_unsubcscribe_when_Observable_is_disposed()
    //    {
    //        Assert.Inconclusive();
    //    }

    //    [TestMethod]
    //    public void Should_get_new_subcscription_when_previous_duplicate_has_been_disposed()
    //    {
    //        Assert.Inconclusive();
    //    }
    //}
}
