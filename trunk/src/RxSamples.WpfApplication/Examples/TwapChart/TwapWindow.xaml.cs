namespace RxSamples.WpfApplication.Examples.TwapChart
{
    /// <summary>
    /// Interaction logic for Time Weighted Average window
    /// </summary>
    public partial class TwapWindow
    {
        public TwapWindow()
        {
            InitializeComponent();
            DataContext = new PricesViewModel(new PriceService());
        }
    }
}
