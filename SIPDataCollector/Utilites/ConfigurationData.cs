using ConfigurationProvider;
using SIPDataCollector.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

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

        internal static  Dictionary<string, int> acceptableSlObj = new Dictionary<string, int>();

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
                ////ConntnString = ConfigurationSettings.AppSettings["CMDbConn"].ToString();
                ConntnString = ConnectionStrings.DecryptConnectionString(ConfigurationSettings.AppSettings["CMDbConn"]);
                skillsToMonitor = ConfigurationSettings.AppSettings["skillsToMonitorForSIP"];
                DashboardRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DashboardRefreshTime"]);
                acceptableSL = Convert.ToInt32(ConfigurationSettings.AppSettings["acceptableSL"]);
                DBRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DBRefreshTime"]);
                if (!string.IsNullOrEmpty(skillsToMonitor) || skillsToMonitor.ToLower() != "na")
                    skillList = skillsToMonitor.Split(',');
                auxCodes = DataAccess.GetAuxCodes();
                acceptableSlObj = DataAccess.GetAcceptableLevels();
            }
            catch (Exception ex)
            {
                Log.Error("Error in BcmsSIPManager.ConfigurationData[LoadConfig] : " + ex);
            }
        }

        #region notused
        public void GetConnectionString()
        {
            Log.Debug("GetConnectionString()");
            try
            {
                string pwdString = string.Empty , userIdString = string.Empty;
                string decryptedPassword = string.Empty; string decryptedUserID = string.Empty;
                string[] stringarray = ConntnString.Split(';');
                int pwdndex = Array.IndexOf(stringarray, "Password");
                int usrIdndex = Array.IndexOf(stringarray, "Password");
                if (stringarray.Contains("Password"))
                {
                    pwdString = stringarray.ElementAt(pwdndex);
                    decryptedPassword = StringEncryptor.Decrypt(pwdString);
                    if (decryptedPassword == null || String.IsNullOrEmpty(decryptedPassword))
                    {
                        Log.Warn("Failed to decrypt the password");
                        decryptedPassword = pwdString;
                    }
                }
                if (stringarray.Contains("user"))
                {
                    userIdString = stringarray.ElementAt(usrIdndex);
                    decryptedUserID = StringEncryptor.Decrypt(userIdString);
                    if (decryptedUserID == null || String.IsNullOrEmpty(decryptedUserID))
                    {
                        Log.Warn("Failed to decrypt the password");
                        decryptedUserID = userIdString;
                    }
                }
                try
                {
                    string ConnectionString = String.Format(ConntnString, decryptedPassword);
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
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
        #endregion
    }
}
