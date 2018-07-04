using System;
using System.ServiceProcess;
using System.ServiceModel;
using CMDataCollector;

namespace TRealtimeData
{
    public partial class TRealtimeService : ServiceBase
    {
        private static Logger.Logger log = new Logger.Logger(typeof(TRealtimeService));

        public TRealtimeService()
        {
            log.Debug("InitializeComponent()");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("OnStart()");
            ServiceHost myServiceHost = new ServiceHost(typeof(CMDataService));
            try
            {
                myServiceHost.Open();
                BCMSDashboardManager.Start();
                log.Debug("BCMS Service is started");
            }
            catch (Exception ex)
            {
                log.Error("BCMS Service OnStart : " + ex);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
