using System;
using System.Linq;

namespace RxSamples.WpfApplication.Examples.SystemMonitor
{
    /*
     * Add a ctor that takes a "servername" so I can return lists of websites, windows services, and databases.
     * Using a select transform, create an object like new {ApplicationName="SDP", Server="SRVPAR1H02", Component="WebSite", Instance="Alpha.web2.socgen.com"}
    */
    public class MonitorService : IMonitorService
    {
        private readonly string _serverName;
        private readonly Random _rnd = new Random();

        public MonitorService(string serverName)
        {
            _serverName = serverName;
        }

        public string ServerName
        {
            get { return _serverName; }
        }

        #region Implementation of IMonitorService

        public IObservable<MonitoredComponent> ListWebSites()
        {
            var websites = new[]
                           {
                               new MonitoredComponent("SDP", ComponentType.WebSite, _serverName, "alpha.DMZ1.socgen.com", SystemStatus()),
                               new MonitoredComponent("SDP", ComponentType.WebSite, _serverName, "alpha.DMZ2.socgen.com", SystemStatus()),
                               new MonitoredComponent("SDP", ComponentType.WebSite, _serverName, "alpha.LAN2.socgen.com", SystemStatus()),
                               new MonitoredComponent("SDP", ComponentType.WebSite, _serverName, "alpha.LAN2.socgen.com", SystemStatus()),

                           }.ToObservable();
            if (_serverName.Contains("DMZ"))
            {
                return websites.Where(site => site.InstanceName.Contains("DMZ"));
            }
            if (_serverName.Contains("LAN"))
            {
                return websites.Where(site => site.InstanceName.Contains("LAN"));
            }
            return websites;
        }

        public IObservable<MonitoredComponent> ListServices()
        {
            var services = new[]
                               {
                                   new MonitoredComponent("SDP", ComponentType.WindowsService, _serverName, "alpha.CoreServices.1", SystemStatus()),
                                   new MonitoredComponent("SDP", ComponentType.WindowsService, _serverName, "alpha.CoreServices.2", SystemStatus()),
                                   new MonitoredComponent("FXO", ComponentType.WindowsService, _serverName, "alpha.OrderRepository.1", SystemStatus()),
                                   new MonitoredComponent("FXO", ComponentType.WindowsService, _serverName, "alpha.OrderRepository.2", SystemStatus()),
                                   new MonitoredComponent("ABB", ComponentType.WindowsService, _serverName, "alpha.Backbone.1", SystemStatus()),
                                   new MonitoredComponent("ABB", ComponentType.WindowsService, _serverName, "alpha.Backbone.2", SystemStatus()),
                                   new MonitoredComponent("ABT", ComponentType.WindowsService, _serverName, "alpha.Blotter.1", SystemStatus()),
                                   new MonitoredComponent("ABT", ComponentType.WindowsService, _serverName, "alpha.Blotter.2", SystemStatus()),
                                   new MonitoredComponent("DHB", ComponentType.WindowsService, _serverName, "DAT2_GLOBAL", SystemStatus()),
                               }.ToObservable();

            if (_serverName.EndsWith("1"))
            {
                return services.Where(svc => svc.InstanceName.EndsWith("1"));
            }
            if (_serverName.EndsWith("2"))
            {
                return services.Where(svc => svc.InstanceName.EndsWith("2"));
            }
            return services;
        }

        public IObservable<MonitoredComponent> ListDatabases()
        {
            var dbs = new[]
                          {
                              new MonitoredComponent("SDP", ComponentType.Database, _serverName, "SDP_DB", SystemStatus()),
                              new MonitoredComponent("FXO", ComponentType.Database, _serverName, "FXO_DB", SystemStatus()),
                              new MonitoredComponent("ABT", ComponentType.Database, _serverName, "ABT_DB", SystemStatus()),
                              new MonitoredComponent("DHB", ComponentType.Database, _serverName, "DHB_DB", SystemStatus())
                          }.ToObservable();
            return dbs.Where(_ => _serverName.Contains("DB"));
        }

        private IObservable<SystemStatus> SystemStatus()
        {
            var delay = _rnd.Next(1, 300);
            double delaySec = delay / 10.0;
            return Observable.Return(SystemMonitor.SystemStatus.Online).Delay(TimeSpan.FromSeconds(delaySec));
        }

        #endregion
    }
}