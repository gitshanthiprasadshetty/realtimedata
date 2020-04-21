using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMDataCollector.Models;
using CMDataCollector.Utilities;

namespace CMDataCollector
{
    public class CMDataService : IRealtimeDataService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger.Logger Log = new Logger.Logger(typeof(CMDataService));

        /// <summary>
        /// Gets Bcms data for all skills
        /// </summary>
        /// <returns>returns Bcms data for all skills</returns>
        public List<RealtimeData> MonitorBcms()
        {
            Log.Debug("MonitorBcms()");
            try
            {
                return CacheMemory.GetBcmsData();
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsDashboardService[MonitorBcms] : " + ex);
                return new List<RealtimeData>();
            }
        }

        /// <summary>
        /// Gets Bcms data for given skill
        /// </summary>
        /// <param name="skillToMonitor">skillId</param>
        /// <returns>returns Bcms data for given skill</returns>
        public RealtimeData MonitorBcmsForSkill(string skillToMonitor)
        {
            Log.Debug("MonitorBcmsForSkill()");
            try
            {
                Log.Debug("BcmsDashboardService[MonitorBcmsForSkill] : skillToMonitor = " + skillToMonitor);
                return CacheMemory.GetBcmsDataForSkill(skillToMonitor);
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsDashboardService[MonitorBcmsForSkill] : " + ex);
                return new RealtimeData();
            }
        }

        public List<BcmsSystem> MonitorBcmsSystem(string skillName)
        {
            Log.Debug("MonitorBcmsForSkill()");
            try
            {
                Log.Debug("BcmsDashboardService[MonitorBcmsForSkill] : skillToMonitor = " + skillName);
                return CacheMemory.GetBcmsSystemData(skillName);
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsDashboardService[MonitorBcmsForSkill] : " + ex);
                return new List<BcmsSystem>();
            }
        }

        /// <summary>
        /// Gives data on trunk traffic
        /// </summary>
        /// <returns></returns>
        public List<TrunkGroupTraffic> MonitorTrunkTraffic()
        {
            try
            {
                Log.Debug("BcmsDashboardService[MonitorTrunkTraffic] :");
                return CacheMemory.GetTrunkTrafficDetails();
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsDashboardService[MonitorTrunkTraffic] : " + ex);
                return new List<TrunkGroupTraffic>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<HuntGroupTraffic> MonitorAllHuntTraffics()
        {
            try
            {
                Log.Debug("MonitorAllHuntTraffics :");
                return CacheMemory.GetHuntTrafficDetails();
            }
            catch (Exception ex)
            {
                Log.Error("Error in MonitorAllHuntTraffics : " + ex);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HuntGroupTraffic MonitorHuntTraffic(string huntGroupNumber)
        {
            try
            {
                Log.Debug("MonitorHuntTraffic :");
                var result = CacheMemory.GetHuntTrafficDetails();
                return result?.FirstOrDefault(x => x.HuntGroup == huntGroupNumber);
            }
            catch (Exception ex)
            {
                Log.Error("Error in MonitorHuntTraffic : " + ex);                
            }
            return null;
        }

        /// <summary>
        /// Status of current service
        /// When service is up = > true, down => no output
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return true;
        }

        /// <summary>
        /// Gives value on service conectivity to CM
        /// if connectToCM variable is true => service is connected to CM
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToCM()
        {
            Log.Debug("BcmsDashboardService[IsConnectedToCM] :");
            try
            {
                return CMConnectionManager.GetInstance().connectToCM;
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsDashboardService[IsConnectedToCM] :" + ex);
                return false;
            }
        }

        #region Not used

        //public void StartBCMS()
        //{
        //    try
        //    {
        //        CMConnectionManager.GetInstance().Start();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        #endregion
    }
}
