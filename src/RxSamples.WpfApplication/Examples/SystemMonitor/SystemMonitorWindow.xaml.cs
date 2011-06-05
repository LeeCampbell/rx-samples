namespace RxSamples.WpfApplication.Examples.SystemMonitor
{
    /// <summary>
    /// Interaction logic for SystemMonitorWindow.xaml
    /// </summary>
    public partial class SystemMonitorWindow
    {
        public SystemMonitorWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
    }
}
