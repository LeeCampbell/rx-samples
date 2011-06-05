using System;
using System.ComponentModel;

namespace RxSamples.WpfApplication.Examples.SystemMonitor
{
    public  class MonitoredComponent : INotifyPropertyChanged
    {
        private readonly string _applicationName;
        private readonly ComponentType _componentType;
        private readonly string _serverName;
        private readonly string _instanceName;
        private SystemStatus _status;

        public MonitoredComponent(string applicationName, ComponentType componentType, string serverName, string instanceInstanceName, IObservable<SystemStatus> statusStream)
        {
            _applicationName = applicationName;
            _componentType = componentType;
            _serverName = serverName;
            _instanceName = instanceInstanceName;
            Status = SystemStatus.Unknown;

            statusStream.Subscribe(newStatus=>Status = newStatus);
        }

        public string InstanceName
        {
            get { return _instanceName; }
        }

        public SystemStatus Status
        {
            get { return _status; }
            set
            {
                if(_status!=value)
                {
                    _status = value;
                    InvokePropertyChanged("Status");
                }
            }
        }

        public string ApplicationName
        {
            get { return _applicationName; }
        }

        public ComponentType ComponentType
        {
            get { return _componentType; }
        }

        public string ServerName
        {
            get { return _serverName; }
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