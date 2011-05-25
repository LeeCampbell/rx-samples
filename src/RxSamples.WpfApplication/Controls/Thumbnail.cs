using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

//TODO: If DPI < 96dpi it comes out blury. Can we sort some thing out for that?
//TODO: memory problems currently. i think I double handle stuff too much. FIXED? 430MB of images only used 80MB of memory (ProcMon eyeball testing)
//TODO: Only add the thumbnail to the queue once it IsVisible = true
//TODO: Changing MaxWidth,Height, Source just set it to invalid. If it is visible when this happens then add to queue.
//TODO: The above dont work, as I want to be able to set its visibility based on if it is loading (catch 22).
//http://blogs.msdn.com/dancre/archive/2006/02/06/implementing-a-virtualized-panel-in-wpf-avalon.aspx
//http://blogs.msdn.com/jgoldb/archive/2008/03/08/performant-virtualized-wpf-canvas.aspx

namespace RxSamples.WpfApplication.Controls
{
    /// <summary>
    /// A control that provides asynchronous loading of a dimension constrained image.
    /// </summary>
    /// <remarks>
    /// This control is intended to be used inplace of <see cref="T:System.Windows.Controls.Image"/> 
    /// which performs all of it's work on the UI thread. This causes the UI to become unresponsive 
    /// when loading large images.
    /// </remarks>
    public sealed class Thumbnail : Control
    {
        #region Private fields
        private const int DeviceIndependenyLogicUnit = 96;
        //Static fields
        private static bool _processing;
        private static readonly ConcurrentQueue<Thumbnail> PendingThumbnails = new ConcurrentQueue<Thumbnail>();
        //Instance fields
        private readonly TransformImageDecoder _decoder = new TransformImageDecoder();
        private readonly object _renderRequestsSync = new object();
        private readonly ConcurrentStack<ImageParameters> _renderRequests = new ConcurrentStack<ImageParameters>();
        private bool _unloaded;
        #endregion

        static Thumbnail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Thumbnail), new FrameworkPropertyMetadata(typeof(Thumbnail)));
        }
        public Thumbnail()
        {
            Unloaded += Thumbnail_Unloaded;
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == MaxWidthProperty || e.Property == MaxHeightProperty)
            {
                QueueResolveImage();
            }
        }

        private void Thumbnail_Unloaded(object sender, RoutedEventArgs e)
        {
            _unloaded = true;
        }

        #region UriSource Property
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register("UriSource", typeof(Uri), typeof(Thumbnail), new UIPropertyMetadata(UriSource_Changed));
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        private static void UriSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thumbnail = (Thumbnail)d;
            thumbnail.QueueResolveImage();
        }
        #endregion

        #region ImageSource Property
        //Does this need to be a DP if it readonly? Obviously I cant target it for binding, but I should get autonotification for it.
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(Thumbnail), new UIPropertyMetadata());
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            private set { SetValue(ImageProperty, value); }
        }
        #endregion

        #region IsLoading Property
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(Thumbnail), new UIPropertyMetadata(IsLoading_Changed));
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            private set { SetValue(IsLoadingProperty, value); }
        }
        private static void IsLoading_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thumbnail = (Thumbnail)d;
            if (thumbnail.IsLoading)
                thumbnail.HasLoadFailed = false;
        }
        #endregion

        #region HasLoadFailed Property
        public static readonly DependencyProperty HasLoadFailedProperty = DependencyProperty.Register("HasLoadFailed", typeof(bool), typeof(Thumbnail));
        public bool HasLoadFailed
        {
            get { return (bool)GetValue(HasLoadFailedProperty); }
            private set { SetValue(HasLoadFailedProperty, value); }
        }
        #endregion

        private void QueueResolveImage()
        {
            IsLoading = true;
            _renderRequests.Push(new ImageParameters(UriSource, MaxWidth, MaxHeight));
            PendingThumbnails.Enqueue(this);
            WakeProcessor();
        }

        private void StartImageResample()
        {
            ImageParameters parameters;
            lock (_renderRequestsSync)
            {
                if (_renderRequests.TryPop(out parameters))
                {
                    _renderRequests.Clear();
                }
            }
            if (parameters != null)
                ResampleImage(parameters);
        }

        private void ResampleImage(ImageParameters parameters)
        {
            ImageSource result;
            if (string.IsNullOrEmpty(parameters.Path))
            {
                result = null;
            }
            else
            {
                if (!IsParameterRelevant(parameters))
                    return;
                byte[] buffer = File.ReadAllBytes(parameters.Path);
                result = _decoder.Decode(buffer, parameters.MaxWidth, parameters.MaxHeight);
                buffer = null;//does this help at all?
            }
            Dispatcher.BeginInvoke(
                new ThreadStart(delegate
                {
                    Image = result;
                    IsLoading = false;
                    if (result == null)
                    {
                        HasLoadFailed = true;
                    }
                }));
        }


        /// <summary>
        /// Determines whether the specified parameter is still relevant.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// 	<c>true</c> if the specified parameter is equal to the most recent render request; otherwise, <c>false</c>.
        /// </returns>
        private bool IsParameterRelevant(ImageParameters parameter)
        {
            ImageParameters last;
            _renderRequests.TryPeek(out last);
            return _unloaded || (last == null) || parameter.Equals(last);
        }

        #region Static Methods
        private static void WakeProcessor()
        {
            if (_processing)
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(ProcessImages);
        }

        private static void ProcessImages(object ignored)
        {
            _processing = true;
            while (!PendingThumbnails.IsEmpty)
            {
                Thumbnail thumbnail;
                if (PendingThumbnails.TryDequeue(out thumbnail))
                {
                    thumbnail.StartImageResample();
                }
            }
            _processing = false;
        }


        #endregion

        #region Private classes
        private sealed class ImageParameters
        {
            private readonly string _path;
            private readonly double _maxHeight;
            private readonly double _maxWidth;

            public ImageParameters(Uri path, double maxWidth, double maxHeight)
            {
                _path = (path == null) ? null : path.LocalPath;
                _maxWidth = maxWidth;
                _maxHeight = maxHeight;
            }

            public string Path
            {
                get { return _path; }
            }

            public double MaxHeight
            {
                get { return _maxHeight; }
            }

            public double MaxWidth
            {
                get { return _maxWidth; }
            }

            public override bool Equals(object obj)
            {
                var other = obj as ImageParameters;
                if (other == null)
                    return false;
                return (other.MaxWidth == MaxWidth) &&
                    (other.MaxHeight == MaxHeight) &&
                    string.Equals(other.Path, Path, StringComparison.InvariantCultureIgnoreCase);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private interface IImageSourceDecoder
        {
            ImageSource Decode(byte[] content, double maxWidth, double maxHeight);
        }
        public class TransformImageDecoder : IImageSourceDecoder
        {
            private const string ErrorCategory = "Thumbnail Errors";

            #region IImageSourceDecoder Members

            public ImageSource Decode(byte[] content, double maxWidth, double maxHeight)
            {
                using (Stream mem = new MemoryStream(content))
                {
                    BitmapDecoder imgDecoder;
                    try
                    {
                        imgDecoder = BitmapDecoder.Create(mem, BitmapCreateOptions.None, BitmapCacheOption.None);
                    }
                    catch (NotSupportedException nsEx)
                    {
                        Debug.Write(nsEx, ErrorCategory);
                        return null;
                    }

                    BitmapFrame frame = imgDecoder.Frames[0];
                    double scale = GetTransormScale(maxWidth, maxHeight, frame.PixelWidth, frame.PixelHeight);
                    var dpix = frame.DpiX > 0 ? frame.DpiX : DeviceIndependenyLogicUnit;
                    var dpiy = frame.DpiY > 0 ? frame.DpiY : DeviceIndependenyLogicUnit;
                    var xScale = scale * (dpix / DeviceIndependenyLogicUnit);
                    var yScale = scale * (dpiy / DeviceIndependenyLogicUnit);

                    BitmapSource thumbnail = ScaleBitmap(frame, xScale, yScale);

                    // this will disconnect the stream from the image completely ...
                    try
                    {
                        var writable = new WriteableBitmap(thumbnail);
                        writable.Freeze();
                        return writable;
                    }
                    catch (InvalidCastException icEx) //SystemException felt a bit too heavy...maybe not.
                    {
                        Debug.Write(icEx, ErrorCategory);
                        return null;
                    }
                    catch (IOException ioEx)
                    {
                        Debug.Write(ioEx, ErrorCategory);
                        return null;
                    }
                    catch (FileFormatException ffEx)
                    {
                        Debug.Write(ffEx, ErrorCategory);
                        return null;
                    }
                }
            }

            #endregion

            private static double GetTransormScale(double maxWidth, double maxHeight, double currentWidth, double currentHeight)
            {
                double xRatio = maxWidth / currentWidth;
                double yRatio = maxHeight / currentHeight;
                double resizeRatio = (xRatio > yRatio) ? xRatio : yRatio;
                if (resizeRatio > 1)
                    resizeRatio = 1;
                return resizeRatio;
            }

            //private static BitmapSource ScaleBitmap(BitmapSource source, double scale)
            //{
            //    return ScaleBitmap(source, scale, scale);
            //}
            private static BitmapSource ScaleBitmap(BitmapSource source, double xScale, double yScale)
            {
                if (xScale > 0.9999 && xScale < 1.0001
                    &&
                    yScale > 0.9999 && yScale < 1.0001)
                {
                    return source;
                }

                var thumbnail = new TransformedBitmap();
                thumbnail.BeginInit();
                thumbnail.Source = source;
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new ScaleTransform(xScale, yScale));
                thumbnail.Transform = transformGroup;
                thumbnail.EndInit();
                return thumbnail;
            }
        }
        #endregion
    }
}