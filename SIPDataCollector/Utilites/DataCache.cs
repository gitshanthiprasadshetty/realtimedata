using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIPDataCollector.Models;

namespace SIPDataCollector.Utilites
{
    class DataCache
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger.Logger log = new Logger.Logger(typeof(DataCache));

        /// <summary>
        /// Holds Bcms Dashboard data for SIP
        /// </summary>
        static readonly ConcurrentDictionary<int, RealtimeData> CacheObj = new ConcurrentDictionary<int, RealtimeData>();

        /// <summary>
        /// 
        /// </summary>
        static RealtimeData _bcmsObj;

        /// <summary>
        /// 
        /// </summary>
        static List<RealtimeData> _listObj;

        /// <summary>
        /// Updates main cache data for each skill
        /// </summary>
        /// <param name="data">bcms data to update</param>
        public static void UpdateCacheData(Object data)
        {
            log.Info("UpdateCacheData()");
            try
            {
                _bcmsObj = (RealtimeData)data;
                if (data != null)
                {
                    if (CacheObj.ContainsKey(_bcmsObj.SkillId))
                    {
                        var value = CacheObj.FirstOrDefault(x => x.Key == _bcmsObj.SkillId).Value;
                        CacheObj.TryUpdate(value.SkillId, _bcmsObj, value);
                    }
                    else
                        CacheObj.TryAdd(_bcmsObj.SkillId, _bcmsObj);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in UpdateCacheData : ", ex);
            }
        }

        public static void UpdateCacheData(List<RealtimeData> itemList)
        {
            log.Info("UpdateCacheData()");
            try
            {
                if (itemList != null)
                {
                    log.Info($"total items to update to cache memeory = {itemList.Count}");
                    foreach (var item in itemList)
                    {
                        if (CacheObj.ContainsKey(item.SkillId))
                        {
                            var value = CacheObj.FirstOrDefault(x => x.Key == item.SkillId).Value;

                            if (value != null)
                            {
                                // if object is already present, we need to consider below feilds from cache and update to object, 
                                // else values will be update as zero.
                                item.AverageHandlingTime = value.AverageHandlingTime;
                                item.SLPercentage = value.SLPercentage;
                                item.AbandonedInteractionsSummary = value.AbandonedInteractionsSummary;
                                item.ActiveInteractionsSummary = value.ActiveInteractionsSummary;
                                item.AverageAbandonedTime = value.AverageAbandonedTime;
                                item.AbandonPercentage = value.AbandonPercentage;
                                item.AverageACWTime = value.AverageACWTime;
                                item.AverageHoldTime = value.AverageHoldTime;
                                item.OldestInteractionWaitTime = value.OldestInteractionWaitTime;

                                CacheObj.TryUpdate(value.SkillId, item, value);
                            }
                        }
                        else
                            CacheObj.TryAdd(item.SkillId, item);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in UpdateCacheData : ", ex);
            }
        }

        /// <summary>
        /// updates cache with historical data
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateHistoricalData(SkillData data)
        {
            try
            {
                log.Debug("UpdateHistoricalData : for skill = " + data.skillId);
                if (data != null)
                {
                    if (CacheObj.TryGetValue(data.skillId, out RealtimeData values))
                    {
                        log.Debug("Updating with histoircaldata for skill = " + data.skillId);
                        // RealtimeData oldValues = values;                        
                        // values.AverageHandlingTime = data.AvgHandlingTime;
                        values.AverageHandlingTime = data.AvgHandlingTime;
                        values.SLPercentage = data.SLPercentage;
                        values.AbandonedInteractionsSummary = data.AbandCalls;
                        //log.Debug("ActiveInteractionsSummary before update = " + values.ActiveInteractionsSummary);
                        values.ActiveInteractionsSummary = data.TotalACDInteractions;
                        //log.Debug("ActiveInteractionsSummary after update = " + values.ActiveInteractionsSummary);
                        values.AverageAbandonedTime = data.AvgAbandTime;
                        values.AbandonPercentage = data.AbandonPercentage;
                        CacheObj.TryUpdate(data.skillId, values, values);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in UpdateHistoricalData : ", ex);
            }
        }

        /// <summary>
        /// Gets all bcms data for configured skillids
        /// </summary>
        /// <returns>All bcms data for configured skillids</returns>
        public static List<RealtimeData> GetBcmsData()
        {
            log.Debug("GetBcmsData()");
            try
            {
                if (CacheObj != null && CacheObj.Count > 0)
                {
                    _bcmsObj = new RealtimeData();
                    _listObj = new List<RealtimeData>();

                    foreach (KeyValuePair<int, RealtimeData> entry in CacheObj)
                    {
                        _bcmsObj = entry.Value;
                        _listObj.Add(_bcmsObj);
                    }
                    log.Debug("GetBcmsData BcmsDashboard Return Count for sip : " + _listObj.Count);
                    return _listObj;
                }
                log.Debug("GetBcmsData BcmsDashboard No data to return");
                return _listObj;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetBcmsData : ", ex);
                return null;
            }
        }

        /// <summary>
        /// Gets bcms data for requested skillid
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>Single bcms data for requested skillid</returns>
        public static RealtimeData GetBcmsDataForSkill(int skillId)
        {
            log.Info($"GetBcmsDataForSkill() , skillId = {skillId}");
            try
            {
                if (CacheObj != null && CacheObj.Count > 0)
                {
                    log.Debug("DataCache[GetBcmsDataForSkill] Dictionary count : " + CacheObj?.Count);
                    _bcmsObj = new RealtimeData();
                    var result = CacheObj.FirstOrDefault(x => x.Key == skillId);
                    if (!result.Equals(default(KeyValuePair<string, RealtimeData>)))
                    {
                        _bcmsObj = CacheObj[result.Key];
                    }
                }
                return _bcmsObj;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetBcmsDataForSkill : ", ex);
                return null;
            }
        }

        /// <summary>
        /// Below methods updated summarydata to the cacheobj for that skillID which is retrived from SkillExtensionObj for skillExtension
        /// </summary>
        /// <param name="dataTable"></param>
        public static void UpdateSummaryData(DataTable dataTable)
        {
            try
            {
                log.Info("UpdateSummaryData()");
                int sl = ConfigurationData.acceptableSL;
                int skillId = 0, activeTime, HoldTime, AcwTime, callsWithinSLA, ACDSummary, waitTime;
                List<string> skillsToMonitor = ConfigurationData.skillList;
                //decimal slPercentage = (decimal)0m;
                if (dataTable != null)
                {
                    log.Info($"DataTable from DB count is {dataTable.Rows.Count}, hence trying to update cacheObj for Abandoned, Active and skillId");
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        skillId = SIPManager.GetInstance().GetSkillExtensionInfo(dataTable.Rows[i][0]);

                        var matchSkill = skillsToMonitor?.FirstOrDefault(x => x.Contains(skillId.ToString()));
                        if (matchSkill != null)
                        {
                            log.Info($"UpdateSummaryData(): {skillId}");

                            if (CacheObj.TryGetValue(skillId, out RealtimeData val))
                            {
                                if (val != null)
                                {
                                    activeTime = val.InteractionsActiveTime + Convert.ToInt32(dataTable.Rows[i][1]);
                                    HoldTime = val.InteractionsHoldTime + Convert.ToInt32(dataTable.Rows[i][2]);
                                    AcwTime = val.InteractionsAcwTime + Convert.ToInt32(dataTable.Rows[i][3]);
                                    ACDSummary = val.ActiveInteractionsSummary + Convert.ToInt32(dataTable.Rows[i][4]);
                                    callsWithinSLA = val.CallsAnsweredWithinSLA + Convert.ToInt32(dataTable.Rows[i][5]);
                                    waitTime = val.TotalInteractionsQueueTime + Convert.ToInt32(dataTable.Rows[i][6]);
                                    val.InteractionsActiveTime = activeTime;
                                    val.InteractionsHoldTime = HoldTime;
                                    val.InteractionsAcwTime = AcwTime;
                                    val.ActiveInteractionsSummary = ACDSummary;
                                    val.CallsAnsweredWithinSLA = callsWithinSLA;
                                    val.TotalInteractionsQueueTime = waitTime;
                                    val.AverageHandlingTime = (val.InteractionsActiveTime+val.InteractionsHoldTime) / (val.ActiveInteractionsSummary > 0 ? val.ActiveInteractionsSummary : 1);
                                    val.AverageHoldTime = val.InteractionsHoldTime / (val.ActiveInteractionsSummary > 0 ? val.ActiveInteractionsSummary : 1);
                                    val.AverageACWTime = val.InteractionsAcwTime / (val.ActiveInteractionsSummary > 0 ? val.ActiveInteractionsSummary : 1);
                                    val.AverageWaitingTime = val.TotalInteractionsQueueTime / (val.ActiveInteractionsSummary > 0 ? val.ActiveInteractionsSummary : 1);
                                    CacheObj.TryUpdate(skillId, val, val);
                                }
                            }
                            else
                            {
                                CacheObj.AddOrUpdate(skillId, new RealtimeData
                                {
                                    InteractionsActiveTime = Convert.ToInt32(dataTable.Rows[i][1]),
                                    InteractionsHoldTime = Convert.ToInt32(dataTable.Rows[i][2]),
                                    InteractionsAcwTime = Convert.ToInt32(dataTable.Rows[i][3]),
                                    ActiveInteractionsSummary = Convert.ToInt32(dataTable.Rows[i][4]),
                                    CallsAnsweredWithinSLA = Convert.ToInt32(dataTable.Rows[i][5]),
                                    SkillId = skillId,
                                    TotalInteractionsQueueTime = Convert.ToInt32(dataTable.Rows[i][6]),
                                    AverageHandlingTime = (Convert.ToInt32(dataTable.Rows[i][1])+ Convert.ToInt32(dataTable.Rows[i][2])) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                    AverageHoldTime = Convert.ToInt32(dataTable.Rows[i][2]) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                    AverageACWTime = Convert.ToInt32(dataTable.Rows[i][3]) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                    AverageWaitingTime = Convert.ToInt32(dataTable.Rows[i][6]) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                },
                                (k, v) => new RealtimeData
                                {
                                    InteractionsActiveTime = Convert.ToInt32(dataTable.Rows[i][1]),
                                    InteractionsHoldTime = Convert.ToInt32(dataTable.Rows[i][2]),
                                    InteractionsAcwTime = Convert.ToInt32(dataTable.Rows[i][3]),
                                    ActiveInteractionsSummary = Convert.ToInt32(dataTable.Rows[i][4]),
                                    CallsAnsweredWithinSLA = Convert.ToInt32(dataTable.Rows[i][5]),
                                    SkillId = skillId,
                                    TotalInteractionsQueueTime = Convert.ToInt32(dataTable.Rows[i][6]),
                                    AverageHandlingTime = (Convert.ToInt32(dataTable.Rows[i][1]) + Convert.ToInt32(dataTable.Rows[i][2])) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                    AverageHoldTime = Convert.ToInt32(dataTable.Rows[i][2]) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                    AverageACWTime = Convert.ToInt32(dataTable.Rows[i][3]) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                    AverageWaitingTime = Convert.ToInt32(dataTable.Rows[i][6]) / (Convert.ToInt32(dataTable.Rows[i][4]) > 0 ? Convert.ToInt32(dataTable.Rows[i][4]) : 1),
                                }) ;
                            }
                        }
                        else
                        {
                            log.Info($"Retrived skill {skillId} is not configured in TRealtimeData config to monitor");
                        }
                    }
                }
                log.Info($"Total CacheObj count {CacheObj?.Count()}");
            }
            catch (Exception ex)
            {
                log.Error($"Exception in UpdateSummaryData(): {ex}");
            }
        }

        /// <summary>
        /// The below method updates acd interactions to the cacheobj recevied from the VDN monitor for that ksillID
        /// </summary>
        /// <param name="skillId"></param>
        public static void UpdateActiveInteraction(int skillId)
        {
            try
            {
                log.Info($"UpdateActiveInteraction():  SkillID= {skillId}");
                CacheObj.TryGetValue(skillId, out RealtimeData oldValue);
                if (oldValue != null)
                {
                    log.Info($"Incrementing ActiveInteractions in the cache obj: {oldValue.ActiveInteractionsSummary}");
                    oldValue.ActiveInteractionsSummary++;
                    CacheObj.TryUpdate(skillId, oldValue, oldValue);
                    log.Info($"UpdateActiveInteraction: Done updating to CacheObj, count {CacheObj?.Count()}");
                    return;
                }
                log.Info("No data found in CacheObj to update");
            }
            catch (Exception e)
            {
                log.Error($"Exception in UpdateActiveInteraction(): {e}");
            }
        }

        /// <summary>
        /// Below method updates the asa data to the cacheobj for that skillID
        /// </summary>
        /// <param name="skillId"></param>
        public static void UpdateASAData(int skillId)
        {
            try
            {
                log.Info($"UpdateASAData(): SkillID= { skillId}");
                CacheObj.TryGetValue(skillId, out RealtimeData oldValue);
                if (oldValue != null)
                {
                    log.Info($"Incrementing AvgSpeedAnswer in the cache obj {oldValue.AvgSpeedAnswer}");
                    oldValue.AvgSpeedAnswer++;
                    CacheObj.TryUpdate(skillId, oldValue, oldValue);
                    log.Info($"UpdateASAData: Done updating to CacheObj, count {CacheObj?.Count()}");
                    return;
                }
                log.Info("No data found in CacheObj to update");
            }
            catch (Exception e)
            {
                log.Error($"Exception in UpdateASAData(): {e}");
            }
        }

        /// <summary>
        /// Below method updates the abandon data to the cacheobj for that skillID
        /// </summary>
        /// <param name="skillId"></param>
        public static void UpdateAbandonedCount(int skillId)
        {
            try
            {
                log.Info($"UpdateAbandonedCount(): SkillID= { skillId}");
                CacheObj.TryGetValue(skillId, out RealtimeData oldValue);
                if (oldValue != null)
                {
                    log.Info($"Incrementing AbandonedInteractionsSummary in the cache obj {oldValue.AbandonedInteractionsSummary}");
                    oldValue.AbandonedInteractionsSummary++;
                    CacheObj.TryUpdate(skillId, oldValue, oldValue);
                    log.Info($"UpdateAbandonedCount: Done updating to CacheObj, count {CacheObj?.Count()}");
                    return;
                }
                log.Info("No data found in CacheObj to update");
            }
            catch (Exception e)
            {
                log.Error($"Exception in UpdateAbandonedCount(): {e}");
            }
        }

        public static void UpdateCacheObj(List<DataModel.AgentSessionDataModel> agentSkillInfo)
        {
            try
            {
                log.Info($"UpdateCacheObj()");
                int i = 0, queueTime = 0, skill, callSlaCount;
                int sl = ConfigurationData.acceptableSL;
                if (agentSkillInfo != null)
                {
                    foreach (var data in agentSkillInfo)
                    {
                        callSlaCount = 0;
                        queueTime = data.InteractionList.Count() > 0 ? data.InteractionList[i].InteractionData.QueueTime : 0;
                        //queueTime = data.InteractionList[i].InteractionData.QueueTime;
                        skill = Convert.ToInt32(data.InteractionList[i].InteractionData.Skill);
                        if (queueTime < sl)
                        {
                            log.Info("Queued time is less than SL");
                            if (CacheObj.TryGetValue(skill, out RealtimeData values))
                            {
                                if (values != null)
                                {
                                    callSlaCount = values.CallsAnsweredWithinSLA++;
                                    log.Info($"CallSLACount for {skill} is {callSlaCount}");
                                }
                            }
                        }
                        CacheObj.AddOrUpdate(skill, new RealtimeData
                        {
                            CallsAnsweredWithinSLA = callSlaCount,
                            InteractionsQueueTime = queueTime
                        },
                    (k, v) => new RealtimeData
                    {
                        CallsAnsweredWithinSLA = callSlaCount,
                        InteractionsQueueTime = queueTime
                    });
                    }
                }
                log.Info($"CacheObj updated count is {CacheObj?.Count()}");
            }
            catch (Exception ex)
            {
                log.Error($"Exception in UpdateCache() {ex}");
            }
        }

        public static RealtimeData UpdateFetchBcmsData(RealtimeData data)
        {
            try
            {
                log.Info("UpdateFetchBcmsData");
                if (CacheObj.TryGetValue(data.SkillId, out RealtimeData val))
                {
                    if (val != null)
                    {
                        data.InteractionsActiveTime = val.InteractionsActiveTime;
                        data.InteractionsHoldTime = val.InteractionsHoldTime;
                        data.InteractionsQueueTime = val.InteractionsQueueTime;
                        data.InteractionsAcwTime = val.InteractionsAcwTime;
                        data.CallsAnsweredWithinSLA = val.CallsAnsweredWithinSLA;
                        data.SLPercentage = val.SLPercentage;
                        data.TotalInteractionsQueueTime = val.TotalInteractionsQueueTime;
                        data.AverageACWTime = val.AverageACWTime;
                        data.AverageHoldTime = val.AverageHoldTime;
                        data.AverageWaitingTime = val.AverageWaitingTime;
                        data.AverageHandlingTime = val.AverageHandlingTime;
                        data.OldestInteractionWaitTime = val.OldestInteractionWaitTime;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                log.Error($"Exception in UpdateFetchBcmsData() {ex}");
                return data;
            }
        }

        public static bool ClearCacheObj()
        {
            try
            {
                log.Info("ClearCacheObj()");
                CacheObj.Clear();
                log.Info($"ClearCacheObj() count: {CacheObj?.Count()}");
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"Exception in ClearCacheObj():{ex}");
                return false;
            }
        }
        #region Bcms Summary



        #endregion

    }
}
