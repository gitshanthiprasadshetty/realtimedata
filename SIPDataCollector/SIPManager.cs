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

namespace SIPDataCollector
{
    public class SIPManager : ISipDataCollector
    {
        #region Global Declaration

        /// <summary>
        /// Logger
        /// </summary>
        static Logger.Logger Log = new Logger.Logger(typeof(SIPManager));

        /// <summary>
        /// TMAC Proxy
        /// </summary>
       // private static readonly ServiceClient tmacServiceClient = new ServiceClient();

        /// <summary>
        /// TMAC Proxy
        /// </summary>
        static AMACWeb_Proxy.TmacConnector _tmacProxy = new AMACWeb_Proxy.TmacConnector("DefaultTMACServer");

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
        static bool _isStarted = false;

        //bool _sipthreadStatus = false;

        //Thread sipThread;

        ConcurrentDictionary<string, RData> acdInteraction = new ConcurrentDictionary<string, RData>();

        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

        #endregion

        #region Singleton

        /// <summary>
        /// Constructor
        /// </summary>
        //private SIPManager()
        //{
        //}

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
            Log.Debug("Start");
            try
            {
                // First map Skill-Extension data reading from db, before starting the actual work.
                if (!_isStarted)
                    _instance.MapSkillExtnData();

                //  sipThread = new Thread(StartSip);      
                var sipThread = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        Log.Debug("Get Data for SIP");
                        _instance.FetchBcmsData();
                        Thread.Sleep(ConfigurationData.DashboardRefreshTime);
                    }
                }));
                sipThread.Start();

                Thread historicalData = new Thread(new ThreadStart(delegate{ GetHistoricalData(); }));
                historicalData.Start();
            }
            catch (Exception ex)
            {
                Log.Error("Error in BCMSDashboardManager[StartSIP] :" + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //public void StartSip()
        //{
        //    try
        //    {

        //        //if (_sipthreadStatus == false)
        //        //{
        //            // Thread that runs on timely-fashion for given intervals of time in miliseconds.
        //            var sipThread = new Thread(new ThreadStart(delegate
        //            {
        //                while (true)
        //                {
        //                    Log.Debug("Get Data for SIP");
        //                    _instance.FetchBcmsData();
        //                    Thread.Sleep(ConfigurationData.DashboardRefreshTime);
        //                }
        //            }));
        //            sipThread.Start();

        //        //    if (sipThread.ThreadState == ThreadState.Aborted || sipThread.ThreadState == ThreadState.Stopped
        //        //   || sipThread.ThreadState == ThreadState.Unstarted)
        //        //    {
        //        //        _sipthreadStatus = false;
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public bool SIPStatus()
        //{
        //    return _isStarted;
        //}

        /// <summary>
        /// Gets List of bcms data related to all given skills in config
        /// </summary>
        /// <returns>returns all bcms data related for given skills in config.</returns>
        public List<BcmsDataForSIP> GetBcmsData()
        {
            Log.Debug("SIPManager[GetBcmsData]");
            try
            {
                return DataCache.GetBcmsData();
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetBcmsData() :" + ex);
            }
            return null;
        }

        /// <summary>
        /// Gets bcms data for requested skillid
        /// </summary>
        /// <param name="skillId">skillId</param>
        /// <returns>returns bcms data for passed skillid</returns>
        public BcmsDataForSIP GetBcmsDataForSkill(string skillId)
        {
            Log.Debug("SIPManager[GetBcmsDataForSkill]");
            try
            {
                return DataCache.GetBcmsDataForSkill(skillId);
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetBcmsDataForSkill() :" + ex);
            }
            return null;
        }

        /// <summary>
        /// Pulls data from alternate server if HA is enabled.
        /// </summary>
        public void PullDataFromAlternateServer()
        {
            Log.Debug("SIPManager[PullDataFromAlternateServer]");
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
                        Log.Debug("pulldata : " + i + "AgentCount " + data.AgentData.Count());
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
                Log.Error("Error in PullDataFromAlternateServer() :" + ex);
            }
        }


        ///// <summary>
        ///// Fetch the bcms data.
        ///// </summary>
        //public void GetRealTimeBcmsData()
        //{
        //    Log.Debug("SIPManager[GetRealTimeData]");
        //    try
        //    {
        //        _instance.FetchBcmsData();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Error in GetRealTimeBcmsData() :" + ex);
        //    }
        //}

        #endregion

        #region Private 

        /// <summary>
        /// Maps Extension number to Skillid reffering table[TMAC_SKILLS table]
        /// </summary>
        public void MapSkillExtnData()
        {
            Log.Debug("SIPManager[MapSkillExtnData]");
            try
            {
                // Load all local config values.
                ConfigurationData.LoadConfig();

                // Enable the loop
                _isStarted = true;

                // Initialize agent-{skill} mapping
                AgentSkillInfo();

                // get skillidlist from config to monitor.
                var skillIdList = ConfigurationData.skillsToMonitor;

                // get extnid for given set of skills
                var result = DataAccess.GetSkillExtnInfo(skillIdList);
                if (result != null)
                {
                    Log.Debug("Skill to Extension mapping");
                    foreach (DataRow entry in result.Rows)
                    {
                        // add to dictionary object to maintain a skill-Extn mapping.
                        _skillExtnInfo.Add(entry.ItemArray[1].ToString(), new SkillExtensionInfo
                        {
                            SkillId = entry.ItemArray[0].ToString(),
                            ExtensionId = entry.ItemArray[1].ToString(),
                            SkillName = entry.ItemArray[2].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in MapSkillExtnData() :" + ex);
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
                Log.Debug("FetchBcmsData()");

                // get the skills from config to be monitored.
                var skillIdList = ConfigurationData.skillList;

               // List<BcmsDataForSIP> bcmsdata = new List<BcmsDataForSIP>();
                BcmsDataForSIP data;

                // retruns list of data
                var getActiveInteractions = _tmacProxy.Stat_GetSkillData();
                // Gives wallboard data for all("") skills.
                var res = _tmacProxy.GetTmacWallboardSkills("");

                if (res != null)
                {
                    Log.Debug("Total count from GetTmacWallboardSkills : " + res.Count());
                    foreach (var entry in res)
                    {
                        // if skill to exten mapping is not happend, try again.
                        if (_skillExtnInfo.Count() == 0)
                            MapSkillExtnData();

                        // if current EXTENSIONID is present in _skillExtnInfo dictionary object, then take a count else ignore. 
                        // here SkillID obtained from _tmacProxy is actually EXTENSIONID.
                        if (_skillExtnInfo.ContainsKey(entry.SkillID))
                        {
                            //var datad =_tmacProxy.GetQueueStatusForSkill(entry.SkillID,"");
                            Log.Debug("Extension-Id : " + entry.SkillID + " is found in _skillExtnInfo dictionary object");
                            data = new BcmsDataForSIP();

                            // get the channel 
                            var channel = entry.SkillName.ToLower().StartsWith("v") ? "voice" : "chat";
                            if (entry.SkillName.ToLower().StartsWith("e"))
                                channel = "email";

                            // get total abundent calls and summary of acd calls from DB.
                            //data.AbandCalls = DataAccess.GetAbnData(entry.SkillID, channel).ToString();
                            //data.AcdCallsSummary = DataAccess.GetACDData(entry.SkillID, channel).ToString();
                            data.Channel = channel;

                            // below four fields data are obtained from GetTmacWallboardSkills method.
                            data.SkillName = entry.SkillName;
                            //data.Staff = entry.AgentsStaffed.ToString();
                            //data.Avail = entry.AgentAvailable.ToString();
                            try
                            {
                                data.CallsWaiting = (channel.ToLower() == "email") ? WorkQueueProxy.GetQueueCount(entry.SkillID) : entry.CallsInQueue.ToString();

                                data.OldestCall = (channel.ToLower() != "voice") ? WorkQueueProxy.GetOldestWaitTime(entry.SkillID) : "";

                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error while reading workqueue data : " + ex);
                            }
                          
                            // here model name skill is actually extnid,so get actual skillid from _skillExtnInfo object
                            data.Skill = _skillExtnInfo[entry.SkillID].SkillId;
                            // new method will be exposed to get this data for given skillid

                            //if (getActiveInteractions != null)
                            //    data.ACD = getActiveInteractions.FirstOrDefault(x => x.SkillID == data.Skill).ActiveInteractions.ToString();

                            // get the agentdata[Total agents logged-in for a skill] for given skillId
                            try
                            {
                                data.AgentData = _instance.GetAgentListLoggedInForSkill(data.Skill);
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
                                                    Log.Debug("update | old acd value = " + value.TotalACDInteractions + ", new acd : " + data.ACD + " for skill : " + data.Skill);
                                                    acdInteraction.TryRemove(data.Skill, out RData oldValue);
                                                    acdInteraction.TryAdd(data.Skill, new RData { LastUpdatedTime = lastSyncDate, TotalACDInteractions = oldValue.TotalACDInteractions + data.ACD });
                                                }
                                                else
                                                {
                                                    Log.Debug("AddOrUpdate | old acd value = " + value.TotalACDInteractions + ", new acd : " + data.ACD + " for skill : " + data.Skill);
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
                                    Log.Debug("Active Interaction : " + entry.ActiveInteractions + 1);
                                    data.Staff = Convert.ToString(data.AgentData.Count());
                                    //data.Avail = data.AgentData.Count(x => x.State.Contains("Available")).ToString();
                                    data.ACW = data.AgentData.Count(x => x.State.Contains("ACW")).ToString();
                                    data.AUX = Convert.ToString(data.AgentData.Count(x => ConfigurationData.auxCodes.Contains(x.State)));
                                }

                            }
                            catch (Exception)
                            {
                                Log.Error("Error while calling tmac method : GetAgentListLoggedInForSkill() for skill = " + data.Skill);
                            }


                            // currently these values are not used in front-end.
                            data.ACD = Convert.ToString(entry.ActiveInteractions);
                            data.Avail = Convert.ToString(entry.AgentAvailable);
                            data.AvgAbandTime = "00:00:00";
                            data.Date = "";
                            data.Extn = "";
                            data.Other = "";

                            try
                            {
                                data.AccptedSL = ConfigurationData.acceptableSlObj.FirstOrDefault(x => x.Key == data.Skill).Value;
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
            }
            catch (Exception ex)
            {
                Log.Error("Error in FetchBcmsData() :" + ex);
            }
        }

        /// <summary>
        /// Creats Agent-skill map list and keeps in sync with db.
        /// </summary>
        void AgentSkillInfo()
        {
            Log.Debug("AgentSkillInfo()");
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
                            Log.Debug("Total Agent-Skill Info Dictionary count : " + _agentSkillInfo.Count());
                        }
                        Thread.Sleep(300000);
                    }
                }));
                dbThread.Start();
            }
            catch (Exception ex)
            {
                _isStarted = false;
                Log.Error("Error in AgentSkillInfo() :" + ex);
            }
        }


        public void GetHistoricalData()
        {
            Log.Debug("GetHistoricalData()");
            try
            {
                while (true)
                {
                    if (!queue.IsEmpty)
                    {
                        Log.Debug("Queue is not empty");
                        foreach (var item in queue)
                        {
                            Log.Debug("Queue item is = " + item);
                            if (queue.TryDequeue(out string skillExtn))
                            {
                                Log.Debug("skillExtn is = " + skillExtn);
                                string skillId = _skillExtnInfo[skillExtn].SkillId ?? string.Empty;
                                var dbData = DataAccess.GetHistoricalData(skillExtn, skillId);
                                if (dbData != null)
                                {
                                    Log.Debug("Received historical data");
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
                                            skillID = _skillExtnInfo[skillExtn].SkillId ?? string.Empty,
                                            TotalACDInteractions = dbData.TotalACDInteractions,
                                            AbandonPercentage = abandPercentage,
                                            AvgAbandTime = dbData.AvgAbandTime
                                        };
                                        if (!string.IsNullOrEmpty(data.skillID) && (data != null))
                                            DataCache.UpdateHistoricalData(data);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("Error while processing histoircal data : ", ex);
                                    }
                                }
                            }
                        }                        
                    }
                    Thread.Sleep(ConfigurationData.DBRefreshTime);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetHistoricalData", ex);
            }
        }
        /// <summary>
        /// Filters out only Loggedin agents from agent-skill dictionary object for requested skillid.
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>List of agents who are currently loggedin for given skill</returns>
        List<AgentData> GetAgentListLoggedInForSkill(string skillId)
        {
            Log.Debug("GetAgentListLoggedInForSkill()");
            try
            {
                List<AgentData> agentListForSkill = new List<AgentData>();
                AgentData agentData;

                // get all currently logged-in agents.
                var loggedInAgents = _tmacProxy.GetLoggedInAgentList("");

                // _agentSkillInfo contains all the agentdata from db, get all agents who has requested skillid.
                var agentList = _agentSkillInfo.Where(x => x.Value.Skills.Contains(skillId)).Select(x => x.Key);
                // Log.Debug("Total number of agents found for skill : " + skillId + "is "+agentList.Count());

                // Loop through and check amoung list of agents having given skill, how many of them are currently
                // logged-into TMAC. Make a list of these agents and return.
                foreach (var entry in agentList)
                {
                    // for each agentid check against loggedin agents, if found then add to list obj else ignore.
                    var agentDetails = loggedInAgents.FirstOrDefault(y => y.AgentLoginID == entry);
                    if (agentDetails != null)
                    {
                        agentData = new AgentData { LoginId = entry, State = agentDetails.CurrentAgentStatus, TotalStaffedAgents = loggedInAgents.Count };
                        agentListForSkill.Add(agentData);
                    }
                }
                return agentListForSkill;
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAgentListLoggedInForSkill() :" + ex);
                return null;
            }
        }

        #endregion



        #endregion
    }
}
