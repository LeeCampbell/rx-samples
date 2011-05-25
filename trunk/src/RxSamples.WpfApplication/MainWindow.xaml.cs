using System.Windows;
using RxSamples.WpfApplication.Examples.Gallery;
using RxSamples.WpfApplication.Examples.MemberSearch;


namespace RxSamples.WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GalleryUnReponsiveButton_Click(object sender, RoutedEventArgs e)
        {
            IImageService imageSvc = new ImageService();
            ShowGallery(new PhotoGalleryViewModel(imageSvc));
        }
        private void GalleryReponsiveButton_Click(object sender, RoutedEventArgs e)
        {
            IImageService imageSvc = new ImageService();
            ShowGallery(new ResponsivePhotoGalleryViewModel(imageSvc));
        }
        private void GalleryRxButton_Click(object sender, RoutedEventArgs e)
        {
            IImageService imageSvc = new ImageService();
            PhotoGalleryBase viewModel = new RxPhotoGalleryViewModel(imageSvc, new SchedulerProvider());
            ShowGallery(viewModel);
        }
        private void ChartTWAPButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Examples.TwapChart.TwapWindow();
            window.ShowDialog();
        }

        private static void ShowGallery(PhotoGalleryBase viewModel)
        {
            var window = new GalleryWindow {DataContext = viewModel};
            window.ShowDialog();
        }

        private void MemberSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new MemberSearchWindow();
            window.ShowDialog();
        }

        //Coming soon....
        //private void SystemMonitorButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var window = new SystemMonitorWindow();
        //    window.ShowDialog();
        //}
    }
}
