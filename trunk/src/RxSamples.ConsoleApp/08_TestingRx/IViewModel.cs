using System.Collections.ObjectModel;

namespace RxSamples.ConsoleApp.TestingRx
{
    public interface IViewModel
    {
        ObservableCollection<decimal> Prices { get; }
        bool IsConnected { get; set; }
    }
}