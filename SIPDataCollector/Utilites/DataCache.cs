using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                log.Error("Error in UpdateCacheData : " , ex);
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
                if(data != null)
                {
                    if(CacheObj.TryGetValue(data.skillId, out RealtimeData values))
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
                }
                log.Debug("GetBcmsData BcmsDashboard Return Count for sip : " + _listObj.Count);
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
                    log.Debug("DataCache[GetBcmsDataForSkill] Dictionary count : " + CacheObj.Count);
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
                log.Error("Error in GetBcmsDataForSkill : " , ex);
                return null;
            }
        }

        #region Bcms Summary



        #endregion

    }
}
