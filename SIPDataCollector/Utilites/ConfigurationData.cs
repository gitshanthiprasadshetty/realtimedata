using SIPDataCollector.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SIPDataCollector.Utilites
{
    class ConfigurationData
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger.Logger Log = new Logger.Logger(typeof(ConfigurationData));

        /// <summary>
        /// DB Connection string
        /// </summary>
        public static string ConntnString { get; set; }

        /// <summary>
        /// Total skills to monitor with comma seperated values.
        /// </summary>
        public static string skillsToMonitor { get; set; }

        /// <summary>
        /// Dashboard refresh time
        /// </summary>
        public static int DashboardRefreshTime { get; set; }

        /// <summary>
        /// Array of skills
        /// </summary>
        public static string[] skillList { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        static readonly Dictionary<string, List<string>> channelObj = new Dictionary<string, List<string>>();

        internal static List<string> auxCodes;

        internal static int acceptableSL;

        /// <summary>
        /// Dashboard refresh time
        /// </summary>
        public static int DBRefreshTime { get; set; }
        /// <summary>
        /// Method to load all data from config.
        /// </summary>
        public static void LoadConfig()
        {
            Log.Debug("BcmsSIPManager.ConfigurationData[LoadConfig]");
            try
            {
                ConntnString = ConfigurationSettings.AppSettings["CMDbConn"].ToString();
                skillsToMonitor = ConfigurationSettings.AppSettings["skillsToMonitorForSIP"].ToString();
                DashboardRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DashboardRefreshTime"]);
                skillList = skillsToMonitor.Split(',');
                auxCodes = DataAccess.GetAuxCodes();
                acceptableSL = Convert.ToInt32(ConfigurationSettings.AppSettings["acceptableSL"]);
                DBRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DBRefreshTime"]);
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsSIPManager.ConfigurationData[LoadConfig] : " + ex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        //static void Channel()
        //{
        //    Log.Debug("Channel");
        //    try
        //    {
        //        channelObj.Clear();
        //        var section = (BcmsSIPConfigSection)ConfigurationManager.GetSection("BCMSServiceSettings");
        //        foreach (BCMSInstanceData data in section.BCMSServiceItems)
        //        {
        //            channelObj.Add(data.ChannelName, data.SkillId.Split(',').ToList());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Error in ConfigurationData[Channel] : " + ex);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        //public static string GetChannel(string skillId)
        //{
        //    Log.Debug("GetChannel");
        //    try
        //    {
        //        var result = channelObj.FirstOrDefault(x => x.Value.Contains(skillId)).Key;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Error in GetChannel: for skillId = : " + skillId + Environment.NewLine + ex);
        //        return null;
        //    }
        //}
    }
}
