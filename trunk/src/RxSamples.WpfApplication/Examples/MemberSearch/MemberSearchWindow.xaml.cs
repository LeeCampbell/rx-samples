using System.Windows;

namespace RxSamples.WpfApplication.Examples.MemberSearch
{
    /// <summary>
    /// Interaction logic for MemberSearchWindow.xaml
    /// </summary>
    public partial class MemberSearchWindow : Window
    {
        private readonly TypeModel _model;

        public MemberSearchWindow()
        {
            InitializeComponent();
            _model = new TypeModel(new TypeService());
            DataContext = _model;
        }
    }
}
