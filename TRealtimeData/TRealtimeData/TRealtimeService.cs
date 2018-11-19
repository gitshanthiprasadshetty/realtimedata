using System;
using System.ServiceProcess;
using System.ServiceModel;
using CMDataCollector;
using System.Threading;

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
                log.Debug("Starting TRealtime service.");
                myServiceHost.Open();
                Thread startThread = new Thread(new ThreadStart(delegate { BCMSDashboardManager.Start(); })); // new Thread(new ThreadStart(delegate{ GetHistoricalData(); }));
                startThread.Start();
                //BCMSDashboardManager.Start();
                log.Debug("TRealtime Service is started");
            }
            catch (Exception ex)
            {
                log.Error("TRealtime Service OnStart : " + ex);
            }
        }

        protected override void OnStop()
        {
            log.Debug("TRealtime Service Stopped");
        }
    }
}
