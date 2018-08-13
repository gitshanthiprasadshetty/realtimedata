using System;
using System.Collections.Generic;
using System.Linq;
using CMDataCollector.Models;
using CMDataCollector.Utilities;
using SIPDataCollector;

namespace CMDataCollector
{
    public class BCMSDashboardManager
    {
        /// <summary>
        /// Logger 
        /// </summary>
        static Logger.Logger Log = new Logger.Logger(typeof(BCMSDashboardManager));

        /// <summary>
        /// Holds value of Alternate service status[for HA]
        /// </summary>
        static bool _isServiceRunning;

        public static void Start()
        {
            try
            {
                Log.Debug("Start()");

                // Load all the config values from app.config
                ConfigurationData.LoadConfig();

                if (ConfigurationData.HAEnabled != 1)
                {
                    // if HA is not enabled establish the connection to CM directly.
                    if (ConfigurationData.ConnectionType.ToLower() == "cm")
                        CMConnectionManager.GetInstance().Start();
                }

                // If HA enabled then decide wether to connect to CM or just start the service without connecting to CM.
                // When one server is connected to CM other/alternate server will just pull data from this server.
                if (ConfigurationData.HAEnabled == 1)
                {
                    // Start Data sync-manager
                   // TRealTimeDataSync.SyncManager.Start();
                    // Declare Alternateserverstatus event, this will be triggered for every 10sec.
                    // TRealTimeDataSync.SyncManager.AlternateServerStatus += new TRealTimeDataSync.SyncManager.ServerStatus(OnStatusChange);

                    if (ConfigurationData.ConnectionType.ToLower() == "cm")
                        CMConnectionManager.GetInstance().connectToCM = false;                  
                }

                // If connection-type is SIP then get realtime data from Tmac service and database.
                // Irrespective of HA status start sip if it's enabled. If HA is enabled then OnStatusChange method will have data for SIP.
                if (ConfigurationData.ConnectionType.ToLower() == "sip")
                    SIPManager.GetInstance().Start();
            }
            catch (Exception ex)
            {
                Log.Error("Error in BCMSDashboardManager[Start] :" + ex);
            }
        }

        /// <summary>
        /// On stop of winservice this method is called.
        /// </summary>
        public static void Stop()
        {
            try
            {
                // on stop connecting to CM flag 'connectToCM' is changed to false.
                Log.Debug("BCMSDashboardManager[Stop]");
                CMConnectionManager.GetInstance().connectToCM = false;
            }
            catch (Exception ex)
            {
                Log.Error("Error in BCMSDashboardManager[Stop] :" + ex);
            }
        }

        /// <summary>
        /// An event, which checks status of alternate server for every 10sec and do neccessary action 
        /// based on event output.
        /// </summary>
        static void OnStatusChange()
        {
            /*
            try
            {
                Log.Debug("BCMSDashboardManager[OnStatusChange]");

                // this property of SyncManager class will have current status of alternate service.
                // for each x interval[implemented in syncmanager class] status of other server is read and the value is obtained here using below property.
                _isServiceRunning = TRealTimeDataSync.SyncManager.IsAlternateServiceRunning;

                // false: if alterante service is down, Establish a connection to CM from this service if it's not connected before.
                if (!_isServiceRunning)
                {
                    Log.Debug("Alternate Service is down.");

                    if (ConfigurationData.ConnectionType.ToLower() == "cm")
                    {
                        // If alternate service is down,
                        // check the status of this service with resp to connection to CM, if its not connected then establish a connection to CM.
                        // else do nothing => this service is already connected to CM and is pulling data.
                        if (!CMConnectionManager.GetInstance().connectToCM)
                        {
                            // after connecting to CM flag 'connectToCM' is changed to true in CMConnectionManager class.
                            Log.Debug("Trying to connect to CM");
                            CMConnectionManager.GetInstance().Start();
                        }
                        else
                            Log.Debug("This Service is already connected to CM.");
                    }

                    if (ConfigurationData.ConnectionType.ToLower() == "sip")
                    {
                        Log.Debug("Trying to connect...");
                        SIPManager.GetInstance().Start();
                    }
                }

                // FOR CM : if alternate service is already running, two possibilites :
                // 1. Alternate service is running and already connected to CM : In this case dont connect this service to CM, 
                //    instead just keep pulling data from alternate service.
                // 2. Alternate service is running and not connected to CM : Implies this service is already connected to CM, hence do nothing.
                // FOR SIP : if alternate service is already running then keep pulling data.
                //           if alternate service is not running then start this service.
                if (_isServiceRunning)
                {
                    Log.Debug("Alternate Service is up and running.");
                    if (ConfigurationData.ConnectionType.ToLower() == "cm")
                    {
                        // if current service is running and not connected to cm, get data from other service and update to cache mem
                        if (!CMConnectionManager.GetInstance().connectToCM)
                        {
                            Log.Debug("This Service is not connected to CM, Pull data from alternate service.");
                            GetDataFromAlternateServer();
                        }
                        else
                            Log.Debug("This Service is already connected to CM, dont pull data from alternate server.");
                    }


                    if (ConfigurationData.ConnectionType.ToLower() == "sip")
                        SIPManager.GetInstance().PullDataFromAlternateServer();
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error in BCMSDashboardManager[OnStatusChange] :" + ex);
            }
            */
        }

        /// <summary>
        /// Pulls data from alternate server's cache memory and stores in its cache memory.
        /// </summary>
        static void GetDataFromAlternateServer()
        {
            Log.Debug("BCMSDashboardManager[GetDataFromAlternateServer]");
            //try
            //{
                /*
                // pull bcms data from other server
                var pulledCacheData = TRealTimeDataSync.SyncManager.PullDataFromCache();
                BcmsDashboard serviceObj;
                List<AgentData> agentDataSet;

                // frame BcmsDashboard model object for each array of data received
                // and update it to cache object.
                foreach (var data in pulledCacheData)
                {
                    agentDataSet = new List<AgentData>();
                    serviceObj = new BcmsDashboard();
                    serviceObj.AbandCalls = data.AbandCalls;
                    serviceObj.AccptedSL = data.AccptedSL;
                    serviceObj.ACD = data.ACD;
                    serviceObj.AcdCallsSummary = data.AcdCallsSummary;
                    serviceObj.ACW = data.ACW;
                    serviceObj.Avail = data.Avail;
                    serviceObj.AvgAbandTime = data.AvgAbandTime;
                    serviceObj.CallsWaiting = data.CallsWaiting;
                    serviceObj.Date = data.Date;
                    serviceObj.Extn = data.Extn;
                    serviceObj.OldestCall = data.OldestCall;
                    serviceObj.Other = data.Other;
                    serviceObj.Skill = data.Skill;
                    serviceObj.SkillName = data.SkillName;
                    serviceObj.SL = data.SL;
                    serviceObj.Staff = data.Staff;
                    serviceObj.Channel = ConfigurationData.GetChannel(data.Skill);

                    if (data.AgentData != null)
                    {
                        for (int i = 0; i < data.AgentData.Count(); i++)
                        {
                            Log.Debug("pulldata : " + i + "AgentCount " + data.AgentData.Count());
                            agentDataSet.Add(new AgentData
                            {
                                LoginId = data.AgentData[i].LoginId,
                                State = data.AgentData[i].State
                            });
                        }
                    }
                    serviceObj.AgentData = agentDataSet;
                    CacheMemory.AddToCacheMemory(serviceObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in BCMSDashboardManager[GetDataFromAlternateServer] :" + ex);
            }
            */
        }

        #region NotUsed

        /// <summary>
        /// This is first method called when service is started.
        /// </summary>
        //public static void Start()
        //{
        //    try
        //    {
        //        Log.Debug("BCMSDashboardManager[Start]");

        //        // Load all the config values from app.config
        //        ConfigurationData.LoadConfig();

        //        // Check for connection type[CM or SIP]
        //        if (ConfigurationData.ConnectionType.ToLower() == "cm")
        //        {
        //            Log.Debug("Connection Type is CM");

        //            // if HA is not enabled establish the connection to cm directly.
        //            if (ConfigurationData.HAEnabled != 1)
        //                CMConnectionManager.GetInstance().Start();

        //            // if HA enabled then decide wether to connect to cm or just start the service without connecting to cm.
        //            // When one server is connected to CM other/alternate server will just pull data from this server.
        //            if (ConfigurationData.HAEnabled == 1)
        //            {
        //                Log.Debug("HA is enabled, try monitoring status of other server");

        //                // initially the connection to cm should be false, if alternate service is not running then turn on the connection to CM.
        //                CMConnectionManager.GetInstance().connectToCM = false;

        //                // Start Data sync-manager
        //                BcmsDataSync.SyncManager.Start();

        //                // Declare Alternateserverstatus event, this will be triggered for every 10sec.
        //                BcmsDataSync.SyncManager.AlternateServerStatus += new BcmsDataSync.SyncManager.ServerStatus(OnStatusChange);

        //                // this property of DataSyncManager class will have current status of alternate service.
        //                //  _isServiceRunning = BcmsDataSync.SyncManager.IsAlternateServiceRunning;       
        //            }
        //        }

        //        // if connection-type is SIP then get realtime data from Tmac service and database.
        //        if (ConfigurationData.ConnectionType.ToLower() == "sip")
        //        {
        //            Log.Debug("Connection Type is SIP");
        //            if (ConfigurationData.HAEnabled == 1)
        //            {
        //                BcmsDataSync.SyncManager.Start();
        //                BcmsDataSync.SyncManager.AlternateServerStatus += new BcmsDataSync.SyncManager.ServerStatus(OnStatusChange);
        //            }
        //            StartSIP();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Error in BCMSDashboardManager[Start] :" + ex);
        //    }
        //}

        #endregion
    }
}
