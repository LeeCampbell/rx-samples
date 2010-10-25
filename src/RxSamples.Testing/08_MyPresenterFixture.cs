using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RxSamples.ConsoleApp.TestingRx;

namespace RxSamples.Testing
{
    [TestClass]
    public class MyPresenterFixture
    {
        private Mock<IMyService> _myServiceMock;
        private Mock<IViewModel> _viewModelMock;
        private TestSchedulers _schedulerService;

        [TestInitialize]
        public void SetUp()
        {
            _myServiceMock = new Mock<IMyService>();
            _viewModelMock = new Mock<IViewModel>();
            _schedulerService = new TestSchedulers();

            var prices = new ObservableCollection<decimal>();
            _viewModelMock.SetupGet(vm => vm.Prices).Returns(prices);
            _viewModelMock.SetupProperty(vm => vm.IsConnected);
        }

        [TestMethod]
        public void Should_pass_symbol_to_MyService_PriceStream()
        {
            var expected = "SomeSymbol";
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.Is<string>(symbol => symbol == expected))).Returns(priceStream);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerService);
            sut.Show(expected);

            _myServiceMock.Verify();
        }

        [TestMethod]
        public void Should_add_to_VM_Prices_when_MyService_publishes_price()
        {
            decimal expected = 1.23m;
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerService);
            sut.Show("SomeSymbol");
            _schedulerService.ThreadPool.Schedule(() => priceStream.OnNext(expected));  //Schedule the OnNext
            _schedulerService.ThreadPool.RunTo(1);  //Execute the OnNext action
            _schedulerService.Dispatcher.RunTo(1);  //Execute the OnNext Handler (ie adding to the Prices collection)

            Assert.AreEqual(1, _viewModelMock.Object.Prices.Count);
            Assert.AreEqual(expected, _viewModelMock.Object.Prices.First());
        }

        [TestMethod]
        public void Should_subscribe_to_prices_on_threadPool()
        {
            decimal expected = 1.23m;
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);
            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerService);
            sut.Show("SomeSymbol");

            priceStream.OnNext(expected);
            //this should yeild 0 rows. If Done correctly as per below then it would be count==1...
            //threadPoolScheduler.Schedule(() => priceStream.OnNext(expected));
            //threadPoolScheduler.Run();

            Assert.AreEqual(0, _viewModelMock.Object.Prices.Count);
        }

        [TestMethod]
        public void Should_observe_on_dispatcher()
        {
            decimal expected = 1.23m;
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerService);
            sut.Show("SomeSymbol");
            _schedulerService.ThreadPool.Schedule(() => priceStream.OnNext(expected));
            _schedulerService.ThreadPool.RunTo(1);

            Assert.AreEqual(0, _viewModelMock.Object.Prices.Count);
            _schedulerService.Dispatcher.Run();

            Assert.AreEqual(1, _viewModelMock.Object.Prices.Count);
            Assert.AreEqual(expected, _viewModelMock.Object.Prices.First());
        }

        [TestMethod]
        public void Should_timeout_if_no_prices_for_10_seconds()
        {
            var timeoutPeriod = TimeSpan.FromSeconds(10);
            var priceStream = Observable.Never<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerService);
            sut.Show("SomeSymbol");

            _schedulerService.ThreadPool.RunTo(timeoutPeriod.Ticks - 1);
            Assert.IsTrue(_viewModelMock.Object.IsConnected);

            _schedulerService.ThreadPool.RunTo(timeoutPeriod.Ticks);
            Assert.IsFalse(_viewModelMock.Object.IsConnected);
        }
    }
}