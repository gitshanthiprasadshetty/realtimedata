using System;
using System.ServiceProcess;
using System.ServiceModel;
using CMDataCollector;
using System.Threading;
using SIPDataCollector;
using System.Configuration;

namespace TRealtimeData
{
    public partial class TRealtimeService : ServiceBase
    {
        private static Logger.Logger log = new Logger.Logger(typeof(TRealtimeService));
      
        public TRealtimeService()
        {
            var serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            log.Debug("InitializeComponent()");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("OnStart()");
            try
            {
                log.Debug("Starting TRealtime service.");
                string ret = LoadConfig();
                TRealtimeLogic realTime = new TRealtimeLogic();
                realTime.GetInvokeAndHostType(ret);
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

        private static string LoadConfig()
        {
            try
            {
                string type = ConfigurationManager.AppSettings["Type"];
                return type.ToLower();
            }
            catch (Exception ex)
            {
                log.Error("Exception during TRealtimeService.LoadConfig(): " + ex);
                return "";
            }

        }
        
    }
}
