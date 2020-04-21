using System;
using System.Collections.Generic;
using System.Linq;
using SIPDataCollector.Models;
using System.Collections.Concurrent;
using SIPDataCollector.Utilities;
using System.Data;
using System.Threading;
using SIPDataCollector.Utilites;
using Connector.Proxy;
using AMACWeb_Proxy;
using ConfigurationData = SIPDataCollector.Utilites.ConfigurationData;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace SIPDataCollector
{
    [Route("api/[controller]")]
    public class SIPManager : Controller, ISipDataCollector
    {
        #region Global Declaration

        /// <summary>
        /// Logger
        /// </summary>
        static Logger.Logger log = new Logger.Logger(typeof(SIPManager));

        /// <summary>
        /// TMAC Proxy
        /// </summary>
        // private static readonly ServiceClient tmacServiceClient = new ServiceClient();

        /// <summary>
        /// TMAC Proxy
        /// </summary>
        //static AMACWeb_Proxy.TmacConnector _tmacProxy = new AMACWeb_Proxy.TmacConnector("DefaultTMACServer");
        // static TmacConnector _tmacProxy = null;

        /// <summary>
        /// Stored TmacConnector instances of all servers
        /// </summary>
        private static readonly ConcurrentDictionary<string, AMACWeb_Proxy.TmacConnector> TmacConnectors = new ConcurrentDictionary<string, TmacConnector>();
        //private static readonly ConcurrentDictionary<string, List<DataModel.WallboardSkillModel>> wallboardSkills = new ConcurrentDictionary<string, List<DataModel.WallboardSkillModel>>();
        private static readonly ConcurrentDictionary<string, List<AgentData>> loggedInAgents = new ConcurrentDictionary<string, List<AgentData>>();

        /// <summary>
        /// Holds Skill-Extension Related information for given set of skills in config.
        /// </summary>
        static readonly Dictionary<string, SkillExtensionInfo> _skillExtnInfo = new Dictionary<string, SkillExtensionInfo>();

        /// <summary>
        ///  Holds all Agent-{Skill} Related information.
        /// </summary>
        static readonly ConcurrentDictionary<string, AgentSkillInfo> _agentSkillInfo = new ConcurrentDictionary<string, AgentSkillInfo>();

        /// <summary>
        /// Identity to hold loop value.
        /// </summary>
        public static bool _isStarted = false;

        //bool _sipthreadStatus = false;

        //Thread sipThread;

        ConcurrentDictionary<string, RData> acdInteraction = new ConcurrentDictionary<string, RData>();

        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

        #endregion

        #region Singleton

        /// <summary>
        /// Constructor
        /// </summary>       
        private SIPManager()
        {
            try
            {
                //AMACWeb_Proxy.ConfigurationData.LoadConfig();
                //_tmacProxy = new TmacConnector();
            }
            catch (Exception ex)
            {
                //        //log.Error("Exception in Main", ex);
            }
        }

        /// <summary>
        /// singleton instance of class
        /// </summary>
        private static SIPManager _instance;

        /// <summary>
        /// Get current instance of class/ create new instance.
        /// </summary>
        /// <returns>Returns existing instance or new instance of this class</returns>
        public static SIPManager GetInstance()
        {
            if (_instance == null)
                _instance = new SIPManager();

            return _instance;
        }

        #endregion

        #region Methods

        #region Public

        public void Start()
        {
            log.Info("Start of SIPManager");
            try
            {
                // Load configuration first. 
                ConfigurationData.LoadConfig();

                // First map Skill-Extension data reading from db, before starting the actual work.
                if (!_isStarted)
                    _instance.MapSkillExtnData();

                ConfigurationData.LoadDataFromDatabase();
                // if skill to exten mapping is not happend, try again.
                try
                {
                    if (_skillExtnInfo.Count() == 0)
                    {
                       log.Info("skill-extn mapping count is zero, trying again.");
                       MapSkillExtnData();
                    }

                    if (_skillExtnInfo.Count() == 0)
                    {
                        log.Info("skill-extn mapping count is zero, after couple of tries. Please check if skills are configured correctly. Exiting application.");
                        return;
                    }
                }
                catch (Exception exp)
                {
                    log.Error($"Exception in _skillExtnInfo obj: {exp}");
                }

                var sipThread = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        _instance.FetchBcmsData();
                        Thread.Sleep(Utilites.ConfigurationData.DashboardRefreshTime);
                    }
                }));
                sipThread.Start();

                if (!ConfigurationData.isVDN)
                {
                    //            log.Info("start fetching historical data from SQL function workQueueData");
                    Thread historicalData = new Thread(new ThreadStart(delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(ConfigurationData.DBRefreshTime);
                            ConfigurationData.LoadDataFromDatabaseForInterval();
                        }
                    }
                    ));
                    historicalData.Start();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in Start() : ", ex);
            }
        }


        /// <summary>
        /// Gets List of bcms data related to all given skills in config
        /// </summary>
        /// <returns>returns all bcms data related for given skills in config.</returns>
        [HttpGet]
        [Route("api/TRealtimeData/GetRealtimeDataForAllSkills")]
        public List<RealtimeData> GetBcmsData()
        {
            //    log.Info("GetBcmsData()");
            try
            {
                return DataCache.GetBcmsData();
            }
            catch (Exception ex)
            {
                //        log.Error("Error in GetBcmsData() :", ex);
            }
            return null;
        }

        /// <summary>
        /// Gets bcms data for requested skillid
        /// </summary>
        /// <param name="skillId">skillId</param>
        /// <returns>returns bcms data for passed skillid</returns>
        [HttpGet]
        [Route("api/TRealtimeData/GetRealtimeDataForSkill/{skillId}")]
        public RealtimeData GetBcmsDataForSkill(string skillId)
        {
            //    log.Info($"GetBcmsDataForSkill(), skillid = {skillId}");
            try
            {
                return DataCache.GetBcmsDataForSkill(Convert.ToInt32(skillId));
            }
            catch (Exception ex)
            {
                //        log.Error("Error in GetBcmsDataForSkill() :", ex);
            }
            return null;
        }

        /// <summary>
        /// Pulls data from alternate server if HA is enabled.
        /// </summary>
        public void PullDataFromAlternateServer()
        {
            log.Info($"PullDataFromAlternateServer()");
            try
            {
                //var pulledCacheData = TRealTimeDataSync.SyncManager.PullDataFromCache();
                //BcmsDataForSIP serviceObj;
                //List<AgentData> agentDataSet;

                //foreach (var data in pulledCacheData)
                //{
                //    agentDataSet = new List<AgentData>();
                //    serviceObj = new BcmsDataForSIP();
                //    serviceObj.AbandCalls = data.AbandCalls;
                //    serviceObj.AccptedSL = Convert.ToInt32(data.AccptedSL);
                //    serviceObj.ACD = data.ACD;
                //    serviceObj.TotalACDInteractions = data.AcdCallsSummary;
                //    serviceObj.ACW = data.ACW;
                //    serviceObj.Avail = data.Avail;
                //    serviceObj.AvgAbandTime = data.AvgAbandTime;
                //    serviceObj.CallsWaiting = data.CallsWaiting;
                //    serviceObj.Date = data.Date;
                //    serviceObj.Extn = data.Extn;
                //    serviceObj.OldestCall = data.OldestCall;
                //    serviceObj.Other = data.Other;
                //    serviceObj.Skill = data.Skill;
                //    serviceObj.SkillName = data.SkillName;
                //    serviceObj.SLPercentage = data.SL;
                //    serviceObj.Staff = data.Staff;
                //    serviceObj.Channel = "";
                //    serviceObj.Channel = data.SkillName.ToLower().StartsWith("v") ? "voice" : "chat";

                //    for (int i = 0; i < data.AgentData.Count(); i++)
                //    {
                //        log.Debug("pulldata : " + i + "AgentCount " + data.AgentData.Count());
                //        agentDataSet.Add(new AgentData
                //        {
                //            LoginId = data.AgentData[i].LoginId,
                //            State = data.AgentData[i].State
                //        });
                //    }
                //    serviceObj.AgentData = agentDataSet;
                //    DataCache.UpdateCacheData(serviceObj);
                //}
            }
            catch (Exception ex)
            {
                log.Error("Error in PullDataFromAlternateServer() :" + ex);
            }
        }
        [HttpGet]
        [Route("api/TRealtimeData/GetSkillAndExtensions")]
        public List<SkillExtensionInfo> GetSkillAndExtensions()
        {
            log.Info("GetSkillAndExtensions()");
            try
            {
                return _skillExtnInfo.Values.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetSkillAndExtensions: ", ex);
            }
            return null;
        }

        /// <summary>
        /// Below method is exposed to VDN MOnitor service to send the ACD call information
        /// </summary>
        /// <param name="callid"></param>
        /// <param name="queue"></param>
        /// <param name="queuetime"></param>
        /// <param name="abandontime"></param>
        [HttpGet]
        [Route("/VdnInformation/{callid}/{queue}/{queuetime}/{abandontime}")]
        public void VdnInformation(string callid, string queue, string queuetime, string abandontime)
        {
            log.Info($"VdnInformation() : CallId = {callid}, skillExtension = {queue} , QueueTime = {queuetime} , AbandonTime={abandontime}");
            try
            {
                string skillId = ""; int AcceptedSLConvert = 0;
                DateTime queueDateTime = DateTime.ParseExact(queuetime, "yyyyMMddHHmmss", null);
                DateTime ansOrAbandDateTime = DateTime.ParseExact(abandontime, "yyyyMMddHHmmss", null);
                TimeSpan timeDiff = ansOrAbandDateTime - queueDateTime;
                log.Info($"TimeDifference is {timeDiff.TotalSeconds.ToString()} seconds");
                _skillExtnInfo.TryGetValue(queue, out SkillExtensionInfo value);
                if (value != null)
                {
                    skillId = value.SkillId.ToString();
                    AcceptedSLConvert = Convert.ToInt32(Utilites.ConfigurationData.acceptableSlObj?.FirstOrDefault(x => x.Key == skillId).Value);
                    log.Info($"Accepted SL is {AcceptedSLConvert}");
                    DataCache.UpdateActiveInteraction(Convert.ToInt32(skillId));
                    if (timeDiff.TotalSeconds < AcceptedSLConvert)
                    {
                        log.Info($"Update ASA data to cacheobj for skillId={skillId}");
                        DataCache.UpdateASAData(Convert.ToInt32(skillId));
                    }
                    return;
                }
                log.Info("Skill extension not found in SkillExtnInfo");
                // check status : values will be abandoned or acd
                // if acd then do time-diff to queuedtime and get the ASA, then bind this information to cacheobj for that skillId.
                // if abandoned then no need to calculate the queuedtime, but needs to consider abandoned count and increment the cacheobj count.
            }
            catch (Exception ex)
            {
                log.Error($"Error in VdnInformation: {ex}");
            }
        }

        /// <summary>
        /// Below method is exposed to VDN MOnitor service to send the Abandon call information
        /// </summary>
        /// <param name="callid"></param>
        /// <param name="queue"></param>
        /// <param name="queuetime"></param>
        /// <param name="abandontime"></param>
        [HttpGet]
        [Route("api/TRealtimeData/VdnInformationForAbandon/{callid}/{queue}/{queuetime}/{abandontime}")]
        public void VdnInformationForAbandon(string callid, string queue, string queuetime, string abandontime)
        {
            log.Info($"VdnInformationForAbandon() : CallId = {callid}, skillExtension = {queue} , QueueTime = {queuetime} , AbandonTime={abandontime}");
            try
            {
                string skillId = "";
                _skillExtnInfo.TryGetValue(queue, out SkillExtensionInfo value);
                if (value != null)
                {
                    skillId = value.SkillId.ToString();

                    log.Info($"Update Abandoned count to cacheobj for skillId={skillId}");
                    DataCache.UpdateAbandonedCount(Convert.ToInt32(skillId));
                    return;
                }
                log.Info("Skill extension not found in SkillExtnInfo");

                // check status : values will be abandoned or acd
                // if acd then do time-diff to queuedtime and get the ASA, then bind this information to cacheobj for that skillId.
                // if abandoned then no need to calculate the queuedtime, but needs to consider abandoned count and increment the cacheobj count.
            }
            catch (Exception ex)
            {
                log.Error($"Error in VdnInformation: {ex}");
            }
        }
        #endregion

        #region Private 

        /// <summary>
        /// Maps Extension number to Skillid reffering table[TMAC_SKILLS table]
        /// </summary>
        public void MapSkillExtnData()
        {
            log.Debug("SIPManager[MapSkillExtnData]");
            try
            {
                // Enable the loop
                _isStarted = true;

                // Initialize agent-{skill} mapping
                AgentSkillInfo();

                // get skillidlist from config to monitor.
                var skillIdList = Utilites.ConfigurationData.skillList;
                // get extnid for given set of skills
                // var result = DataAccess.GetSkillExtnInfo(skills);
                var result =  SMSAPIProxy.GetSkills();
                if (result != null)
                {
                    log.Debug("Skill to Extension mapping");
                    foreach (var entry in result)
                    {
                        if (skillIdList.Contains(entry.group_NumberField))
                        {
                            log.Info("skill-id to monitor = " + entry.group_NumberField);
                            // add to dictionary object to maintain a skill-Extn mapping.
                            if (_skillExtnInfo.ContainsKey(entry.group_ExtensionField))
                                continue;

                            _skillExtnInfo.Add(entry.group_ExtensionField, new SkillExtensionInfo
                            {
                                SkillId = Convert.ToInt32(entry.group_NumberField),
                                ExtensionId = Convert.ToInt32(entry.group_ExtensionField),
                                SkillName = Convert.ToString(entry.group_NameField),
                                Channel = ConfigurationData.GetChannel(Convert.ToString(entry.group_NumberField))
                            });
                        }
                    }
                    log.Info($"Skill extension info count {_skillExtnInfo.Count()}");
                }

                log.Info("Starting Timer for Config refresh time for SIP");
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = Utilites.ConfigurationData.ReloadConfigTime;
                timer.Elapsed += Utilites.ConfigurationData.RefreshSection;
                timer.Start();
            }
            catch (Exception ex)
            {
                log.Error("Error in MapSkillExtnData() :", ex);
            }
        }

        /// <summary>
        /// Fetch all required bcms data for all given skillids mentioned in config.
        /// using TMAC proxy and few tables from db.
        /// </summary>
        void FetchBcmsData()
        {
            log.Info("FetchBcmsData()");
            try
            {
                Dictionary<int, RealtimeData> realtimeDataForAllSkills = new Dictionary<int, RealtimeData>();
                try
                {
                    foreach (var server in Utilites.ConfigurationData.TmacServers)
                    {
                        log.Info($"Get wallboardskills data from TMAC server {server}");
                        var wallboardSkillsData = GetTmacServerInstance(server).GetTmacWallboardSkills("");
                        if (wallboardSkillsData != null && wallboardSkillsData.Count() > 0)
                        {
                            log.Info($"save wallboardskills data from server {server}");
                            foreach (var wallboardSkillInformation in wallboardSkillsData)
                            {
                                if (_skillExtnInfo.ContainsKey(wallboardSkillInformation.SkillID))
                                {
                                    log.Info($"_skillExtnInfo obj contains {wallboardSkillInformation.SkillID} skillid in the dictionary. Updating the same");
                                    RealtimeData realtimeOfSkill;
                                    // skillid field that we get from tmac actually is skillextensionid
                                    int skillExtnId = Convert.ToInt32(wallboardSkillInformation.SkillID);

                                    if (realtimeDataForAllSkills.Any(x => x.Key == skillExtnId))
                                    {
                                        realtimeOfSkill = realtimeDataForAllSkills?.FirstOrDefault(x => x.Key == skillExtnId).Value;
                                    }
                                    else
                                        realtimeOfSkill = new RealtimeData();

                                    realtimeOfSkill.SkillId = _skillExtnInfo[wallboardSkillInformation.SkillID].SkillId;
                                    realtimeOfSkill.Channel = Utilites.ConfigurationData.GetChannel(Convert.ToString(realtimeOfSkill.SkillId));
                                    realtimeOfSkill.SkillName = wallboardSkillInformation.SkillName;
                                    realtimeOfSkill.SkillExtensionId = _skillExtnInfo[wallboardSkillInformation.SkillID].ExtensionId;
                                    realtimeOfSkill.ActiveInteractions += wallboardSkillInformation.ActiveInteractions;
                                    //DataCache.UpdateActiveInteraction(wallboardSkillInformation.ActiveInteractions, realtimeOfSkill.SkillId);
                                    realtimeOfSkill.TotalAgentsAvailable += wallboardSkillInformation.AgentAvailable;
                                    realtimeOfSkill.AverageAbandonedTime = 0;
                                    try
                                    {
                                        realtimeOfSkill.AcceptedSL = Convert.ToInt32(Utilites.ConfigurationData.acceptableSlObj?.FirstOrDefault(x => x.Key == realtimeOfSkill.SkillId.ToString()).Value);
                                        realtimeOfSkill.InteractionsInQueue = (realtimeOfSkill.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(wallboardSkillInformation.SkillID)) : wallboardSkillInformation.CallsInQueue;
                                        //realtimeOfSkill.CallsWaiting = (realtimeOfSkill.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(wallboardSkillInformation.SkillID)) : wallboardSkillInformation.CallsInQueue;
                                        realtimeOfSkill.OldestInteractionWaitTime = Convert.ToInt32(WorkQueueProxy.GetOldestWaitTime(wallboardSkillInformation.SkillID));
                                        //realtimeOfSkill.OldestCallWaitTime = WorkQueueProxy.GetOldestWaitTime(wallboardSkillInformation.SkillID);

                                        realtimeOfSkill = DataCache.UpdateFetchBcmsData(realtimeOfSkill);
                                        var agentStats = GetAgentListLoggedInForSkill(server, realtimeOfSkill.SkillExtensionId.ToString());
                                        if (agentStats == null)
                                            log.Info("AgentStats is null");

                                            if (realtimeOfSkill.AgentStats == null)
                                                realtimeOfSkill.AgentStats = new List<AgentData>();

                                        realtimeOfSkill.AgentStats.AddRange(agentStats);

                                        if (realtimeOfSkill.AgentStats != null)
                                        {
                                            log.Info($"TotalAgentsStaffed is {realtimeOfSkill.AgentStats.Count()}");
                                            // log.Debug("Active Interaction : " + wallboardSkillInformation.ActiveInteractions + 1);
                                            realtimeOfSkill.TotalAgentsStaffed = realtimeOfSkill.AgentStats.Count();
                                            realtimeOfSkill.TotalAgentsInACW = realtimeOfSkill.AgentStats.Count(x => x.State.Contains("ACW"));
                                            realtimeOfSkill.TotalAgentsInAUX = realtimeOfSkill.AgentStats.Count(x => Utilites.ConfigurationData.auxCodes.Contains(x.State));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error($"Exception while getting/updating agent stats in FetchBCMSData: {ex}");
                                    }

                                    if (realtimeDataForAllSkills.Any(skill => skill.Key == realtimeOfSkill.SkillExtensionId))
                                        realtimeDataForAllSkills.Remove(realtimeOfSkill.SkillExtensionId);

                                    realtimeDataForAllSkills.Add(realtimeOfSkill.SkillExtensionId, realtimeOfSkill);

                                    if (!queue.Contains(wallboardSkillInformation.SkillID))
                                        queue.Enqueue(wallboardSkillInformation.SkillID);
                                }
                            }
                        }
                        else
                        {   log.Info($"No response from TMAC server instance : {server}");
                        }
                        List<RealtimeData> valuesToAddToCacheMem = realtimeDataForAllSkills?.Values.ToList();
                        DataCache.UpdateCacheData(valuesToAddToCacheMem);
                    }
                }
                catch (Exception exception)
                {
                    log.Error("Error in FetchBcmsData: ", exception);
                }

                #region oldcode
                /*
                if (wallboardSkills != null)
                {
                    log.Debug("Total count from GetTmacWallboardSkills : " + res.Count());
                    foreach (var wallboard in wallboardSkills.)
                    {                        
                        // if current EXTENSIONID is present in _skillExtnInfo dictionary object, then take a count else ignore. 
                        // here SkillID obtained from _tmacProxy is actually EXTENSIONID.
                        if (_skillExtnInfo.ContainsKey(wallboard.SkillID))
                        {
                            log.Debug("Extension-Id : " + wallboard.SkillID + " is found in _skillExtnInfo dictionary object");
                            data = new RealtimeData();
                            // here model name skill is actually extnid,so get actual skillid from _skillExtnInfo object
                            data.SkillId = _skillExtnInfo[wallboard.SkillID].SkillId;                         
                            data.Channel = Utilites.ConfigurationData.GetChannel(Convert.ToString(data.SkillId));
                            data.SkillName = wallboard.SkillName;
                            try
                            {
                                // data.InteractionsInQueue = (data.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(entry.SkillID)) : entry.CallsInQueue;
                                data.CallsWaiting = (data.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(wallboard.SkillID)) : wallboard.CallsInQueue;
                                // data.OldestInteractionWaitTime = Convert.ToInt32(WorkQueueProxy.GetOldestWaitTime(entry.SkillID));
                                data.OldestCallWaitTime = WorkQueueProxy.GetOldestWaitTime(wallboard.SkillID);

                            }
                            catch (Exception ex)
                            {
                                log.Error("Error while reading workqueue data : " + ex);
                            }


                            data.SkillExtensionId = _skillExtnInfo[wallboard.SkillID].ExtensionId;
                       
                            // get the agentdata[Total agents logged-in for a skill] for given skillId
                            try
                            {
                                //   data.AgentStats = _instance.GetAgentListLoggedInForSkill(data.SkillId.ToString());
                                data.AgentData = _instance.GetAgentListLoggedInForSkill(data.SkillId.ToString());
                                if (data.AgentData != null)
                                {                                 
                                    log.Debug("Active Interaction : " + wallboard.ActiveInteractions + 1);
                                    data.TotalAgentsStaffed = data.AgentData.Count();
                                    data.TotalAgentsInACW = data.AgentData.Count(x => x.State.Contains("ACW"));
                                    data.TotalAgentsInAUX = data.AgentData.Count(x => Utilites.ConfigurationData.auxCodes.Contains(x.State));
                                }
                            }
                            catch (Exception)
                            {
                                log.Error("Error while calling tmac method : GetAgentListLoggedInForSkill() for skill = " + data.SkillId);
                            }
     
                            data.ActiveCalls = wallboard.ActiveInteractions;
                            data.TotalAvailableAgents = wallboard.AgentAvailable;
                            data.AverageAbandonedTime = "00:00:00";


                            try
                            {
                                data.AcceptedSL = Convert.ToInt32(Utilites.ConfigurationData.acceptableSlObj?.FirstOrDefault(x => x.Key == data.SkillId.ToString()).Value);
                            }
                            catch (Exception)
                            {
                            }

                            //bcmsdata.Add(data);
                            // update data to cache memory.
                            DataCache.UpdateCacheData(data);

                            if (!queue.Contains(wallboard.SkillID))
                                queue.Enqueue(wallboard.SkillID);
                        }
                    }
                }
                else
                    log.Info($"No response from TMAC");
                    */
                #endregion
            }
            catch (Exception ex)
            {
                log.Error("Error in FetchBcmsData() :", ex);
            }
        }

        /// <summary>
        /// Creats Agent-skill map list and keeps in sync with db.
        /// </summary>
        void AgentSkillInfo()
        {
            log.Debug("AgentSkillInfo()");
            try
            {
                if (ConfigurationData.GetAgentSkillInfoConfig.ToLower() == "db")
                {
                    log.Info("Getting agent skill info from DB TMAC table");
                    DataTable result = null;

                    // get agent-skillid data from db[TMAC_Agent_Skills]
                    var dbThread = new Thread(new ThreadStart(delegate
                    {
                        while (_isStarted)
                        {
                            try
                            {
                                // Get agent-{skill} info from db.
                                result = DataAccess.GetAgentSkillInfo();
                                if (result != null)
                                {
                                    log.Info("Got data from db table");
                                    foreach (DataRow entry in result.Rows)
                                    {
                                        // since we get data from db for every 5min and getAgentloggedinlist method cant be synced together for 5min interval
                                        // consider all agentdata , filter can be done later in below GetAgentListLoggedInForSkill method.

                                        var value = entry.ItemArray[1].ToString();
                                        string[] skillArray = value.Split(',');
                                        //log.Debug("Entry data [0]: " + entry.ItemArray[0].ToString());
                                        //log.Debug("Entry data [1]: " + entry.ItemArray[1].ToString());

                                        _agentSkillInfo.AddOrUpdate(entry.ItemArray[0].ToString(), new AgentSkillInfo
                                        {
                                            AgentId = entry.ItemArray[0].ToString(),
                                            SkillExtension = skillArray.ToList()
                                        },
                                        (k, v) => new AgentSkillInfo
                                        {
                                            AgentId = entry.ItemArray[0].ToString(),
                                            SkillExtension = skillArray.ToList()
                                        });
                                    }
                                    log.Debug("Total Agent-Skill Info dictionary count : " + _agentSkillInfo?.Count());
                                }
                            }
                            catch (Exception e)
                            {
                                log.Error($"Exception while updating agentSkillInfo: { e}");
                            }
                            Thread.Sleep(Utilites.ConfigurationData.DBRefreshTime);
                        }
                    }));
                    dbThread.Start();
                }
                else
                {
                    try
                    {
                        log.Info("Getting agent skill info from TMAC GetAgentSessionsList method");
                        List<string> server = ConfigurationData.TmacServers;
                        List<DataModel.AgentSessionDataModel> agentSkillInfo = new List<DataModel.AgentSessionDataModel>();
                        var tmacThread = new Thread(new ThreadStart(delegate
                        {
                            while (_isStarted)
                            {
                                try
                                {
                                    log.Info("Fetching Agent-Skill info from GetAgentSessionsList()");
                                    foreach (var item in server)
                                    {
                                        try
                                        {
                                            agentSkillInfo = GetTmacServerInstance(item).GetAgentSessionsList("", "", "", "", item);
                                        }
                                        catch (Exception e)
                                        {
                                            log.Warn($"TMAC Server is unreachable or agentSkillInfo is null");
                                        }
                                    }
                                    if (agentSkillInfo != null)
                                    {
                                        if (agentSkillInfo.Count > 0)
                                        {
                                            log.Info($"AgentSkillInfo from GetAgentSessionList has data of agent skill info");
                                            foreach (var data in agentSkillInfo)
                                            {
                                                _agentSkillInfo.AddOrUpdate(data.AgentLoginID, new AgentSkillInfo
                                                {
                                                    AgentId = data.AgentLoginID,
                                                    SkillExtension = data.AgentVoiceSkillsAsString.Split(',').ToList()
                                                },
                                                (k, v) => new AgentSkillInfo
                                                {
                                                    AgentId = data.AgentLoginID,
                                                    SkillExtension = data.AgentVoiceSkillsAsString.Split(',').ToList()
                                                });
                                            }

                                            //DataCache.UpdateCacheObj(agentSkillInfo);
                                            log.Debug("Total Agent-Skill Info dictionary count : " + _agentSkillInfo?.Count());
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Error while updating agent skill info object: {e}");
                                }
                                Thread.Sleep(Utilites.ConfigurationData.DashboardRefreshTime);
                            }
                        }));
                        tmacThread.Start();
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exception in AgentSkillInfo GetAgentSessionsList method {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                //_isStarted = false;
                log.Error("Error in AgentSkillInfo() :", ex);
            }
        }

        /*
        public void GetHistoricalData()
        {
            log.Debug("GetHistoricalData()");
            try
            {
                while (true)
                {
                    if (!queue.IsEmpty)
                    {
                        log.Debug("Queue is not empty");
                        foreach (var item in queue)
                        {
                            log.Debug("Queue item is = " + item);
                            if (queue.TryDequeue(out string skillExtn))
                            {
                                log.Debug("skillExtn is = " + skillExtn);
                                int skillId = _skillExtnInfo[skillExtn].SkillId;
                                var dbData = DataAccess.GetHistoricalData(skillExtn, skillId);
                                if (dbData != null)
                                {
                                    log.Debug("Received historical data");
                                    try
                                    {
                                        decimal abandPercentage = Math.Round(Convert.ToDecimal(100 * Convert.ToDouble(dbData.AbandCalls) / ((dbData.AbandCalls) + (dbData.TotalACDInteractions == 0 ? 1 : dbData.TotalACDInteractions))), 2);

                                        SkillData data = new SkillData
                                        {
                                            ACDTime = dbData.ACDTime,
                                            ACWTime = dbData.ACWTime,
                                            AbandCalls = dbData.AbandCalls,
                                            SLPercentage = dbData.SLPercentage,
                                            AvgHandlingTime = ((dbData.ACDTime + dbData.AHTTime) / (dbData.TotalCallsHandled == 0 ? 1 : dbData.TotalCallsHandled)),
                                            skillId = Convert.ToInt32(_skillExtnInfo[skillExtn].SkillId),
                                            TotalACDInteractions = dbData.TotalACDInteractions,
                                            AbandonPercentage = abandPercentage,
                                            AvgAbandTime = dbData.AvgAbandTime
                                        };
                                        // if (!string.IsNullOrEmpty(data.skillId) && (data != null))
                                        if (data != null)
                                            DataCache.UpdateHistoricalData(data);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Error while processing histoircal data : ", ex);
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(Utilites.ConfigurationData.DBRefreshTime);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in GetHistoricalData", ex);
            }
        }
        */

        /// <summary>
        /// Filters out only Loggedin agents from agent-skill dictionary object for requested skillid.
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>List of agents who are currently loggedin for given skill</returns>
        List<AgentData> GetAgentListLoggedInForSkill(string server, string skillExtension)
        {
            log.Debug($"GetAgentListLoggedInForSkill(): Server {server}, skillExtension {skillExtension}");
            try
            {
                List<AgentData> agentListForSkill = new List<AgentData>();
                AgentData agentData;

                // get all currently logged-in agents.

                var loggedInAgents = GetTmacServerInstance(server).GetLoggedInAgentList("");


                // _agentSkillInfo contains all the agentdata from db, get all agents who has requested skillid.
                var agentList = _agentSkillInfo?.Where(x => x.Value.SkillExtension.Contains(skillExtension)).Select(x => x.Key);
                // log.Debug("Total number of agents found for skill : " + skillId + "is "+agentList.Count());

                // Loop through and check amoung list of agents having given skill, how many of them are currently
                // logged-into TMAC. Make a list of these agents and return.
                foreach (var entry in agentList)
                {
                    // for each agentid check against loggedin agents, if found then add to list obj else ignore.
                    var agentDetails = loggedInAgents.FirstOrDefault(y => y.AgentLoginID == entry);
                    if (agentDetails != null)
                    {
                        log.Info($"Updating AgentDetails stats LoginID {entry}, State {agentDetails.CurrentAgentStatus}, StationID {agentDetails.StationID}");
                        agentData = new AgentData { LoginId = entry, State = agentDetails.CurrentAgentStatus, StationID = agentDetails.StationID };
                        agentListForSkill.Add(agentData);
                    }
                }
                return agentListForSkill;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetAgentListLoggedInForSkill() :" + ex);
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Gets the TMAC connector instance for a TMAC Server
        /// </summary>
        /// <param name="tmacServer">TMAC Server Name</param>
        /// <returns><see cref="TmacConnector"/></returns>
        public static TmacConnector GetTmacServerInstance(string tmacServer)
        {
            try
            {
                log.Info("Default Tmac  Server is: " + tmacServer);
                if (!string.IsNullOrWhiteSpace(tmacServer))
                {
                    if (!TmacConnectors.ContainsKey(tmacServer))
                    {
                        TmacConnectors.TryAdd(tmacServer, new TmacConnector(tmacServer));
                    }
                    return TmacConnectors[tmacServer];
                }
                log.Error("[GetTmacServerInstance]: Server name null or empty.");
                return null;
            }
            catch (Exception ex)
            {
                log.Error("Exception in GetTamcServerInstance: " + ex);
                return null;
            }
        }

        /// <summary>
        /// The below method gets the abandcalls, acdcalls and skillid from the function WorkQueueData and updates to cacheobj
        /// </summary>
        public void GetSummaryData(DateTime endDateTime, string date, string startTime, string endTime)
        {
            try
            {
                log.Info($"GetSummaryData() ");
                if (false)
                {
                    DataTable dTable = new DataTable();
                    string skills = string.Join(",", _skillExtnInfo.Keys.ToArray());

                    log.Info($"GetSummaryData() from WorkQueueData date:{date}, startTime:{startTime},endTime:{endTime},skills:{skills}");
                    string connString = ConfigurationData.ConntnString;

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        SqlCommand cmd = new SqlCommand("SELECT AbandCalls,PassedCalls,Skill FROM [dbo].[WorkQueueData](@Date,@StartTime,@EndTime,@Id,@acceptableSL)", conn);
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@Id", skills);
                        cmd.Parameters.AddWithValue("@acceptableSL", 1); //acceptableSL is not used inside this function so passing it as 1 - 09 Dec
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dTable);
                    }
                    DataCache.UpdateSummaryData(dTable);
                    ConfigurationData.LastExecutedTime = endDateTime;
                }
                else
                {
                    int sla = ConfigurationData.acceptableSL;
                    DataTable dTable = new DataTable();
                    string skills = string.Join(",", _skillExtnInfo.Keys.ToArray());

                    log.Info($"GetSummaryData() from GetRealtimeData SP date:{date}, startTime:{startTime},endTime:{endTime},skills:{skills}");
                    string connString = ConfigurationData.ConntnString;

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand cmd = new SqlCommand("[GetRealtimeData]", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@startdate", SqlDbType.VarChar).Value = date + startTime;
                            cmd.Parameters.Add("@enddate", SqlDbType.VarChar).Value = date + endTime;
                            cmd.Parameters.Add("@acceptablesla", SqlDbType.VarChar).Value = sla;

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dTable);
                        }
                    }
                    DataCache.UpdateSummaryData(dTable);
                    ConfigurationData.LastExecutedTime = endDateTime;
                }
            }
            catch (Exception e)
            {
                log.Error("Exception in GetSummaryData(): " + e);
            }
        }

        public int GetSkillExtensionInfo(object skillExtension)
        {
            try
            {
                log.Info($"GetSkillExtensionInfo: {skillExtension.ToString()}");
                _skillExtnInfo.TryGetValue(skillExtension.ToString(), out SkillExtensionInfo value);
                if (value != null)
                {
                    log.Info($"Returning skillId {value.SkillId} for {skillExtension.ToString()}");
                    return value.SkillId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetSkillExtensionInfo(): {ex}");
                return 0;
            }

        }
        #endregion
    }
}
