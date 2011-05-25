using System.Windows;
using System.Windows.Controls;

namespace RxSamples.WpfApplication.Controls
{
    public class Light : Control
    {
        static Light()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Light), new FrameworkPropertyMetadata(typeof(Light)));
        }
    }
}
