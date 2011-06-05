using System;

namespace RxSamples.WpfApplication.Examples.SystemMonitor
{
    public interface IMonitorService
    {
        IObservable<MonitoredComponent> ListWebSites();
        IObservable<MonitoredComponent> ListServices();
        IObservable<MonitoredComponent> ListDatabases();
    }
}