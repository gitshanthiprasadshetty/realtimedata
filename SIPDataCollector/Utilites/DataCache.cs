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
        static readonly ConcurrentDictionary<string, BcmsDataForSIP> CachObj = new ConcurrentDictionary<string, BcmsDataForSIP>();

        /// <summary>
        /// 
        /// </summary>
        static BcmsDataForSIP _bcmsObj;

        /// <summary>
        /// 
        /// </summary>
        static List<BcmsDataForSIP> _listObj;

        /// <summary>
        /// Updates main cache data for each skill
        /// </summary>
        /// <param name="data">bcms data to update</param>
        public static void UpdateCacheData(Object data)
        {
            Log.Debug("DataCache.[UpdateCacheData]");
            try
            {
                _bcmsObj = (BcmsDataForSIP)data;
                if (data != null)
                {
                    if (CachObj.ContainsKey(_bcmsObj.Skill))
                    {
                        var value = CachObj.FirstOrDefault(x => x.Key == _bcmsObj.Skill).Value;
                        CachObj.TryUpdate(value.Skill, _bcmsObj, value);
                    }
                    else
                        CachObj.TryAdd(_bcmsObj.Skill, _bcmsObj);
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
                Log.Debug("UpdateHistoricalData : for skill = " + data.skillID);
                if(data != null)
                {
                    if(CachObj.TryGetValue(data.skillID, out BcmsDataForSIP values))
                    {
                        Log.Debug("Updating with histoircaldata for skill = " + data.skillID);
                        BcmsDataForSIP oldValues = values;                        
                        values.AvgHandlingTime = data.AvgHandlingTime;
                        values.SLPercentage = Convert.ToString(data.SLPercentage);
                        values.AbandCalls = Convert.ToString(data.AbandCalls);
                        values.TotalACDInteractions = Convert.ToString(data.TotalACDInteractions);
                        values.AvgAbandTime = data.AvgAbandTime;
                        values.AbandonPercentage = data.AbandonPercentage;
                        CachObj.TryUpdate(data.skillID, values, oldValues);
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
        public static List<BcmsDataForSIP> GetBcmsData()
        {
            Log.Debug("DataCache[GetBcmsData]");
            try
            {
                if (CachObj.Count > 0)
                {
                    Log.Debug("Sip Dictionary count : " + CachObj.Count);
                    _bcmsObj = new BcmsDataForSIP();
                    _listObj = new List<BcmsDataForSIP>();
                    foreach (KeyValuePair<string, BcmsDataForSIP> entry in CachObj)
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
        public static BcmsDataForSIP GetBcmsDataForSkill(string skillId)
        {
            Log.Debug("DataCache[GetBcmsDataForSkill]");
            try
            {
                if (CachObj.Count > 0)
                {
                    Log.Debug("DataCache[GetBcmsDataForSkill] Dictionary count : " + CachObj.Count);
                    _bcmsObj = new BcmsDataForSIP();
                    var result = CachObj.FirstOrDefault(x => x.Key == skillId);
                    if (!result.Equals(default(KeyValuePair<string, BcmsDataForSIP>)))
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
