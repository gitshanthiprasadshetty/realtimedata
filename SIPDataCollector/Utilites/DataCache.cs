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
        private static Logger.Logger Log = new Logger.Logger(typeof(DataCache));

        /// <summary>
        /// Holds Bcms Dashboard data for SIP
        /// </summary>
        static readonly ConcurrentDictionary<int, RealtimeData> CachObj = new ConcurrentDictionary<int, RealtimeData>();

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
            Log.Debug("DataCache.[UpdateCacheData]");
            try
            {
                _bcmsObj = (RealtimeData)data;
                if (data != null)
                {
                    if (CachObj.ContainsKey(_bcmsObj.SkillId))
                    {
                        var value = CachObj.FirstOrDefault(x => x.Key == _bcmsObj.SkillId).Value;
                        CachObj.TryUpdate(value.SkillId, _bcmsObj, value);
                    }
                    else
                        CachObj.TryAdd(_bcmsObj.SkillId, _bcmsObj);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in DataCache[UpdateCacheData] : " + ex);
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
                Log.Debug("UpdateHistoricalData : for skill = " + data.skillId);
                if(data != null)
                {
                    if(CachObj.TryGetValue(data.skillId, out RealtimeData values))
                    {
                        Log.Debug("Updating with histoircaldata for skill = " + data.skillId);
                        RealtimeData oldValues = values;                        
                        values.AverageHandlingTime = data.AvgHandlingTime;
                        values.SLPercentage = data.SLPercentage;
                        values.AbandonedCalls = data.AbandCalls;
                        values.TotalActiveInteractions = data.TotalACDInteractions;
                        values.AverageAbandonedTime = data.AvgAbandTime;
                        values.AbandonPercentage = data.AbandonPercentage;
                        CachObj.TryUpdate(data.skillId, values, oldValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in UpdateHistoricalData : " + ex);
            }
        }

        /// <summary>
        /// Gets all bcms data for configured skillids
        /// </summary>
        /// <returns>All bcms data for configured skillids</returns>
        public static List<RealtimeData> GetBcmsData()
        {
            Log.Debug("DataCache[GetBcmsData]");
            try
            {
                if (CachObj.Count > 0)
                {
                    Log.Debug("Sip Dictionary count : " + CachObj.Count);
                    _bcmsObj = new RealtimeData();
                    _listObj = new List<RealtimeData>();
                    foreach (KeyValuePair<int, RealtimeData> entry in CachObj)
                    {
                        _bcmsObj = entry.Value;
                        _listObj.Add(_bcmsObj);
                    }
                }
                Log.Debug("DataCache[GetBcmsData] BcmsDashboard Return Count for sip : " + _listObj.Count);
                return _listObj;
            }
            catch (Exception ex)
            {
                Log.Error("Error in sip DataCache[GetBcmsData] : " + ex);
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
            Log.Debug("DataCache[GetBcmsDataForSkill]");
            try
            {
                if (CachObj.Count > 0)
                {
                    Log.Debug("DataCache[GetBcmsDataForSkill] Dictionary count : " + CachObj.Count);
                    _bcmsObj = new RealtimeData();
                    var result = CachObj.FirstOrDefault(x => x.Key == skillId);
                    if (!result.Equals(default(KeyValuePair<string, RealtimeData>)))
                    {
                        _bcmsObj = CachObj[result.Key];
                    }
                }
                return _bcmsObj;
            }
            catch (Exception ex)
            {
                Log.Error("Error in DataCache[GetBcmsDataForSkill] : " + ex);
                return null;
            }
        }

        #region Bcms Summary



        #endregion

    }
}
