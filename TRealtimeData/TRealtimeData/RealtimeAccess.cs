using CMDataCollector;
using SIPDataCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TRealtimeData
{
    class RealtimeAccess : ITRealtimeInterface
    {
        private static Logger.Logger log = new Logger.Logger(typeof(RealtimeAccess));
        public RealtimeAccess()
        {
        }
        //the below method is called from logic which starts the cmdataservice or sipmanager based on the configuration - sep 9/19
        public void GetInvokeAndHostType(string id)
        {
            try
            {
                log.Info("GetInvokeType(): " + id);
                ServiceHost myServiceHost;
                if (id.ToLower() == "cm")
                {
                    log.Info("Starting CMDataCollector");
                    myServiceHost = new ServiceHost(typeof(CMDataService));
                    BCMSDashboardManager.Start();
                }
                else
                {
                    log.Info("Starting SIPDataCollector");
                    myServiceHost = new ServiceHost(typeof(SIPManager));
                    SIPManager.GetInstance().Start();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception in GetInvokeAndHostType(): " + e);
            }
            
        }        
    }

    //below class is added to create class object which calls the factory methods
    public class TRealtimeLogic
    {
        private static Logger.Logger log = new Logger.Logger(typeof(TRealtimeLogic));
        ITRealtimeInterface _realTime;
        public TRealtimeLogic()
        {
            _realTime = RealtimeFactory.GetDataAccessObj();
        }
        public void GetInvokeAndHostType(string type)
        {
             _realTime.GetInvokeAndHostType(type);
        }
    }
}
