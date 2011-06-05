using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RxSamples.WpfApplication.Examples.SystemMonitor
{
    /*This probably could be just a Rx join statement
    
     * application = "CombineLatest".
     *      (
     *          host1,
     *          host2,
     *          host3
     *      )
     *      
    */
    public class MonitoredApplication : INotifyPropertyChanged
    {
        private readonly string _name;
        private readonly ObservableCollection<MonitoredComponent> _subSystems = new ObservableCollection<MonitoredComponent>();
        private SystemStatus _status;

        public MonitoredApplication(string name, IObservable<MonitoredComponent> subSystems)
        {
            _name = name;

            SubSystems.CollectionChanged += SubSystems_CollectionChanged;

            subSystems.Subscribe
                (
                    _subSystems.Add,
                    () => _subSystems.Clear()
                );
        }

        public string Name
        {
            get { return _name; }
        }

        public ObservableCollection<MonitoredComponent> SubSystems
        {
            get { return _subSystems; }
        }

        public SystemStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    InvokePropertyChanged("Status");
                }
            }
        }

        void SubSystems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (MonitoredComponent monitoredSystem in e.OldItems)
                {
                    monitoredSystem.PropertyChanged -= SubSystem_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (MonitoredComponent monitoredSystem in e.NewItems)
                {
                    monitoredSystem.PropertyChanged += SubSystem_PropertyChanged;
                }
            }
            ReEvaluateStatus();
        }

        void SubSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")//Currently will only be Status, but for completeness.
            {
                var monitoredSystem = (MonitoredComponent)sender;
                EvaluateSubSystem(monitoredSystem);
            }
        }

        private void EvaluateSubSystem(MonitoredComponent monitoredComponent)
        {
            if (monitoredComponent.Status == SystemStatus.Offline)
            {
                this.Status = SystemStatus.Offline;
            }
            else
            {
                ReEvaluateStatus();
            }
        }

        private void ReEvaluateStatus()
        {
            if (SubSystems.Any(subSystem => subSystem.Status == SystemStatus.Offline))
            {
                this.Status = SystemStatus.Offline;
            }
            else if (SubSystems.Any(subSystem => subSystem.Status == SystemStatus.Unknown))
            {
                this.Status = SystemStatus.Unknown;
            }
            else if (SubSystems.IsEmpty())
            {
                this.Status = SystemStatus.Unknown;
            }
            else
            {
                this.Status = SystemStatus.Online;
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}