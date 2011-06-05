using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RxSamples.WpfApplication.Examples.SystemMonitor
{
    public class ViewModel
    {
        private readonly ObservableCollection<MonitoredApplication> _applications = new ObservableCollection<MonitoredApplication>();

        public ViewModel()
        {
            var svrWebDMZ1 = new MonitorService("Server_Web_DMZ_01");
            var svrWebLAN2 = new MonitorService("Server_Web_LAN_02");
            var svrDB = new MonitorService("Server_DB_MSSQL");

            var svrWebDMZ1Components = GetComponents(svrWebDMZ1);
            var svrWebLAN2Components = GetComponents(svrWebDMZ1);
            var svrDBComponents = GetComponents(svrWebDMZ1);

            var apps = from monitoredApplication in Applications 

            var blah = Observable.Join
                (
                    web1,
                    web2,
                    dbServer
                )
                .Select()

            var components = Observable.Merge
                (
                    web1.ListDatabases(),
                    web1.ListServices(),
                    web1.ListWebSites(),
                    
                    web2.ListDatabases(),
                    web2.ListServices(),
                    web2.ListWebSites(),

                    dbServer.ListDatabases(),
                    dbServer.ListServices(),
                    dbServer.ListWebSites()
                );

            var applications = from component in components
                               group component by component.ApplicationName into app
                               select new MonitoredApplication(app.Key, app);

            applications.Subscribe(Applications.Add);
        }

        
        public ObservableCollection<MonitoredApplication> Applications
        {
            get { return _applications; }
        }

        private IObservable<MonitoredComponent> GetComponents(IMonitorService monitorService)
        {
            return Observable.Merge
                (
                    monitorService.ListDatabases(),
                    monitorService.ListServices(),
                    monitorService.ListWebSites()
                );
        }

    }
}
