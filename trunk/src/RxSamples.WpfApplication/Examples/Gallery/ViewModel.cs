using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;

//Check out the tests for the Rx version of the viewmodel (last class in this file) in the tests project RxPhotoGalleryViewModelTests.cs

namespace RxSamples.WpfApplication.Examples.Gallery
{
    /// <summary>
    ///  View model base class that just exposes an <c>Images</c> property and an <c>IsLoading</c> property.
    /// </summary>
    public abstract class PhotoGalleryBase : INotifyPropertyChanged
    {
        private readonly ObservableCollection<string> _images = new ObservableCollection<string>();
        public ObservableCollection<string> Images
        {
            get { return _images; }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    InvokePropertyChanged("IsLoading");
                }
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Naive implementation of the ViewModel. All work done on the dispatcher in a blocking fashion.
    /// </summary>
    public sealed class PhotoGalleryViewModel : PhotoGalleryBase
    {
        public PhotoGalleryViewModel(IImageService imageService)
        {
            IsLoading = true;

            var images = imageService.EnumerateImages();
            foreach (var image in images)
            {
                Images.Add(image);
            }
            //--OR--//images.ToList().ForEach(Images.Add);
            //--OR--//images.Run(Images.Add);

            IsLoading = false;
        }
    }

    /// <summary>
    /// Responsive implementation of the ViewModel.
    /// </summary>
    /// <remarks>
    /// Can be tested but involves threading and pushing "Dispatcher frames". Yuck.
    /// </remarks>
    public sealed class ResponsivePhotoGalleryViewModel : PhotoGalleryBase
    {
        public ResponsivePhotoGalleryViewModel(IImageService imageService)
        {
            IsLoading = true;
            var images = imageService.EnumerateImages();
            var dispatcher = Dispatcher.CurrentDispatcher;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (var imagePath in images)
                {
                    string path = imagePath;
                    Action addImage = () => Images.Add(path);
                    dispatcher.Invoke(addImage);
                }
                Action completed = () => IsLoading = false;
                dispatcher.Invoke(completed);
            });
        }
    }

    /// <summary>
    /// Tested Rx implementation of the ViewModel
    /// </summary>
    public sealed class RxPhotoGalleryViewModel : PhotoGalleryBase
    {
        public RxPhotoGalleryViewModel(IImageService imageService, ISchedulerProvider scheduler)
        {
            IsLoading = true;
            var files = imageService.EnumerateImages()
                                    .ToObservable();

            files
                .SubscribeOn(scheduler.ThreadPool)
                .ObserveOn(scheduler.Dispatcher)
                .Subscribe(
                    imagePath =>
                    {
                        Images.Add(imagePath);
                    },
                    () =>
                    {
                        IsLoading = false;
                    });
        }
    }
}