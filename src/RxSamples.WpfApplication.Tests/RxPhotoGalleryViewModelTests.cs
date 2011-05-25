using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RxSamples.WpfApplication.Examples.Gallery;

namespace RxSamples.WpfApplication.Tests
{
    [TestClass]
    public class RxPhotoGalleryViewModelTests
    {
        private Mock<IImageService> _imageSrvMock;
        private TestSchedulderProvider _testSchedulderProvider;
        private List<string> _expectedImages;

        [TestInitialize]
        public void SetUp()
        {
            _imageSrvMock = new Mock<IImageService>();
            _testSchedulderProvider = new TestSchedulderProvider();

            _expectedImages = new List<string> { "one.jpg", "two.jpg", "three.jpg" };
            _imageSrvMock.Setup(svc => svc.EnumerateImages())
                            .Returns(_expectedImages);

        }

        [TestMethod]
        public void Should_add_ImagesServiceResults_to_Images()
        {
            //Arrange
            // done in setup

            //Act
            var sut = new RxPhotoGalleryViewModel(_imageSrvMock.Object, _testSchedulderProvider);
            _testSchedulderProvider.ThreadPool.Run();
            _testSchedulderProvider.Dispatcher.Run();

            //Assert
            CollectionAssert.AreEqual(_expectedImages, sut.Images);
        }

        [TestMethod]
        public void Should_set_IsLoading_to_true()
        {
            //Arrange
            // done in setup

            //Act
            var sut = new RxPhotoGalleryViewModel(_imageSrvMock.Object, _testSchedulderProvider);

            //--NOTE-- note the missing TestScheduler.Run() calls. This will stop any observable being processed. Cool.

            //Assert
            Assert.IsTrue(sut.IsLoading);
        }

        [TestMethod]
        public void Should_set_IsLoading_to_false_when_completed_loading()
        {
            //Arrange
            // done in setup

            //Act
            var sut = new RxPhotoGalleryViewModel(_imageSrvMock.Object, _testSchedulderProvider);
            _testSchedulderProvider.ThreadPool.Run();
            _testSchedulderProvider.Dispatcher.Run();

            //Assert
            Assert.IsFalse(sut.IsLoading);
        }
    }
}
