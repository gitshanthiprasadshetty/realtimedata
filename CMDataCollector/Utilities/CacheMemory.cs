using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using CMDataCollector.Models;
using AutoMapper;
using CMDataCollector.DataParser;
using Newtonsoft.Json;

namespace CMDataCollector.Utilities
{
    public class CacheMemory
    {
        #region Global Declarations

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(CacheMemory));    

        /// <summary>
        /// Concurrent dictornay used to store cache memory.
        /// Since multiple connection will use same cachememory, concurrent dictionary is used.
        /// Holds BCMS data
        /// </summary>
        static readonly ConcurrentDictionary<string, BcmsDashboard> skillCachObj = new ConcurrentDictionary<string, BcmsDashboard>();

        /// <summary>
        /// Holds trunk related data
        /// </summary>
        static readonly ConcurrentDictionary<int, TrunkGroupTraffic> trunkCachObj = new ConcurrentDictionary<int, TrunkGroupTraffic>();

        /// <summary>
        /// Holds trunk related data
        /// </summary>
        static readonly ConcurrentDictionary<string, HuntGroupTraffic> huntTrafficCachObj = new ConcurrentDictionary<string, HuntGroupTraffic>();

        static readonly ConcurrentDictionary<string, BcmsSystem> systemCachObj = new ConcurrentDictionary<string, BcmsSystem>();

        /// <summary>
        /// 
        /// </summary>
       // static readonly Dictionary<string, string> skillExtnData = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        static List<BcmsDashboard> _listObj;

        /// <summary>
        /// Model declaration for bcmsdashboard
        /// </summary>
        static BcmsDashboard _bcmsObj;

        static BcmsSystem _bcmsSysObj;

        /// <summary>
        /// Model declaration for historical report.
        /// </summary>
        BcmsHistoricalReport _reportObj;

        /// <summary>
        /// Holds the List of traffic data for each trunkgroup.
        /// </summary>
        static List<TrunkGroupTraffic> _trunkDataListObj;

        /// <summary>
        /// Model declaration for TrunkGroupTraffic.
        /// </summary>
        static TrunkGroupTraffic _trunkDataObj;

        /// <summary>
        /// Historaical service instance.
        /// </summary>
        readonly THistoricalData.HistoricalDataClient _bcmsReportClientObj =
            new THistoricalData.HistoricalDataClient();

        #endregion

        #region CM

        #region skill

        /// <summary>
        /// Gets all Bcms data for configured skills.
        /// </summary>
        /// <returns>returns list of bcmsdata for each skill</returns>
        public static List<BcmsDashboard> GetBcmsData()
        {
            Log.Debug("CacheMemory[GetBcmsData]");
            try
            {              
                // initiate update
                InitiateUpdateThread();

                Log.Debug("check for type : cm or sip");
                // check the connection type before getting data. If SIP read from sip cache else cm cache obj
                if (ConfigurationData.ConnectionType.ToLower() == "cm")
                {
                    Log.Debug("get and return data for cm");
                    if (skillCachObj != null)
                    {
                        if (skillCachObj.Count > 0)
                        {
                            Log.Debug("Dictionary count : " + skillCachObj.Count);
                            _bcmsObj = new BcmsDashboard();
                            _listObj = new List<BcmsDashboard>();
                            foreach (KeyValuePair<string, BcmsDashboard> entry in skillCachObj)
                            {
                                _bcmsObj = entry.Value;
                                _listObj.Add(_bcmsObj);
                            }
                        }
                        Log.Debug("CacheMemory[GetBcmsData] BcmsDashboard Return Count : " + _listObj.Count);
                        return _listObj;
                    }
                }
                if (ConfigurationData.ConnectionType.ToLower() == "sip")
                {
                    Log.Debug("Return data for sip");
                    var result = SIPDataCollector.SIPManager.GetInstance().GetBcmsData();
                    if (result != null)
                    {
                        //Mapper.CreateMap<BcmsSIPManager.Models.BcmsDataForSIP, BcmsDashboard>();
                        //return Mapper.Map<List<BcmsDashboard>>(result);
                        var output = new List<BcmsDashboard>();
                        var agentDetails = new List<AgentData>();
                        foreach (var entry in result)
                        {
                            var val = new BcmsDashboard();
                            val.AbandCalls = entry.AbandCalls;
                            val.AccptedSL = entry.AccptedSL.ToString();
                            val.ACD = entry.ACD;
                            val.AcdCallsSummary = entry.TotalACDInteractions;
                            val.ACW = entry.ACW;
                            val.AUX = entry.AUX;
                            val.Avail = entry.Avail;
                            val.AvgAbandTime = entry.AvgAbandTime;
                            val.CallsWaiting = entry.CallsWaiting;
                            val.Channel = entry.Channel;
                            val.Date = entry.Date;
                            val.Extn = entry.Extn;
                            val.OldestCall = entry.OldestCall;
                            val.Other = entry.Other;
                            val.Skill = entry.Skill;
                            val.SkillName = entry.SkillName;
                            val.SL = entry.SLPercentage;
                            val.Staff = entry.Staff;
                            val.AvgHandlingTime = entry.AvgHandlingTime;
                            val.AbandonPercentage = entry.AbandonPercentage;
                            Log.Debug("Avg handling : " + val.AvgAbandTime);
                            for (int i = 0; i < entry.AgentData.Count; i++)
                            {
                                agentDetails.Add(new AgentData
                                {
                                    LoginId = entry.AgentData[i].LoginId,
                                    State = entry.AgentData[i].State
                                });
                            }

                            // total agentlist currently loggedin having this skill
                            val.AgentData = agentDetails;
                            output.Add(val);
                        }
                        return output;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[GetBcmsData] : " + ex);
            }
            return null;
        }

        /// <summary>
        ///  Gets Bcms data for requested skill.
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>returns bcms data only for a requested skillid</returns>
        public static BcmsDashboard GetBcmsDataForSkill(string skillId)
        {
            Log.Debug("CacheMemory[GetBcmsDataForSkill]");
            try
            {
                // check the connection type before getting data. If SIP read from sip cache else cm cache obj
                if (ConfigurationData.ConnectionType.ToLower() == "sip")
                {
                    var result = SIPDataCollector.SIPManager.GetInstance().GetBcmsDataForSkill(skillId);
                    if (result != null)
                    {
                        var agentDetails = new List<AgentData>();
                        //Mapper.CreateMap<BcmsSIPManager.Models.BcmsDataForSIP, BcmsDashboard>();
                        //return Mapper.Map<BcmsDashboard>(result);
                        var val = new BcmsDashboard();
                        val.AbandCalls = result.AbandCalls;
                        val.AccptedSL = result.AccptedSL.ToString();
                        val.ACD = result.ACD;
                        val.AcdCallsSummary = result.TotalACDInteractions;
                        val.ACW = result.ACW;
                        val.AUX = result.AUX;
                        val.Avail = result.Avail;
                        val.AvgAbandTime = result.AvgAbandTime;
                        val.AbandonPercentage = result.AbandonPercentage;
                        val.AvgHandlingTime = result.AvgHandlingTime;                        
                        val.CallsWaiting = result.CallsWaiting;
                        val.Channel = result.Channel;                        
                        val.Date = result.Date;
                        val.Extn = result.Extn;
                        val.OldestCall = result.OldestCall;
                        val.Other = result.Other;
                        val.Skill = result.Skill;
                        val.SkillName = result.SkillName;
                        val.SL = result.SLPercentage;
                        val.Staff = result.Staff;
                        if (result != null)
                        {
                            for (int i = 0; i < result.AgentData.Count; i++)
                            {
                                agentDetails.Add(new AgentData
                                {
                                    LoginId = result.AgentData[i].LoginId,
                                    State = result.AgentData[i].State
                                });
                            }
                        }
                        val.AgentData = agentDetails;
                        return val;
                    }
                }
                if (ConfigurationData.ConnectionType.ToLower() == "cm")
                {
                    //if (CachObj != null)
                    //{
                        if (skillCachObj.Count > 0)
                        {
                            Log.Debug("CacheMemory[GetBcmsDataForSkill] Dictionary count : " + skillCachObj.Count);
                            _bcmsObj = new BcmsDashboard();
                            var result = skillCachObj.FirstOrDefault(x => x.Key == skillId);
                            if (!result.Equals(default(KeyValuePair<string, BcmsDashboard>)))
                            {
                                _bcmsObj = skillCachObj[result.Key];
                            }
                        }
                        return _bcmsObj;
                    //}
                }            
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[GetBcmsDataForSkill] : " + ex);
            }
            return null;
        }

        /// <summary>
        ///  Gets Bcms data for requested skill.
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>returns bcms data only for a requested skillid</returns>
        public static List<BcmsSystem> GetBcmsSystemData(string skillName)
        {
            Log.Debug("GetBcmsSystemData() : " + skillName);
            try
            {
                if (ConfigurationData.ConnectionType.ToLower() == "cm")
                {
                    if (systemCachObj.Count > 0)
                    {
                        Log.Debug("GetBcmsSystemData Dictionary count : " + systemCachObj.Count);
                        _bcmsSysObj = new BcmsSystem();
                        List<BcmsSystem> result = new List<BcmsSystem>();
                        if (!string.IsNullOrWhiteSpace(skillName))
                            result.Add(systemCachObj.FirstOrDefault(x => x.Key == skillName).Value);
                        else
                            result.AddRange(systemCachObj.Values);

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetBcmsSystemData() : " + ex);
            }
            return new List<BcmsSystem>();
        }

        /// <summary>
        /// Get cache count [gives only CM cache count, for SIP this is not required as of now.]
        /// Used to check whether all the skills mentioned in config is queried atleast once. 
        /// If yes then returns true else false.
        /// Required because based on this, update to cache happens else update wont be started.
        /// </summary>
        /// <returns>returns total CM cache count</returns>
        public static bool GetCacheCount()
        {
            Log.Debug("CacheMemory[GetCacheCount]");
            try
            {
                // get total number of skills that we are suppose to monitor from config.
                int value = ConfigurationData.skillList.Count();

                // array of skills 
                // string[] skills = ConfigurationData.skillList;

                Log.Debug("CacheMemory[GetCacheCount] Dictionary count : " + skillCachObj.Count);

                // if cache count is equal to total number of skills to be monitored then return true.
                if (skillCachObj.Count() == value)
                    return true;

                // 21-06-17
                // this method will return false if in config user gives wrong/non-existable skillid



                // commented below lines of code because to reduce unnecessary creation of multiple request commands to CM and
                // to reduce multiple thread creation.
                #region Commented lines
                // if cache count is not equal to total number of skills to be monitored,
                // then find which skill is not found in cache memory and execute it seperately using ExecuteCommandForSkill method.
                //if (CachObj.Count() != value)
                //{
                //    List<string> skillSet = new List<string>();
                //    foreach (var skillId in skills)
                //    {
                //        // check which skill is not present in cache memory, create a list of those.
                //        bool res = CachObj.ContainsKey(skillId);
                //        if (!res)
                //            skillSet.Add(skillId);
                //    }

                //    // for each skillset run execute command.
                //    foreach (var skill in skillSet)
                //    {
                //        CMConnectionManager.GetInstance().ExecuteCommandForSkill(skill);
                //    }
                //}
                #endregion

                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[GetCacheCount] : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Pulls historical related data from exposed service.
        /// </summary>
        /// <param name="skillId">skillid</param>
        public void GetBcmsHistoricalData(string skillId)
        {
            Log.Debug("CacheMemory[GetBcmsHistoricalData]");
            try
            {
                // call bcms historical report service
                var result = _bcmsReportClientObj.GetSkillHistoricalData(skillId);
                if (result != null)
                {
                    //Log.Debug("abandonedcalls dumbu : " + Convert.ToString(result.AbandCalls));
                    //Log.Debug("AcdCalls dumbu : " + Convert.ToString(result.AcdCalls));
                    Log.Debug("CacheMemory[GetBcmsHistoricalData] Result from service for skill : "+result.SkillId);
                    _reportObj = new BcmsHistoricalReport();
                    _reportObj.abandoned_Calls = Convert.ToString(result.AbandCalls);
                    //Log.Debug("abandonedcalls bokka : " + Convert.ToString(result.AbandCalls));
                    _reportObj.acceptable_Service_Level = Convert.ToString(result.ServiceLevel);
                    _reportObj.acd_Calls = Convert.ToString(result.AcdCalls);
                    //Log.Debug("AcdCalls bokka : " + Convert.ToString(result.AcdCalls));
                    _reportObj.avg_Abandoned_Time = Convert.ToString(result.AvgAbandTime);
                    _reportObj.avg_Speed_Answered = result.AvgSpeedAnswer;
                    _reportObj.avg_Staff = Convert.ToString(result.AvgStaff);
                    _reportObj.avg_Talk_Time = result.AvgTalkTime;
                    _reportObj.date = result.DateInt;
                    _reportObj.flow_In = Convert.ToString(result.FlowIn);
                    _reportObj.flow_Out = Convert.ToString(result.FlowOut);
                    _reportObj.interval = result.Interval;
                    _reportObj.pct_In_Svc_Level = result.ServiceLevel;
                    _reportObj.total_After_Call = result.TotalAfterCallTime;
                    _reportObj.total_Aux_Other = result.TotalAux;
                    _reportObj.report_Type = result.ReportType;
                    _reportObj.skill = result.SkillId;
                    _reportObj.skill_Name = result.SkillName;
                    _reportObj.switch_Name = result.SwitchName;

                    // update to same bcms cache memory with required data
                    UpdateCacheMemory(_reportObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[GetBcmsHistoricalData] : " + ex);
            }
        }

        /// <summary>
        /// Keep update bcms cache-memory[CM] with bcms data.
        /// </summary>
        /// <param name="bcmsReturnObj"><see cref="BcmsDashboard"/>dddd</param>
        /// <returns></returns>
        public static bool UpdateCacheMemory(BcmsDashboard bcmsReturnObj)
        {
            Log.Debug("CacheMemory[UpdateCacheMemory].BcmsDashboard");
            try
            {
                if (bcmsReturnObj != null)
                {                    
                    if (bcmsReturnObj.Skill != null)
                    {
                        Log.Debug("CacheMemory[UpdateCacheMemory] skill: " + bcmsReturnObj.Skill);
                        skillCachObj.TryAdd(bcmsReturnObj.Skill, bcmsReturnObj);

                        var isKeyExist = skillCachObj.ContainsKey(bcmsReturnObj.Skill);
                        Log.Debug("CacheMemory[UpdateCacheMemory] isKeyExist: " + isKeyExist);
                        if (isKeyExist)
                        {
                            var value = skillCachObj.FirstOrDefault(x => x.Key == bcmsReturnObj.Skill).Value;
                            // var value = CachObj[bcmsReturnObj.Skill];
                            // Log.Debug("CacheMemory[UpdateCacheMemory] update to dictionary " + bcmsReturnObj.Skill + "cache abandon call = " + value.AgentData.Count);
                            try
                            {
                                //Log.Debug("Before update : " + JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));

                                value.AbandCalls = (value.AbandCalls == null) ? "0" : value.AbandCalls;
                                value.AvgAbandTime = (value.AbandCalls == null) ? "0" : value.AvgAbandTime;
                                value.AcdCallsSummary = (value.AbandCalls == null) ? "0" : value.AcdCallsSummary;
                                value.SL = string.IsNullOrEmpty(value.SL) ? "0" : value.SL;

                                bcmsReturnObj.AbandCalls = value.AbandCalls;
                                bcmsReturnObj.AvgAbandTime = value.AvgAbandTime;
                                bcmsReturnObj.AcdCallsSummary = value.AcdCallsSummary;
                                bcmsReturnObj.AbandCallsSummary = value.AbandCallsSummary;
                                bcmsReturnObj.SL = value.SL;
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("Error in CacheMemory[UpdateCacheMemory] while appending the historical values : " + ex);
                            }

                            bool updateResult = skillCachObj.TryUpdate(bcmsReturnObj.Skill, bcmsReturnObj, value);
                            Log.Debug("CacheMemory[UpdateCacheMemory] update to dictionary is  " + updateResult + "for " + bcmsReturnObj.Skill);

                            //Log.Debug("After Update : " + JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
                            //if (value.AbandCalls != null)
                            //{
                            //    Log.Debug("CacheMemory[UpdateCacheMemory] update to dictionary for skill : " + bcmsReturnObj.Skill + " cache abandon call = " + value.AbandCalls);
                            //    bcmsReturnObj.AbandCalls = value.AbandCalls;
                            //    bcmsReturnObj.AvgAbandTime = value.AvgAbandTime;
                            //    bcmsReturnObj.AcdCallsSummary = value.AcdCallsSummary;
                            //    bool updateResult = CachObj.TryUpdate(bcmsReturnObj.Skill, bcmsReturnObj, value);
                            //    Log.Debug("CacheMemory[UpdateCacheMemory] update to dictionary is  " + updateResult + "for " + bcmsReturnObj.Skill);
                            //}
                            //else
                            //{

                            //}
                        }
                        else
                            Log.Debug("CacheMemory[UpdateCacheMemory] Add to dictionary " + bcmsReturnObj.Skill);
                        // _cachObj.AddOrUpdate(bcmsReturnObj.Skill,bcmsReturnObj,
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[UpdateCacheMemory].BcmsDashboard : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Keep update bcms cache-memory[CM] with bcms historical data.
        /// </summary>
        /// <param name="reportValue">historical data for a skill</param>
        public static void UpdateCacheMemory(BcmsHistoricalReport reportValue)
        {
            try
            {
                Log.Debug("UpdateCacheMemory()");
                if (reportValue != null)
                {
                    Log.Debug("UpdateCacheMemory for skill : " + reportValue.skill);

                    _bcmsObj = new BcmsDashboard();

                    //foreach (var item in skillCachObj)
                    //{
                    //    Log.Info("skillid " + item.Value.Skill);
                    //    Log.Info("AbandCalls " + item.Value.AbandCalls);
                    //    Log.Info("AbandCallsSummary " + item.Value.AbandCallsSummary);
                    //    Log.Info("AcdCallsSummary " + item.Value.AcdCallsSummary);
                    //}

                    //Log.Debug(JsonConvert.SerializeObject(skillCachObj, Newtonsoft.Json.Formatting.Indented));
                    BcmsDashboard values;
                    if (skillCachObj.TryGetValue(reportValue.skill, out values))
                    {
                        Log.Debug("update historical data to cache memory");
                        //Log.Debug("Input value : " + JsonConvert.SerializeObject(values1, Newtonsoft.Json.Formatting.Indented));
                        values.AbandCallsSummary = reportValue.abandoned_Calls;
                        //values1.AbandCallsSummaryTest = reportValue.abandoned_Calls;
                        //values1.AbandCallsSummaryTest = "prakash";
                        values.AvgAbandTime = reportValue.avg_Abandoned_Time;
                        values.AcdCallsSummary = reportValue.acd_Calls;
                    }
                    else
                        Log.Info("No key present in the cache memory");
                   // Log.Debug("Afert update : " + JsonConvert.SerializeObject(values1, Newtonsoft.Json.Formatting.Indented));

                    //if (skillCachObj.ContainsKey(reportValue.skill))
                    //{
                    //    //_bcmsObj = skillCachObj[reportValue.skill];

                    //    BcmsDashboard values;

                    //    //skillCachObj.TryGetValue(reportValue.skill, out values){

                    //    //};

                    //    Log.Debug("Input value : "+ JsonConvert.SerializeObject(reportValue, Newtonsoft.Json.Formatting.Indented));
                    //    //Log.Debug("");

                    //    Log.Debug("Before update : "+JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.Indented));



                    //    values.AbandCallsSummary = reportValue.abandoned_Calls;
                    //    Log.Info("abandcalls summary new value : " + values.AbandCallsSummary);
                    //   // values.AvgAbandTime = reportValue.avg_Abandoned_Time;
                    //    values.AcdCallsSummary = reportValue.acd_Calls;

                        //_bcmsObj.AccptedSL = values.AccptedSL;
                        //_bcmsObj.ACD = values.ACD;
                        //_bcmsObj.ACW = values.ACW;
                        //_bcmsObj.AbandCalls = values.AbandCalls;
                        //_bcmsObj.AbandonPercentage = values.AbandonPercentage;
                        //_bcmsObj.AgentData = values.AgentData;
                        //_bcmsObj.AvgHandlingTime = values.AvgHandlingTime;
                        //_bcmsObj.CallsWaiting = values.CallsWaiting;
                        //_bcmsObj.Channel = values.Channel;
                        //_bcmsObj.Date = values.Date;
                        //_bcmsObj.Extn = values.Extn;
                        //_bcmsObj.OldestCall = values.OldestCall;
                        //_bcmsObj.Other = values.Other;
                        //_bcmsObj.PercentageInSL = values.PercentageInSL;
                        //_bcmsObj.Skill = values.Skill;
                        //_bcmsObj.SkillName = values.SkillName;
                        //_bcmsObj.Staff = values.Staff;
                        //_bcmsObj.AUX = values.AUX;
                        //_bcmsObj.Avail = values.Avail;

                        //_bcmsObj.AbandCallsSummary = reportValue.abandoned_Calls;
                        //Log.Info("abandcalls summary new value : " + _bcmsObj.AbandCallsSummary);
                        //_bcmsObj.AvgAbandTime = reportValue.avg_Abandoned_Time;
                        //_bcmsObj.AcdCallsSummary = reportValue.acd_Calls;
                        //_bcmsObj.SL = reportValue.pct_In_Svc_Level;
                        //Log.Debug("AddToCacheMemory() cache old value key :" + values.Skill);
                        //skillCachObj.TryUpdate(reportValue.skill, _bcmsObj, values);

                        //Log.Debug("After update "+JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.Indented));
                    //}
                    //else
                    //{
                    //    Log.Debug("skill not found in dictionary.");
                    //}

                    //var isKeyExist = skillCachObj.ContainsKey(reportValue.skill);
                    //if (isKeyExist)
                    //{
                    //    Log.Info("skill exists in current cache memory, update values.");
                    //    var currentData = skillCachObj.FirstOrDefault(x => x.Key == reportValue.skill).Value;
                    //    Log.Info("abandcalls summary old value : " + currentData.AbandCallsSummary);
                    //    _bcmsObj = currentData;
                    //    _bcmsObj.AbandCallsSummary = reportValue.abandoned_Calls;
                    //    Log.Info("abandcalls summary new value : " + _bcmsObj.AbandCallsSummary);
                    //    _bcmsObj.AvgAbandTime = reportValue.avg_Abandoned_Time;
                    //    _bcmsObj.AcdCallsSummary = reportValue.acd_Calls;
                    //    _bcmsObj.SL = reportValue.pct_In_Svc_Level;
                    //    _bcmsObj.AbandCalls = "0";
                    //    _bcmsObj.PercentageInSL = "0";
                    //    try
                    //    {
                    //        if (systemCachObj != null && systemCachObj.Count > 0)
                    //        {
                    //            _bcmsObj.AbandCalls = systemCachObj.FirstOrDefault(x => x.Key == _bcmsObj.SkillName).Value.AbandandCalls ?? "0";
                    //            _bcmsObj.PercentageInSL = systemCachObj.FirstOrDefault(x => x.Key == _bcmsObj.SkillName).Value.PercentageSL ?? "0";
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Log.Error("Error while updating system data to skill : " + ex);
                    //    }

                    //    Log.Debug("update values to skill : " + currentData.Skill);
                    //    skillCachObj.AddOrUpdate(currentData.Skill, _bcmsObj, (k, v) => _bcmsObj);

                    //    //skillCachObj.TryUpdate(currentData.Skill, _bcmsObj, currentData);                 
                    //}
                }
                else
                    Log.Info("No data to update to cache memory.");
            }
            catch (Exception ex)
            {
                Log.Error("Error in UpdateCacheMemory() : " + ex);
            }
        }

        //public static void UpdateCacheMemoryWithSystemData(BcmsSystem sys)
        //{
        //    try
        //    {
        //        var key = skillCachObj.FirstOrDefault(x=>x.Value.SkillName == sys.SkillName).Key;
        //        if (key != null)
        //        {
        //            Log.Debug("key exists");
        //            _bcmsObj = new BcmsDashboard();
        //            var currentData = skillCachObj.FirstOrDefault(x => x.Key == key).Value;
        //            _bcmsObj = currentData;
        //            _bcmsObj.PercentageInSL = sys.PercentageSL;
        //            _bcmsObj.AbandCalls = sys.AbandandCalls;
        //            skillCachObj.TryUpdate(key, _bcmsObj, currentData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Error in UpdateCacheMemoryWithSystemData() : " + ex);
        //    }
        //}

        /// <summary>
        /// Calls Historical service in a periodical fashion.
        /// </summary>
        public void ListBcmsSkill()
        {
            Log.Debug("CacheMemory[ListBcmsSkill]");
            try
            {
                while (true)
                {
                    // get all skill list from config that we use to monitor for bcms
                    var skills = ConfigurationData.skillList;

                    // loop for all skills and get historical data for each skill.
                    foreach (var entry in skills)
                    {
                        GetBcmsHistoricalData(entry);
                    }
                    Log.Debug("CacheMemory[ListBcmsSkill] ReportRefreshTime");
                    Thread.Sleep(ConfigurationData.ReportRefreshTime);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[ListBcmsSkill] : " + ex);
            }
        }

        /// <summary>
        /// Add/Update CacheMemory [Used for HA DataSync]
        /// </summary>
        /// <param name="obj">Data received from other server</param>
        public static void AddToCacheMemory(object obj)
        {
            Log.Debug("CacheMemory[AddToCacheMemory]");
            try
            {
                // unbox the object data to model data.
                _bcmsObj = (BcmsDashboard)obj;
                if (_bcmsObj != null)
                {
                    Log.Debug("AddToCacheMemory() Adding to CacheMemory for skill :" + _bcmsObj.Skill);

                    if (skillCachObj.ContainsKey(_bcmsObj.Skill))
                    {
                        var val = skillCachObj[_bcmsObj.Skill];
                        Log.Debug("AddToCacheMemory() cache old value key :" + val.Skill);
                        skillCachObj.TryUpdate(_bcmsObj.Skill, _bcmsObj, val);
                    }
                    else
                        skillCachObj.TryAdd(_bcmsObj.Skill, _bcmsObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[AddToCacheMemory] : " + ex);
            }
        }

        #endregion

        #region Traffic

        /// <summary>
        /// Add/Update trunk-data to trunk cache-object.
        /// </summary>
        /// <param name="trunkData">trunk data recevied from CM</param>
        public static void UpdateCacheMemory(Object data)
        {
            Log.Debug("CacheMemory[UpdateCacheMemory]");
            try
            {
                if (data != null)
                {
                    _trunkDataObj = (TrunkGroupTraffic)data;
                    Log.Debug("CacheMemory[UpdateCacheMemory] trunkGroup : "+ _trunkDataObj.TrunkGroup);
                    trunkCachObj.AddOrUpdate(_trunkDataObj.TrunkGroup, _trunkDataObj,
                    (k, v) => _trunkDataObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[UpdateCacheMemory] : " + ex);
            }
        }

        /// <summary>
        /// Get all trunk-traffic data stored in cache memory.
        /// </summary>
        /// <returns></returns>
        public static List<TrunkGroupTraffic> GetTrunkTrafficDetails()
        {
            Log.Debug("CacheMemory[GetTrunkTrafficDetails]");
            try
            {                
                if (trunkCachObj != null && trunkCachObj.Count() > 0)
                {
                    _trunkDataObj = new TrunkGroupTraffic();
                    _trunkDataListObj = new List<TrunkGroupTraffic>();
                    foreach (var entry in trunkCachObj)
                    {
                        _trunkDataObj = entry.Value;
                        _trunkDataListObj.Add(_trunkDataObj);
                    }
                    return _trunkDataListObj;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[GetTrunkTrafficDetails] : " + ex);
            }
            return null;
        }

        public static void AddOrUpdateCacheMemory(HuntGroupTraffic data)
        {
            Log.Debug("AddOrUpdateCacheMemory()");
            try
            {
                if (data != null)
                {
                    Log.Debug("CacheMemory[UpdateCacheMemory] hunt group : " + data.HuntGroup);
                    huntTrafficCachObj.AddOrUpdate(data.HuntGroup, data,
                    (k, v) => data);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[UpdateCacheMemory] : " + ex);
            }
        }

        public static List<HuntGroupTraffic> GetHuntTrafficDetails()
        {
            Log.Debug("CacheMemory[GetTrunkTrafficDetails]");
            try
            {
                // initiate update
                InitiateUpdateThread();

                if (huntTrafficCachObj != null && huntTrafficCachObj.Count() > 0)
                {
                    //HuntGroupTraffic _huntDataObj = new HuntGroupTraffic();
                    //List<HuntGroupTraffic> _huntDataListObj = new List<HuntGroupTraffic>();

                    return huntTrafficCachObj.Values.ToList();

                    //foreach (var entry in huntTrafficCachObj)
                    //{
                    //    _huntDataObj = entry.Value;
                    //    _huntDataListObj.Add(_huntDataObj);
                    //}
                    // return _huntDataListObj;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[GetTrunkTrafficDetails] : " + ex);
            }
            return null;
        }

        #endregion

        #region system

        public static int UpdateCacheMemory(List<BcmsSystem> systemData)
        {
            Log.Debug("system : UpdateCacheMemory()");
            try
            {
                systemCachObj.Clear();
                foreach (var entry in systemData)
                {
                    systemCachObj.TryAdd(entry.SkillName, entry);
                    // UpdateCacheMemoryWithSystemData(entry);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in system : UpdateCacheMemory", ex);
                return -1;
            }
            return 0;
        }

        #endregion

        #endregion

        #region SIP

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateCacheData(Object data)
        {
            Log.Debug("CacheMemory[UpdateCacheData]");
            try
            {
                _bcmsObj = (BcmsDashboard)data;
                if (data != null)
                {
                    skillCachObj.TryAdd(_bcmsObj.Skill, _bcmsObj);
                    var isKeyExist = skillCachObj.ContainsKey(_bcmsObj.Skill);
                    if (isKeyExist)
                    {
                        var value = skillCachObj.FirstOrDefault(x => x.Key == _bcmsObj.Skill).Value;
                        skillCachObj.TryUpdate(value.Skill, _bcmsObj, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CacheMemory[UpdateCacheData] : " + ex);
            }
        }
        #endregion


        #region globalmethods

        private static void InitiateUpdateThread()
        {            
            try
            {
                // log.Debug("CacheMemory[GetBcmsData] :: isUpdateStarted" + CMConnectionManager.isUpdateStarted);
                // need to skip this thread from starting, when this service is not connected to cm
                if (CMConnectionManager.GetInstance().connectToCM)
                {
                    Log.Debug("CacheMemory[GetBcmsData] :: IsUpdateStarted : " + CMConnectionManager.GetInstance().IsUpdateStarted +
                               " | IsBcmsEnabled :" + CMConnectionManager.GetInstance().isBcmsEnabled + " | IstrafficEnabled :" + CMConnectionManager.GetInstance().isTrafficEnabled);
                    if (!CMConnectionManager.GetInstance().IsUpdateStarted)
                    {
                        CMConnectionManager.GetInstance().IsUpdateStarted = true;
                        var u = new Thread(new ThreadStart(delegate
                        {
                            Log.Debug("CacheMemory[GetBcmsData] :: UpdateDashboard");
                            CMConnectionManager.GetInstance().UpdateDashboard();
                        }));
                        u.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in InitiateUpdateThread() : ", ex);
            }
        }

        #endregion

        #region not used

        /// <summary>
        /// SIP, call for smsapi to map skillid and extn
        /// </summary>
        //public void GetSkillList()
        //{
        //    try
        //    {
        //        var result = _bcmsReportClientObj.GetSkillExtnList();
        //        //SkillExtnList skillEtnMap;
        //        if (result != null)
        //        {
        //            foreach(var entry in result)
        //            {
        //               // skillEtnMap = new SkillExtnList();
        //                skillExtnData.Add(entry.Extension,entry.Skill);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static string GetSkillForExtn(string extn)
        //{
        //    try
        //    {
        //        var skillId = skillExtnData.FirstOrDefault(x => x.Key == extn).Value;
        //        return skillId;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        #endregion


    }
}
