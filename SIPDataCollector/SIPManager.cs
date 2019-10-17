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
        static TmacProxyRest _tmacProxy = null;

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
                AMACWeb_Proxy.ConfigurationData.LoadConfig();
                _tmacProxy = new TmacProxyRest();
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

                Thread historicalData = new Thread(new ThreadStart(delegate{ GetHistoricalData(); }));
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
                log.Error("Error in GetBcmsDataForSkill() :" , ex);
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
                var skillIdList = Utilites.ConfigurationData.skillList;
                string skills = string.Join(",", skillIdList);
                // get extnid for given set of skills
                var result = DataAccess.GetSkillExtnInfo(skills);
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
            catch (Exception ex)
            {
                log.Error("Error in MapSkillExtnData() :" , ex);
            }
        }

        /// <summary>
        /// Fetch all required bcms data for all given skillids mentioned in config.
        /// using TMAC proxy and few tables from db.
        /// </summary>
        void FetchBcmsData()
        {
            try
            {
                log.Debug("FetchBcmsData()");

                // get the skills from config to be monitored.
                // string[] skillIdList = ConfigurationData.skillList;

                // List<BcmsDataForSIP> bcmsdata = new List<BcmsDataForSIP>();
                RealtimeData data;

                // retruns list of data
                // var getActiveInteractions = _tmacProxy.Stat_GetSkillData();
                // Gives wallboard data for all("") skills.
                var res = _tmacProxy.GetTmacWallboardSkills("");

                if (res != null)
                {
                    log.Debug("Total count from GetTmacWallboardSkills : " + res.Count());
                    foreach (var entry in res)
                    {
                        // if skill to exten mapping is not happend, try again.
                        if (_skillExtnInfo.Count() == 0)
                        {
                            log.Info("_skillExtnInfo count is 0");
                            MapSkillExtnData();
                        }

                        // if current EXTENSIONID is present in _skillExtnInfo dictionary object, then take a count else ignore. 
                        // here SkillID obtained from _tmacProxy is actually EXTENSIONID.
                        if (_skillExtnInfo.ContainsKey(entry.SkillID))
                        {                             

                            //var datad =_tmacProxy.GetQueueStatusForSkill(entry.SkillID,"");
                            log.Debug("Extension-Id : " + entry.SkillID + " is found in _skillExtnInfo dictionary object");
                            data = new RealtimeData();
                            // here model name skill is actually extnid,so get actual skillid from _skillExtnInfo object
                            data.SkillId = _skillExtnInfo[entry.SkillID].SkillId;
                            // get the channel 
                            //var channel = entry.SkillName.ToLower().StartsWith("v") ? "voice" : "chat";
                            //if (entry.SkillName.ToLower().StartsWith("e"))
                            //    channel = "email";
                            // var channel = ConfigurationData.GetChannel(Convert.ToString(data.SkillId));
                            // get total abundent calls and summary of acd calls from DB.
                            //data.AbandCalls = DataAccess.GetAbnData(entry.SkillID, channel).ToString();
                            //data.AcdCallsSummary = DataAccess.GetACDData(entry.SkillID, channel).ToString();
                            data.Channel = Utilites.ConfigurationData.GetChannel(Convert.ToString(data.SkillId));

                            // below four fields data are obtained from GetTmacWallboardSkills method.
                            data.SkillName = entry.SkillName;

                            //data.Staff = entry.AgentsStaffed.ToString();
                            //data.Avail = entry.AgentAvailable.ToString();
                            try
                            {
                               // data.InteractionsInQueue = (data.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(entry.SkillID)) : entry.CallsInQueue;
                                data.CallsWaiting = (data.Channel.ToLower() == "email") ? Convert.ToInt32(WorkQueueProxy.GetQueueCount(entry.SkillID)) : entry.CallsInQueue;
                                //  data.OldestInteractionWaitTime = (data.Channel.ToLower() != "voice") ? Convert.ToInt32(WorkQueueProxy.GetOldestWaitTime(entry.SkillID)) : 0;
                                // data.OldestInteractionWaitTime = Convert.ToInt32(WorkQueueProxy.GetOldestWaitTime(entry.SkillID));
                                data.OldestCallWaitTime = WorkQueueProxy.GetOldestWaitTime(entry.SkillID);

                            }
                            catch (Exception ex)
                            {
                                log.Error("Error while reading workqueue data : " + ex);
                            }


                            data.SkillExtensionId = _skillExtnInfo[entry.SkillID].ExtensionId;
                            // new method will be exposed to get this data for given skillid

                            //if (getActiveInteractions != null)
                            //    data.ACD = getActiveInteractions.FirstOrDefault(x => x.SkillID == data.Skill).ActiveInteractions.ToString();

                            // get the agentdata[Total agents logged-in for a skill] for given skillId
                            try
                            {
                             //   data.AgentStats = _instance.GetAgentListLoggedInForSkill(data.SkillId.ToString());
                                data.AgentData = _instance.GetAgentListLoggedInForSkill(data.SkillId.ToString());
                                if (data.AgentData != null)
                                {
                                    //data.ACD = data.AgentData.Count(x => x.State.Contains("On Call")).ToString();

                                    /*
                                    if (acdInteraction != null)
                                    {                                   
                                        DateTime lastSyncDate = DateTime.Now.Date;

                                        RData value;

                                        if (acdInteraction != null)
                                        {
                                            if (acdInteraction.TryGetValue(data.Skill, out value))
                                            {
                                                DateTime date = value.LastUpdatedTime;
                                                if (date == lastSyncDate)
                                                {
                                                    log.Debug("update | old acd value = " + value.TotalACDInteractions + ", new acd : " + data.ACD + " for skill : " + data.Skill);
                                                    acdInteraction.TryRemove(data.Skill, out RData oldValue);
                                                    acdInteraction.TryAdd(data.Skill, new RData { LastUpdatedTime = lastSyncDate, TotalACDInteractions = oldValue.TotalACDInteractions + data.ACD });
                                                }
                                                else
                                                {
                                                    log.Debug("AddOrUpdate | old acd value = " + value.TotalACDInteractions + ", new acd : " + data.ACD + " for skill : " + data.Skill);
                                                    acdInteraction.AddOrUpdate(data.Skill, new RData { LastUpdatedTime = lastSyncDate, TotalACDInteractions = data.ACD },
                                                        (k, v) => new RData
                                                        {
                                                            LastUpdatedTime = lastSyncDate,
                                                            TotalACDInteractions = data.ACD
                                                        });
                                                }
                                                //acdInteraction.TryAdd(data.Skill, new RData { LastUpdatedTime = lastSyncDate, TotalACDInteractions = data.ACD });
                                            }
                                            else
                                                acdInteraction.TryAdd(data.Skill, new RData { LastUpdatedTime = lastSyncDate, TotalACDInteractions = data.ACD });
                                        }
                                    }
                                    */
                                    //data.ACW = "0";
                                    //data.AUX = "0";
                                    //data.TotalACDInteractions = Convert.ToString(entry.ActiveInteractions + 1);
                                    // data.TotalACDInteractions = acdInteraction[data.Skill].TotalACDInteractions ?? "0";
                                    log.Debug("Active Interaction : " + entry.ActiveInteractions + 1);
                                    data.TotalAgentsStaffed = data.AgentData.Count();
                                    //data.Avail = data.AgentData.Count(x => x.State.Contains("Available")).ToString();
                                    data.TotalAgentsInACW = data.AgentData.Count(x => x.State.Contains("ACW"));
                                    data.TotalAgentsInAUX = data.AgentData.Count(x => Utilites.ConfigurationData.auxCodes.Contains(x.State));
                                }

                            }
                            catch (Exception)
                            {
                                log.Error("Error while calling tmac method : GetAgentListLoggedInForSkill() for skill = " + data.SkillId);
                            }

                            // currently these values are not used in front-end.
                            //data.ActiveInteractions = entry.ActiveInteractions;
                            //data.TotalAgentsAvailable = entry.AgentAvailable;
                            //data.AverageAbandonedTime = 0;
                            data.ActiveCalls = entry.ActiveInteractions;
                            data.TotalAvailableAgents = entry.AgentAvailable;
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

                            if (!queue.Contains(entry.SkillID))
                                queue.Enqueue(entry.SkillID);
                        }
                    }
                }
                else
                    log.Info("No response from TMAC");

            }
            catch (Exception ex)
            {
                log.Error("Error in FetchBcmsData() :" , ex);
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
                                log.Debug("Entry data [0]: " + entry.ItemArray[0].ToString());
                                log.Debug("Entry data [1]: " + entry.ItemArray[1].ToString());

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
                        Thread.Sleep(300000);
                    }
                }));
                dbThread.Start();
            }
            catch (Exception ex)
            {
                _isStarted = false;
                log.Error("Error in AgentSkillInfo() :" , ex);
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
        /// <summary>
        /// Filters out only Loggedin agents from agent-skill dictionary object for requested skillid.
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>List of agents who are currently loggedin for given skill</returns>
        List<AgentData> GetAgentListLoggedInForSkill(string skillId)
        {
            log.Debug("GetAgentListLoggedInForSkill()");
            try
            {
                List<AgentData> agentListForSkill = new List<AgentData>();
                AgentData agentData;

                // get all currently logged-in agents.
                var loggedInAgents = _tmacProxy.GetLoggedInAgentList("");

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



        #endregion
    }
}
