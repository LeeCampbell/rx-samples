using System.Collections.ObjectModel;

namespace RxSamples.WpfApplication.Examples.TwapChart
{
    public class MaxLengthObservableCollection<T> : ObservableCollection<T>
    {
        private readonly int _length;

        public MaxLengthObservableCollection(int length)
        {
            _length = length;
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if(this.Count> _length)
                base.RemoveAt(0);
        }
    }
}