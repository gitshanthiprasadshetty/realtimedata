using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml;
using SIPDataCollector.Utilities;
using System.Data;
using Connector.DbLayer;

namespace CMDataCollector.Utilities
{
    //*note: while doing dynamic skill loading , make sure that we pass comma seperated valus to 'skillsToMonitor' variable .
    public class ConfigurationData
    {
        #region Config Fields
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger.Logger Log = new Logger.Logger(typeof(ConfigurationData));    

        /// <summary>
        /// IP Address of CM
        /// </summary>
        public static string ServerAddress { get; set;}

        /// <summary>
        /// CM Login username
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// CM Login Password
        /// </summary>
        public static string Password { get; set;}

        /// <summary>
        /// CM connection Port
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// Total skills to monitor to get bcms data
        /// </summary>
        public static string skillsToMonitor { get; set; }

        /// <summary>
        /// Total commands to run :{bcms='monitor bcms skill n', trunk='monitor traffic trunk-groups'}
        /// </summary>
        public static string[] CommandsToRun { get; set; }

        /// <summary>
        /// total number of skills to be taken per connection
        /// </summary>
        public static int skillsPerConnection { get; set; }

        /// <summary>
        /// total skilllist to be monitored is stored in array
        /// </summary>
        public static string[] skillList { get; set; }

        /// <summary>
        /// Dashboard refresh time
        /// </summary>
        public static int DashboardRefreshTime { get; set; }

        /// <summary>
        /// Historical report refresh time
        /// </summary>
        public static int ReportRefreshTime { get; set; }

        /// <summary>
        /// HA
        /// </summary>
        public static int HAEnabled { get; set; }

        /// <summary>
        /// Maximum tries to try on connection failure to CM
        /// </summary>
        public static int MaxTriesOnCMConFailure { get; set; }

        /// <summary>
        /// Action to be taken when Connection to CM fails
        /// </summary>
        public static string ActionOnCMConFailure { get; set; }

        /// <summary>
        /// Connection type {either CM or SIP}
        /// </summary>
        public static string ConnectionType { get; set; }

        /// <summary>
        /// Db connection string
        /// </summary>
        public static string CMConntnString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        static readonly Dictionary<string, List<string>> channelObj = new Dictionary<string, List<string>>();

        /// <summary>
        /// Holds Skill-Extension Related information for given set of skills in config.
        /// </summary>
        static readonly Dictionary<string, SIPDataCollector.Models.SkillExtensionInfo> _skillExtnInfo = new Dictionary<string, SIPDataCollector.Models.SkillExtensionInfo>();

        #endregion

        static string userName = ConfigurationSettings.AppSettings["userName"].ToString();
        static string password = ConfigurationSettings.AppSettings["password"].ToString();        

        /// <summary>
        /// Load all config data
        /// </summary>
        public static void LoadConfig()
        {
            Log.Debug("LoadConfig()");
            try
            {
                Channel();
                DecryptCredentials();
                ServerAddress = ConfigurationSettings.AppSettings["serverAddress"].ToString();
                Port = Convert.ToInt32(ConfigurationSettings.AppSettings["port"]);
                skillsToMonitor = ConfigurationSettings.AppSettings["skillsToMonitor"].ToString();
                skillsPerConnection = Convert.ToInt32(ConfigurationSettings.AppSettings["skillsPerConnection"]);
                skillList = skillsToMonitor.Split(',');
                DashboardRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DashboardRefreshTime"]);
                ReportRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["HistoricalReportRefreshTime"]);
                HAEnabled = Convert.ToInt32(ConfigurationSettings.AppSettings["HAEnabled"]);
                MaxTriesOnCMConFailure = Convert.ToInt32(ConfigurationSettings.AppSettings["MaxTriesOnConnectionFailure"]);
                ActionOnCMConFailure = ConfigurationSettings.AppSettings["ActionOnCMConFailure"].ToLower().ToString();
                ConnectionType = ConfigurationSettings.AppSettings["Type"].ToString();
                CMConntnString = ConfigurationSettings.AppSettings["CMDbConn"].ToString();
                var val = ConfigurationSettings.AppSettings["CommandsToRun"].ToString();
                CommandsToRun = val.Split(',');

                FetchExtenSkillData();
            }
            catch (Exception ex)
            {
                Log.Error("Error in LoadConfig() : " + ex);
            }
        }       

        /// <summary>
        /// Gives total number of connections to be established to CM.
        /// </summary>
        public static int GetNumberOfConnectionsToEstablish()
        {
            Log.Debug("GetNumberOfConnectionsToEstablish()");
            try
            {
                List<int> skills = new List<int>();
                var skillList = skillsToMonitor.Split(',');
                foreach (var skil in skillList)
                {
                    skills.Add(Convert.ToInt32(skil));
                }

                double skillListCount = ((double)skills.Count() / skillsPerConnection);

                string s = skillListCount.ToString("0.00");
                string[] sl = s.Split('.');

                int totalConnections = 0;
                if (Convert.ToInt32(sl[1]) > 0 && Convert.ToInt32(sl[0]) > 0) totalConnections = Convert.ToInt32(sl[0]) + 1;
                if (Convert.ToInt32(sl[1]) > 0 && Convert.ToInt32(sl[0]) < 0) totalConnections = 1;
                if (Convert.ToInt32(sl[1]) == 0 && Convert.ToInt32(sl[0]) > 0) totalConnections=Convert.ToInt32(sl[0]);

                return totalConnections;
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetNumberOfConnectionsToEstablish() : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// Decrypts the username and password
        /// </summary>
        public static void DecryptCredentials()
        {
            Log.Debug("DecryptCredentials()");
            try
            {
                // try decrypting if fails pass the actual value in config.

                string tun = ConfigurationProvider.StringEncryptor.Decrypt(userName);
                if (tun != null)
                    UserName = tun;
                else
                    UserName = userName;

                string tpw = ConfigurationProvider.StringEncryptor.Decrypt(password);
                if (tpw != null)
                    Password = tpw;
                else
                    Password = password;
            }
            catch (Exception ex)
            {
                Log.Error("Error in DecryptCredentials() : " + ex);
            }
        }

        /// <summary>
        /// Map skills based on channel and stores as in-memory data.
        /// </summary>
        public static void Channel()
        {
            Log.Debug("Channel");
            try
            {
                // clear the in-memory data.
                channelObj.Clear();
                var section = (ConfigSection)ConfigurationManager.GetSection("TRealTimeDataServiceSettings");
                foreach (BCMSInstanceData data in section.BCMSServiceItems)
                {
                    Log.Debug("Add skills for channel : " + data.ChannelName);
                    channelObj.Add(data.ChannelName, data.SkillId.Split(',').ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in Channel() : " + ex);
            }
        }
        
        /// <summary>
        /// Gets the channel for given skill
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>return channel name for given skillId</returns>
        public static string GetChannel(string skillId)
        {
            Log.Debug("GetChannel()" + skillId);
            try
            {
                var result = "";
                if (channelObj != null)
                    result = channelObj.FirstOrDefault(x => x.Value.Contains(skillId)).Key;

                Log.Debug("GetChannel return value = " + result);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetChannel(): for skillId = : " + skillId + Environment.NewLine + ex);
                return "";
            }
        }

        /// <summary>
        /// skill to extension map
        /// </summary>
        static void FetchExtenSkillData()
        {
            try
            {
                Log.Debug("FetchExtenSkillData()");
                if (channelObj != null)
                {
                    var emailSkillIds = channelObj.FirstOrDefault(x => x.Key.ToLower() == "email").Value.ToList();
                    if (emailSkillIds != null)
                    {
                        var emailSkills = string.Join(",", emailSkillIds);
                        if (!string.IsNullOrWhiteSpace(emailSkills))
                        {
                            string sql = @"select SkillID,SkillExtension,SkillName from TMAC_Skills with (nolock) Where SkillID in (" + emailSkills + ")";
                            //DataTable result = CMDataCollector.Utilities.SqlDataAccess.ExecuteDataTable(sql,);
                            DataTable result = SqlDataAccess.ExecuteDataTable(sql, CMConntnString);
                            if (result != null)
                            {
                                Log.Debug("Skill to Extension mapping");
                                foreach (DataRow entry in result.Rows)
                                {
                                    // add to dictionary object to maintain a skill-Extn mapping.
                                    _skillExtnInfo.Add(entry.ItemArray[1].ToString(), new SIPDataCollector.Models.SkillExtensionInfo
                                    {
                                        SkillId = entry.ItemArray[0].ToString(),
                                        ExtensionId = entry.ItemArray[1].ToString(),
                                        SkillName = entry.ItemArray[2].ToString()
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in FetchExtenSkillData: " + ex);
            }
        }

        public static string GetExtensionId(string skillId)
        {
            Log.Debug("GetExtensionId()" + skillId);
            try
            {
                if (_skillExtnInfo != null)
                    return _skillExtnInfo.FirstOrDefault(x => x.Value.SkillId == skillId).Key;
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetExtensionId: " + ex);
            }
            return null;
        }

        #region Tobe used later

        ///// <summary>
        ///// To be implemented later.
        ///// </summary>
        //private void SkillListToMonitor()
        //{
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        var skillsToMonitor = ConfigurationSettings.AppSettings["xmlFilePath"].ToString();
        //        doc.Load(skillsToMonitor);
        //        List<int> skillList = new List<int>();
        //        XmlNodeList elemList = doc.GetElementsByTagName("value");
        //        for (int i = 0; i < elemList.Count; i++)
        //        {
        //            // Console.WriteLine(elemList[i].InnerXml);
        //            skillList.Add(Convert.ToInt32(elemList[i].InnerXml));
        //        }  
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        #endregion
    }
}