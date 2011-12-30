using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace RxSamples.WpfApplication.Examples.MemberSearch
{
    public class TypeModel : INotifyPropertyChanged
    {
        private readonly ITypeService _typeService;
        private readonly ObservableCollection<string> _results = new ObservableCollection<string>();
        private string _searchText;
        private bool _isSearching;
        private IDisposable _currentSearch;

        public TypeModel(ITypeService typeService)
        {
            _typeService = typeService;

            var propChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => new PropertyChangedEventHandler(handler),
                h => this.PropertyChanged += h,
                h => this.PropertyChanged -= h);


            propChanged
                .Select(ev => ev.EventArgs.PropertyName)
                .Where(propertyName => propertyName == "SearchText")
                .Subscribe(_=>Search());
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if(value!=_searchText)
                {
                    _searchText = value;
                    InvokePropertyChanged("SearchText");
                }
            }
        }

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                if (value != _isSearching)
                {
                    _isSearching = value;
                    InvokePropertyChanged("IsSearching");
                }
            }
        }

        public ObservableCollection<string> Results
        {
            get { return _results; }
        }

        private void Search()
        {
            //Cancel existing search
            using (_currentSearch) { }  //  Dispose of the currentSearch, even if it is null
            IsSearching = true;
            //Clear out current results
            Results.Clear();

            //Subscribe to new search
            _currentSearch = _typeService.ListMethods(SearchText)
                //Schedule on the taskPool
                .SubscribeOn(Scheduler.TaskPool)
                .ObserveOnDispatcher()
                .Subscribe(
                    Results.Add, 
                    () => { IsSearching = false; });
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}