using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMACWeb_Proxy.commandManagerWCF;
using CMDataCollector.Models;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;

namespace CMDataCollector.Utilities
{
    public class SIPManager
    {
        #region GlobalVariables

        /// <summary>
        /// 
        /// </summary>
        static Logger.Logger Log = new Logger.Logger(typeof(SIPManager));

        /// <summary>
        /// TMAC 
        /// </summary>
       // private static readonly ServiceClient tmacServiceClient = new ServiceClient();

        static AMACWeb_Proxy.TmacConnector tmacProxy = new AMACWeb_Proxy.TmacConnector("DefaultTMACServer");
        /// <summary>
        /// 
        /// </summary>
        readonly CacheMemory cachmemObj = new CacheMemory();

        /// <summary>
        /// Holds Agent->Skill Mapping data
        /// </summary>
        //Dictionary<string, AgentData> agentListObj = new Dictionary<string, AgentData>();

               
        static readonly Dictionary<string, SkillExtensionInfo> _skillExtnInfo = new Dictionary<string, SkillExtensionInfo>();

        /// <summary>
        /// 
        /// </summary>
        static readonly ConcurrentDictionary<string, AgentSkillInfo> _agentSkillInfo = new ConcurrentDictionary<string, AgentSkillInfo>();

        static bool isStarted = false;
        #endregion

        #region Singleton

        /// <summary>
        /// Constructor
        /// </summary>
        private SIPManager()
        {
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

        /// <summary>
        /// 
        /// </summary>
        //public void GetSkillExtnData()
        //{
        //    try
        //    {
        //        cachmemObj.GetSkillList();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        public void GetRealTimeBcmsData()
        {
            Log.Debug("GetRealTimeData()");
            _instance.FetchBcmsData();
        }

        /// <summary>
        /// Maps Extension number to Skillid from db[TMAC_SKILLS table]
        /// </summary>
        public void MapSkillExtnData()
        {
            try
            {
                isStarted = true;
                AgentSkillInfo();
                // get skillidlist from config to monitor.
                var skillIdList = ConfigurationData.skillsToMonitor;

                // get extnid for given skills
                var result = DataAccess.GetSkillExtnInfo(skillIdList);
                if (result != null)
                {
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
                Log.Error("Error in GetSkillExtnData() :" + ex);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 
        /// </summary>
        void FetchBcmsData()
        {
            try
            {
                Log.Debug("FetchBcmsData()");

                //List<AgentData> agentList = new List<AgentData>();

                //// get all skillid for each agent.
                //AgentData agent;
                //var val = tmacProxy.GetLoggedInAgentList("");
                //if (val != null)
                //{
                //    foreach (var entry in val)
                //    {
                //       // var result = entry.InteractionList.Where(x => x.Channel.ToLower() == "voice" && x.LastStatus.ToLower() == "connected");                        
                //        agent = new AgentData();
                //        agent.LoginId = entry.AgentLoginID;
                //        agent.State = entry.CurrentAgentStatus;
                //       // agent.IsOnCall = result.Count() > 0 ? true : false;
                //        agentList.Add(agent);
                //    }
                //}

                //get the skills to be monitored.
                var skillIdList = ConfigurationData.skillList;

                List<BcmsDataForSIP> bcmsdata = new List<BcmsDataForSIP>();
                BcmsDataForSIP data;
                var res = tmacProxy.GetTmacWallboardSkills("");
                if (res != null)
                {
                    foreach (var entry in res)
                    {
                        if (_skillExtnInfo.Count() == 0)
                            MapSkillExtnData();
                        // if current extensionid is present in _skillExtnInfo dictionary object, then take a count else ignore. 
                        if (_skillExtnInfo.ContainsKey(entry.SkillID))
                        {
                            data = new BcmsDataForSIP();
                            data.SkillName = entry.SkillName;
                            // here model nanme skill is actually extnid, get actual skillid from _skillExtnInfo object
                            data.Skill = _skillExtnInfo[entry.SkillID].SkillId;
                            data.Staff = entry.AgentsStaffed.ToString();
                            data.Avail = entry.AgentAvailable.ToString();
                            data.CallsWaiting = entry.CallsInQueue.ToString();
                            // new method will be exposed to get this data for given skillid
                            data.ACD = "0";

                            // get the agentdata for given skillId
                            data.AgentData = _instance.GetAgentListLoggedInForSkill(data.Skill);
                            bcmsdata.Add(data);
                        }
                    }
                }

                // String[] skillList = ConfigurationData.skillList;
                BcmsDashboard bcms;
                if (bcmsdata.Count() > 0)
                {
                    Log.Debug("GetRealTimeData() : bcmsdata.Count()" + bcmsdata.Count());
                    foreach (var entry in bcmsdata)
                    {
                        int abnCount = DataAccess.GetAbnData(entry.Skill);
                        int acdSummaryCount = DataAccess.GetACDData(entry.Skill);
                        Log.Debug("Abn count :" + abnCount + "For skill :" + entry.Skill);
                        //var skillid = CacheMemory.GetSkillForExtn(entry.Skill);
                        //if (skillList.Contains(skillid))
                        //{
                        bcms = new BcmsDashboard();
                        bcms.ACD = entry.ACD;
                        bcms.CallsWaiting = entry.CallsWaiting;
                        bcms.Skill = entry.Skill;
                        bcms.SkillName = entry.SkillName;
                        bcms.Staff = entry.Staff;
                        bcms.Avail = entry.Avail;

                        bcms.AgentData = entry.AgentData;

                        bcms.ACW = "0";
                        bcms.AUX = "0";
                        bcms.AvgAbandTime = "0";
                        bcms.Date = "";
                        bcms.Extn = "";
                        bcms.OldestCall = "";
                        bcms.Other = "";
                        bcms.SL = "";

                        bcms.AbandCalls = abnCount.ToString();
                        bcms.AcdCallsSummary = acdSummaryCount.ToString();
                        bcms.AvgAbandTime = "";
                        CacheMemory.UpdateCacheData(bcms);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetRealTimeData() :" + ex);
            }
        }        

        /// <summary>
        /// Agent-skill map list
        /// </summary>
        void AgentSkillInfo()
        {
            try
            {
                DataTable result = null;

                // get agent-skillid data from db[TMAC_Agent_Skills]
                var dbThread = new Thread(new ThreadStart(delegate
                {
                    while (isStarted)
                    {
                       // [commented: since to be called for intervl atleast < 1min ; here intervl is 5min]
                       // var agentData = tmacProxy.GetLoggedInAgentList("");

                        result = DataAccess.GetAgentSkillInfo();
                        if (result != null)
                        {
                            foreach (DataRow entry in result.Rows)
                            {
                                // check if current-agentid is present in loggedinagentlist.
                                // var agent = agentData.FirstOrDefault(x => x.AgentLoginID == entry.ItemArray[0].ToString());

                                // if found then add record, else ignore.
                                //if (agent != null)
                                //{

                                // since we get data from db for every 5min and getAgentloggedinlist method cant be synced together for 5min interval
                                // consider all agentdata , filter can be done later in GetAgentListLoggedInForSkill method.

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
                                //}
                            }
                        }
                        Thread.Sleep(300000);
                    }
                }));
                dbThread.Start();
            }
            catch (Exception ex)
            {
                isStarted = false;
                Log.Error("Error in AgentSkillInfo() :" + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        List<AgentData> GetAgentListLoggedInForSkill(string skillId)
        {
            try
            {
                List<AgentData> agentListForSkill = new List<AgentData>();
                AgentData agentData;

                // get all loggedin agents.
                var loggedInAgents = tmacProxy.GetLoggedInAgentList("");

                // _agentSkillInfo contains all the agentdata from db, filter it based on available agent list.
                var agentList = _agentSkillInfo.Where(x => x.Value.Skills.Contains(skillId)).Select(x => x.Key);
                foreach(var entry in agentList)
                {
                    // for each agentid check aganst loggedin agents, if found then add to list obj else ignore.
                    var agentDetails = loggedInAgents.FirstOrDefault(y => y.AgentLoginID == entry);
                    if (agentDetails != null)
                    {
                        agentData = new AgentData { LoginId = entry, State = agentDetails.CurrentAgentStatus };
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
