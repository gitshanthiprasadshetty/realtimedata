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
using Connector.DbLayer;
using System.Data.SqlClient;

namespace SIPDataCollector
{
    public class SIPManager : ISipDataCollector
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

        static readonly Dictionary<string,RealtimeData> summaryInfo = new Dictionary<string, RealtimeData>();

        static List<string> skillExtension = new List<string>();
        static string skillIds = "";
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
                log.Error("Exception in Main", ex);
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
            log.Info("Start");
            try
            {
                // First map Skill-Extension data reading from db, before starting the actual work.
                if (!_isStarted)
                    _instance.MapSkillExtnData();

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
                        log.Info("skill-extn mapping count is zero, after couple of tries. please check if skills are configured correctly. Exiting application.");
                        return;
                    }
                }
                catch (Exception)
                {
                }

                //  sipThread = new Thread(StartSip);      
                var sipThread = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        _instance.FetchBcmsData();
                        Thread.Sleep(Utilites.ConfigurationData.DashboardRefreshTime);
                    }
                }));
                sipThread.Start();

                log.Info("start fetching historical data from historical service.");

                //Thread historicalData = new Thread(new ThreadStart(delegate { GetHistoricalData(); }));
                //historicalData.Start();

                Thread historicalData = new Thread(new ThreadStart(delegate { GetSummaryData(); }));
                historicalData.Start();
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
        public List<RealtimeData> GetBcmsData()
        {
            log.Info("GetBcmsData()");
            try
            {
                return DataCache.GetBcmsData();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetBcmsData() :", ex);
            }
            return null;
        }

        /// <summary>
        /// Gets bcms data for requested skillid
        /// </summary>
        /// <param name="skillId">skillId</param>
        /// <returns>returns bcms data for passed skillid</returns>
        public RealtimeData GetBcmsDataForSkill(string skillId)
        {
            log.Info($"GetBcmsDataForSkill(), skillid = {skillId}");
            try
            {
                return DataCache.GetBcmsDataForSkill(Convert.ToInt32(skillId));
            }
            catch (Exception ex)
            {
                log.Error("Error in GetBcmsDataForSkill() :", ex);
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
                var pulledCacheData = TRealTimeDataSync.SyncManager.PullDataFromCache();
                BcmsDataForSIP serviceObj;
                List<AgentData> agentDataSet;

                foreach (var data in pulledCacheData)
                {
                    agentDataSet = new List<AgentData>();
                    serviceObj = new BcmsDataForSIP();
                    serviceObj.AbandCalls = data.AbandCalls;
                    serviceObj.AccptedSL = Convert.ToInt32(data.AccptedSL);
                    serviceObj.ACD = data.ACD;
                    serviceObj.TotalACDInteractions = data.AcdCallsSummary;
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
                    serviceObj.SLPercentage = data.SL;
                    serviceObj.Staff = data.Staff;
                    serviceObj.Channel = "";
                    serviceObj.Channel = data.SkillName.ToLower().StartsWith("v") ? "voice" : "chat";

                    for (int i = 0; i < data.AgentData.Count(); i++)
                    {
                        log.Debug("pulldata : " + i + "AgentCount " + data.AgentData.Count());
                        agentDataSet.Add(new AgentData
                        {
                            LoginId = data.AgentData[i].LoginId,
                            State = data.AgentData[i].State
                        });
                    }
                    serviceObj.AgentData = agentDataSet;
                    DataCache.UpdateCacheData(serviceObj);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in PullDataFromAlternateServer() :" + ex);
            }
        }

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
                // Load all local config values.
                Utilites.ConfigurationData.LoadConfig();

                // Enable the loop
                _isStarted = true;

                // Initialize agent-{skill} mapping
                AgentSkillInfo();

                // get skillidlist from config to monitor.
                //var skillIdList = Utilites.ConfigurationData.skillList;
                //string skills = string.Join(",", skillIdList);
                //// get extnid for given set of skills
                //var result = DataAccess.GetSkillExtnInfo(skills);
                //if (result != null)
                //{
                //    log.Debug("Skill to Extension mapping");
                //    foreach (DataRow entry in result.Rows)
                //    {
                //        // add to dictionary object to maintain a skill-Extn mapping.
                //        if (_skillExtnInfo.ContainsKey(entry.ItemArray[1].ToString()))
                //            continue;

                //        _skillExtnInfo.Add(entry.ItemArray[1].ToString(), new SkillExtensionInfo
                //        {
                //            SkillId = Convert.ToInt32(entry.ItemArray[0]),
                //            ExtensionId = Convert.ToInt32(entry.ItemArray[1]),
                //            SkillName = Convert.ToString(entry.ItemArray[2]),
                //            Channel = Utilites.ConfigurationData.GetChannel(Convert.ToString(entry.ItemArray[0]))
                //        });
                //    }
                //}
                GetSkillExtensionInfo();

                log.Info("Starting Timer for Config refresh time for SIP");
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = Utilites.ConfigurationData.ReloadConfigTime*60000;
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
                if (_skillExtnInfo.Count() == 0)
                {
                    log.Info("skill-extension count is 0, trying again.");
                    MapSkillExtnData();
                }

                Dictionary<int, RealtimeData> realtimeDataForAllSkills = new Dictionary<int, RealtimeData>();
                try
                {
                    foreach (var server in Utilites.ConfigurationData.TmacServers)
                    {                        
                        log.Info($"Get wallboardskills data from TMAC server {server}");
                        var wallboardSkillsData = GetTmacServerInstance(server).GetTmacWallboardSkills("");
                        if (wallboardSkillsData != null && wallboardSkillsData.Count > 0)
                        {
                            log.Info($"save wallboardskills data from server {server}");
                            foreach (var wallboardSkillInformation in wallboardSkillsData)
                            {
                                if (_skillExtnInfo.ContainsKey(wallboardSkillInformation.SkillID))
                                {
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
                                    realtimeOfSkill.TotalAgentsAvailable += wallboardSkillInformation.AgentAvailable;
                                    realtimeOfSkill.AverageAbandonedTime = 0;
                                    try
                                    {
                                        realtimeOfSkill.AcceptedSL = Convert.ToInt32(Utilites.ConfigurationData.acceptableSlObj?.FirstOrDefault(x => x.Key == realtimeOfSkill.SkillId.ToString()).Value);
                                        realtimeOfSkill.InteractionsInQueue = (realtimeOfSkill.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(wallboardSkillInformation.SkillID)) : wallboardSkillInformation.CallsInQueue;
                                        //realtimeOfSkill.CallsWaiting = (realtimeOfSkill.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(wallboardSkillInformation.SkillID)) : wallboardSkillInformation.CallsInQueue;
                                        realtimeOfSkill.OldestInteractionWaitTime = Convert.ToInt32(WorkQueueProxy.GetOldestWaitTime(wallboardSkillInformation.SkillID));
                                        //realtimeOfSkill.OldestCallWaitTime = WorkQueueProxy.GetOldestWaitTime(wallboardSkillInformation.SkillID);

                                        var agentStats = GetAgentListLoggedInForSkill(server, realtimeOfSkill.SkillId.ToString());
                                        if(realtimeOfSkill.AgentStats == null)
                                            realtimeOfSkill.AgentStats = new List<AgentData>();

                                        realtimeOfSkill.AgentStats.AddRange(agentStats);

                                        if (realtimeOfSkill.AgentStats != null)
                                        {
                                            // log.Debug("Active Interaction : " + wallboardSkillInformation.ActiveInteractions + 1);
                                            realtimeOfSkill.TotalAgentsStaffed = realtimeOfSkill.AgentStats.Count();
                                            realtimeOfSkill.TotalAgentsInACW = realtimeOfSkill.AgentStats.Count(x => x.State.Contains("ACW"));
                                            realtimeOfSkill.TotalAgentsInAUX = realtimeOfSkill.AgentStats.Count(x => Utilites.ConfigurationData.auxCodes.Contains(x.State));
                                        }
                                    }
                                    catch (Exception)
                                    {
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
                            log.Info($"No response from TMAC server instance : {server}");


                        //bcmsdata.Add(data);
                        // update data to cache memory.
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
                DataTable result = null;

                // get agent-skillid data from db[TMAC_Agent_Skills]
                var dbThread = new Thread(new ThreadStart(delegate
                {
                    while (_isStarted)
                    {
                        // Get agent-{skill} info from db.
                        result = DataAccess.GetAgentSkillInfo();
                        if (result != null)
                        {
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
                                    Skills = skillArray.ToList()
                                },
                                (k, v) => new AgentSkillInfo
                                {
                                    AgentId = entry.ItemArray[0].ToString(),
                                    Skills = skillArray.ToList()
                                });
                            }
                            log.Debug("Total Agent-Skill Info dictionary count : " + _agentSkillInfo.Count());
                        }
                        Thread.Sleep(Utilites.ConfigurationData.DashboardRefreshTime);
                    }
                }));
                dbThread.Start();
            }
            catch (Exception ex)
            {
                _isStarted = false;
                log.Error("Error in AgentSkillInfo() :", ex);
            }
        }


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
                            if (!queue.IsEmpty)
                            {
                                skillExtension.Clear();
                                skillIds = "";
                                while (queue.TryDequeue(out string skillExtns))
                                {
                                    skillExtension.Add(skillExtns);
                                    skillIds += _skillExtnInfo[skillExtns].SkillId + ",";
                                }
                            }
                            //if (queue.TryDequeue(out string skillExtn))
                            //{                              
                                //log.Debug("skillExtn is = " + skillExtn);
                                //int skillId = _skillExtnInfo[skillExtn].SkillId;
                                var dbData = DataAccess.GetHistoricalData(skillExtension, skillIds);
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
                                            AbandCalls = Convert.ToInt32(summaryInfo.Where(x => x.Value.AbandonedInteractionsSummary.ToString().Contains(dbData.skillId.ToString()))),
                                            SLPercentage = dbData.SLPercentage,
                                            AvgHandlingTime = ((dbData.ACDTime + dbData.AHTTime) / (dbData.TotalCallsHandled == 0 ? 1 : dbData.TotalCallsHandled)),
                                            //skillId = Convert.ToInt32(_skillExtnInfo[skillExtn].SkillId),
                                            TotalACDInteractions = dbData.TotalACDInteractions,
                                            AbandonPercentage = abandPercentage,
                                            AvgAbandTime = dbData.AvgAbandTime,
                                            AHTTime = dbData.AHTTime,
                                            TotalCallsHandled = dbData.TotalCallsHandled,
                                            Date = dbData.Date,
                                            skillId = dbData.skillId,
                                            AvgSpeedAnswer = dbData.AvgSpeedAnswer,
                                            AvgTalkTime = dbData.AvgTalkTime,
                                            TotalAuxTime = dbData.TotalAuxTime,
                                            FlowIn = dbData.FlowIn,
                                            FlowOut = dbData.FlowOut,
                                            AvgStaffedTime = dbData.AvgStaffedTime,
                                            TotalStaffedTime = dbData.TotalStaffedTime,
                                            CallsHandledWithinThreshold = dbData.CallsHandledWithinThreshold,
                                            CallsAbandAfterThreshold = dbData.CallsAbandAfterThreshold,
                                            PassedCalls = dbData.PassedCalls,
                                            TransferCalls = dbData.TransferCalls,
                                            TotalAbandTime = dbData.TotalAbandTime,
                                            SpeedOfAnswer = dbData.SpeedOfAnswer,
                                            TotalTalkTime = dbData.TotalTalkTime,
                                            TotalStaffedAgents = dbData.TotalStaffedAgents
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
                            //}
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
        /// <summary>
        /// Filters out only Loggedin agents from agent-skill dictionary object for requested skillid.
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>List of agents who are currently loggedin for given skill</returns>
        List<AgentData> GetAgentListLoggedInForSkill(string server , string skillId)
        {
            log.Debug("GetAgentListLoggedInForSkill()");
            try
            {
                List<AgentData> agentListForSkill = new List<AgentData>();
                AgentData agentData;

                // get all currently logged-in agents.
                var loggedInAgents = GetTmacServerInstance(server).GetLoggedInAgentList("");

                // _agentSkillInfo contains all the agentdata from db, get all agents who has requested skillid.
                var agentList = _agentSkillInfo.Where(x => x.Value.Skills.Contains(skillId)).Select(x => x.Key);
                // log.Debug("Total number of agents found for skill : " + skillId + "is "+agentList.Count());

                // Loop through and check amoung list of agents having given skill, how many of them are currently
                // logged-into TMAC. Make a list of these agents and return.
                foreach (var entry in agentList)
                {
                    // for each agentid check against loggedin agents, if found then add to list obj else ignore.
                    var agentDetails = loggedInAgents.FirstOrDefault(y => y.AgentLoginID == entry);
                    if (agentDetails != null)
                    {
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

        public void GetSkillExtensionInfo()
        {
            // get skillidlist from config to monitor.
            var skillIdList = Utilites.ConfigurationData.skillList;
            string skills = string.Join(",", skillIdList);
            // get extnid for given set of skills
            var result = DataAccess.GetSkillExtnInfo(skills);
            _skillExtnInfo.Clear();
            if (result != null)
            {
                log.Debug("Skill to Extension mapping");
                foreach (DataRow entry in result.Rows)
                {
                    // add to dictionary object to maintain a skill-Extn mapping.
                    if (_skillExtnInfo.ContainsKey(entry.ItemArray[1].ToString()))
                        continue;

                    _skillExtnInfo.Add(entry.ItemArray[1].ToString(), new SkillExtensionInfo
                    {
                        SkillId = Convert.ToInt32(entry.ItemArray[0]),
                        ExtensionId = Convert.ToInt32(entry.ItemArray[1]),
                        SkillName = Convert.ToString(entry.ItemArray[2]),
                        Channel = Utilites.ConfigurationData.GetChannel(Convert.ToString(entry.ItemArray[0]))
                    });
                }
            }
        }

        public void GetSummaryData()
        {
            try
            {
                log.Info("GetSummaryData()");
                string skills = string.Join(",", _skillExtnInfo.Keys.ToArray());
                DateTime dt = new DateTime();
                string date = DateTime.Now.ToString("yyyymmdd");
                string startTime = DateTime.Now.ToString("HHmmss");
                string endTime = dt.Date.AddDays(1).AddTicks(-1).ToString("HHmmss");
                SqlConnection conn = new SqlConnection(Utilites.ConfigurationData.ConntnString);
                SqlCommand cmd = new SqlCommand("SELECT AbandCalls,PassedCalls,Skill FROM [dbo].[WorkQueueData](@Date,@StartTime,@EndTime,@Id,@acceptableSL)", conn); 
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@Id", skills);
                cmd.Parameters.AddWithValue("@acceptableSL", 1);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dTable = new DataTable();
                da.Fill(dTable);
                if (dTable != null)
                {
                    for(int i = 0; i < dTable.Rows.Count; i++)
                    {
                        summaryInfo.Add(i.ToString(), new RealtimeData
                        {
                            ActiveInteractions = Convert.ToInt32(dTable.Rows[i][0]),
                            AbandonedInteractionsSummary = Convert.ToInt32(dTable.Rows[i][1]),
                            SkillId= Convert.ToInt32(dTable.Rows[i][2])
                        });
                    }                 
                }
            }
            catch (Exception e)
            {
                log.Error("Exception in GetSummaryData(): " + e);
            }
        }
        #endregion
    }
}
