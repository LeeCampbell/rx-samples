using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Concurrency;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RxSamples.ConsoleApp;

namespace RxSamples.Testing
{
    [TestClass]
    public class MyPresenterFixture
    {
        private Mock<IMyService> _myServiceMock;
        private Mock<IViewModel> _viewModelMock;
        private Mock<ISchedulerService> _schedulerServiceMock;

        [TestInitialize]
        public void SetUp()
        {
            _myServiceMock = new Mock<IMyService>();
            _viewModelMock = new Mock<IViewModel>();
            _schedulerServiceMock = new Mock<ISchedulerService>();

            _schedulerServiceMock.SetupGet(ss => ss.Dispatcher).Returns(Scheduler.Immediate);
            _schedulerServiceMock.SetupGet(ss => ss.ThreadPool).Returns(Scheduler.Immediate);
            var prices = new ObservableCollection<decimal>();
            _viewModelMock.SetupGet(vm => vm.Prices).Returns(prices);
        }

        [TestMethod]
        public void Should_pass_symbol_to_MyService_PriceStream()
        {
            var expected = "SomeSymbol";
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.Is<string>(symbol=>symbol==expected))).Returns(priceStream);
            var prices = new ObservableCollection<decimal>();
            _viewModelMock.SetupGet(vm => vm.Prices).Returns(prices);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerServiceMock.Object);
            sut.Show(expected);

            _myServiceMock.Verify();
        }


        [TestMethod]
        public void Should_add_to_VM_Prices_when_MyService_publishes_price()
        {
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);
            var prices = new ObservableCollection<decimal>();
            _viewModelMock.SetupGet(vm => vm.Prices).Returns(prices);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerServiceMock.Object);
            sut.Show("SomeSymbol");
            decimal expected = 1.23m;
            priceStream.OnNext(expected);

            Assert.AreEqual(1, prices.Count);
            Assert.AreEqual(expected, prices.First());
        }

        [TestMethod]
        public void Should_subscribe_to_prices_on_threadPool()
        {
            decimal expected = 1.23m;
            var threadPoolScheduler = new TestScheduler();
            _schedulerServiceMock.SetupGet(ss => ss.ThreadPool).Returns(threadPoolScheduler);
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);
            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerServiceMock.Object);
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
            var threadPoolScheduler = new TestScheduler();
            _schedulerServiceMock.SetupGet(ss => ss.ThreadPool).Returns(threadPoolScheduler);
            var dispatcherScheduler = new TestScheduler();
            _schedulerServiceMock.SetupGet(ss => ss.Dispatcher).Returns(dispatcherScheduler);
            var priceStream = new Subject<decimal>();
            _myServiceMock.Setup(svc => svc.PriceStream(It.IsAny<string>())).Returns(priceStream);

            var sut = new MyPresenter(_myServiceMock.Object, _viewModelMock.Object, _schedulerServiceMock.Object);
            sut.Show("SomeSymbol");
            threadPoolScheduler.Schedule(() => priceStream.OnNext(expected));
            threadPoolScheduler.Run();

            Assert.AreEqual(0, _viewModelMock.Object.Prices.Count);
            dispatcherScheduler.Run();

            Assert.AreEqual(1, _viewModelMock.Object.Prices.Count);
            Assert.AreEqual(expected, _viewModelMock.Object.Prices.First());
        }
    }
}