using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TRealTimeDataSync
{
    public class SyncManager
    {
        #region Global Level Declaration

        /// <summary>
        /// Logger
        /// </summary>
        static Logger.Logger Log = new Logger.Logger(typeof(SyncManager));

        /// <summary>
        /// Event to trigger when alternate service goes up/down.
        /// </summary>
        public delegate void ServerStatus();
        public static event ServerStatus AlternateServerStatus;       

        /// <summary>
        /// Holds the current state of alternate server/service status.
        /// </summary>
        public static bool IsAlternateServiceRunning { get; set; }

        /// <summary>
        /// bool variable to define loop
        /// </summary>
        static bool _started;

        /// <summary>
        /// Alternate service object
        /// </summary>
        static HB_TRealtimeData.CMDataService _service = new HB_TRealtimeData.CMDataService();

        #endregion

        #region Methods

        /// <summary>
        /// Start Bcms DataSync manager when HA is enabled.
        /// </summary>
        public static void Start()
        {
            try
            {
                Log.Debug("SyncManager[Start]");
                _started = true;
                IsAlternateServiceRunning = false;
                Thread ValidateHB = new Thread(new ThreadStart(HeartBeatStatus));
                ValidateHB.Start();
            }
            catch (Exception ex)
            {
                Log.Error("Exception SyncManager[Start] :"+ ex);
            }
        }

        /// <summary>
        /// Decides the status of Alternate server.
        /// </summary>
        static void HeartBeatStatus()
        {
            Log.Debug("SyncManager[HeartBeatStatus]");

            bool isServiceRunning = false;
            bool isRunningResultSpecified = false;

            // keep checking the status of alternate server.
            while (_started)
            {
                try
                {
                    // gets the boolean output of alternate service status.
                    _service.IsRunning(out isServiceRunning, out isRunningResultSpecified);

                    // if alternate service is up and running, do data sync
                    if (isServiceRunning)
                        IsAlternateServiceRunning = true;

                    // if alternate service is not running
                    if (!isServiceRunning)
                        IsAlternateServiceRunning = false;

                    // call event irrespective of output, either service running or stopped.
                    if(AlternateServerStatus !=null)
                        AlternateServerStatus();                    
                }
                catch (Exception ex)
                {
                    // on exceptional cases assume alternate server is not reachable and establish this
                    // service connection to CM by raising a event, if not established before.
                    IsAlternateServiceRunning = false;
                    if (AlternateServerStatus != null)
                        AlternateServerStatus(); 

                    Log.Error("Exception in HeartBeatStatus while checking alternate service status :"+ ex);
                }
                // sleep for 10seconds.
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Pulls the bcms data from alternate server.
        /// </summary>
        /// <returns>returns a list of bcms data</returns>
        public static List<TRealTimeDataSync.HB_TRealtimeData.BcmsDashboard> PullDataFromCache()
        {
            Log.Debug("SyncManager[PullDataFromCache]");
            try
            {                
                return _service.MonitorBcms().ToList();              
            }
            catch (Exception ex)
            {
                Log.Error("Exception in SyncManager[PullDataFromCache] :" + ex);
                return null;
            }
        }

        #endregion
    }
}
