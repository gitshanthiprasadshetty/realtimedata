using ConfigurationProvider;
using Connector.Proxy;
using Connector.SMSAPI;
using SIPDataCollector.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Timers;
using Newtonsoft.Json;
using System.Globalization;

namespace SIPDataCollector.Utilites
{
    class ConfigurationData
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger.Logger log = new Logger.Logger(typeof(ConfigurationData));

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
        public static List<string> skillList { get; set; }
        public static string jsonPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        static readonly Dictionary<string, List<string>> channelObj = new Dictionary<string, List<string>>();

        internal static List<string> auxCodes;

        internal static List<string> TmacServers;

        internal static Dictionary<string, int> acceptableSlObj = new Dictionary<string, int>();

        internal static int acceptableSL;

        public static List<int> totalSkillIds = new List<int>();
        public static int ReloadConfigTime { get; set; }
        public static string GetAgentSkillInfoConfig { get; set; }
        /// <summary>
        /// Dashboard refresh time
        /// </summary>
        public static int DBRefreshTime { get; set; }
        public static bool isVDN { get; set; }
        public static List<string> ExtraAUXCodes { get; set; }
        public static DateTime  LastExecutedTime { get; set; }
        /// <summary>
        /// Method to load all data from config.
        /// </summary>
        public static void LoadConfig()
        {
            log.Debug("LoadConfig()");
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                Channel();
                skillsToMonitor = SectionSkills();
                ConntnString = ConnectionStrings.DecryptConnectionString(ConfigurationSettings.AppSettings["CMDbConn"]);
                DashboardRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DashboardRefreshTime"]);
                acceptableSL = Convert.ToInt32(ConfigurationSettings.AppSettings["acceptableSL"]);
                DBRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DBRefreshTime"]);
                TmacServers = ConfigurationSettings.AppSettings["TmacServers"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                ReloadConfigTime = Convert.ToInt32(ConfigurationManager.AppSettings["ReloadConfigTime"]);
                jsonPath = ConfigurationManager.AppSettings["JsonFilePath"];
                GetAgentSkillInfoConfig = ConfigurationManager.AppSettings["GetAgentSkillInfo"];
                isVDN = Convert.ToBoolean(ConfigurationManager.AppSettings["isVDNMonitor"]);
                ExtraAUXCodes = ConfigurationManager.AppSettings["ExtraAUXCodes"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                skillList = FormatSkills(skillsToMonitor);
                string s = string.Join(",", skillList); //converting list of skillList to string
                log.Info($"Monitoring {s}");

                auxCodes = DataAccess.GetAuxCodes();
                auxCodes.AddRange(ExtraAUXCodes);
                acceptableSlObj = DataAccess.GetAcceptableLevels();

                // load json data to file.
                LoadDataFromFileToMemory();
            }
            catch (Exception ex)
            {
                log.Error("Error in BcmsSIPManager.ConfigurationData[LoadConfig] : " + ex);
            }
        }

        /// <summary>
        /// Loading JSON file from jsonPath to load to cache memory
        /// </summary>
        static void LoadDataFromFileToMemory()
        {
            log.Info("LoadDataFromFileToMemory()");
            try
            {
                if (Directory.Exists(jsonPath))
                {
                    using (StreamReader r = new StreamReader(jsonPath))
                    {
                        string json = r.ReadToEnd();
                        List<RealtimeData> itemToLoadToCacheMemory = JsonConvert.DeserializeObject<List<RealtimeData>>(json);
                        return;
                    }
                }
                log.Info("Configured JsonPath does not exist. Please provide the proper path");
            }
            catch (Exception ex)
            {
                log.Error("Error in LoadDataFromFileToMemory: " + ex);
            }
        }


        public static void LoadDataFromDatabase()
        {
            log.Info("LoadDataFromDatabase()");
            try
            {
                DateTime dt = new DateTime();
                string date = DateTime.Now.ToString("yyyyMMdd");
                string startTime = DateTime.Today.ToString("HHmmss");
                string endTime = dt.Date.AddDays(1).AddTicks(-1).ToString("HHmmss");
                // on app load get data from database by executing function.
                SIPManager.GetInstance().GetSummaryData(new DateTime(),date, startTime, endTime);
                LastExecutedTime = DateTime.Now;
                // after loading from database, push that data to cacheobj
            }
            catch (Exception ex)
            {
                log.Error("Error in LoadDataFromDatabase: " + ex);
            }
        }

        /// <summary>
        /// This method is called for every dashboard configured time if vdnmonitor is set to false. This will get the data from workqueuedata function
        /// </summary>
        public static void LoadDataFromDatabaseForInterval()
        {
            log.Info("LoadDataFromDatabaseForInterval()");
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                string endTime = DateTime.Now.ToString("HHmmss");
                DateTime endDateTime = DateTime.Now;
                
                DateTime startTime = LastExecutedTime;

                if(LastExecutedTime.Date < endDateTime.Date)
                {
                    log.Info("LastExecutedTime is less than endDateTime");
                    startTime = LastExecutedTime.Date.AddDays(1).AddTicks(1).AddSeconds(1);
                }
                // on app load get data from database by executing function.
                SIPManager.GetInstance().GetSummaryData(endDateTime,date, startTime.ToString("HHmmss"), endTime);
                // after loading from database, push that data to cacheobj
            }
            catch (Exception ex)
            {
                log.Error("Error in LoadDataFromDatabase: " + ex);
            }
        }
        public static void Channel()
        {
            log.Debug("Channel");
            try
            {
                // clear the in-memory data.
                channelObj.Clear();
                List<int> nums = new List<int>();
                ConfigurationManager.RefreshSection("TRealTimeDataServiceSettings");
                var section = (SIPDataCollector.Utilites.BcmsSIPConfigSection)ConfigurationManager.GetSection("TRealTimeDataServiceSettings");
                foreach (BCMSInstanceData data in section.BCMSServiceItems)
                {
                    log.Debug("Add skills for channel : " + data.ChannelName);
                    string[] strArrays = data.SkillId?.Split(new char[] { ';' });
                    nums.Clear();
                    if (strArrays[0] != "")
                    {
                        for (int i = 0; i < (int)strArrays.Length; i++)
                        {
                            if (!strArrays[i].Contains("-")) //added to check if the skill is added like 100;101-110. this will modify it to 100-100;101-110 Nov 13,2019
                            {
                                strArrays[i] = strArrays[i] + "-" + strArrays[i];
                            }
                            nums.AddRange(Enumerable.Range(Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]),
                                Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[1]) - Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]) + 1));
                        }
                        List<string> l2 = nums.ConvertAll<string>(delegate (int i) { return i.ToString(); });
                        channelObj.Add(data.ChannelName, l2.Distinct().ToList());
                    }
                }
                log.Info($"Channel Obj count is {channelObj.Count()}");
            }
            catch (Exception ex)
            {
                log.Error("Error in Channel() : " + ex);
            }
        }

        public static string GetChannel(string skillId)
        {
            log.Debug("GetChannel() " + skillId);
            try
            {
                var result = "";
                if (channelObj != null)
                    result = channelObj.FirstOrDefault(x => x.Value.Contains(skillId)).Key;

                log.Debug("GetChannel return value = " + result);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetChannel(): for skillId = : " + skillId + Environment.NewLine + ex);
                return "";
            }
        }

        private static List<string> FormatSkills(string skillList)
        {
            try
            {
                log.Info($"FormatSkills() {skillList}");
                if (skillList != null && skillList.Count() >= 0 && !string.IsNullOrEmpty(skillList))
                {
                    string[] strArrays = skillList?.Split(new char[] { ';' });
                    if ((strArrays != null ? true : strArrays.Length != 0))
                    {
                        if (strArrays[0] != "")
                        {
                            List<HuntGroupType> result = SMSAPIProxy.GetSkills();

                            if (result != null)
                            {
                                var totalIdsToMonitor = channelObj.SelectMany(x => x.Value).ToList();
                                totalSkillIds = totalIdsToMonitor.Select((x => Convert.ToInt32((x)))).ToList();
                                log.Info("Total Skills Obtained from SMSAPI : " + result.Count);
                                List<int> Skills = result.Select(x => Convert.ToInt32(x.group_NumberField)).ToList();
                                log.Info("Total Skills from config info : " + totalSkillIds.Count);
                                totalSkillIds = totalSkillIds?.Where(x => Skills.Contains(x)).ToList();
                                log.Info("Total Skills to monitor after applying filter : " + totalSkillIds.Count);
                                return totalSkillIds.Select(x => Convert.ToString(x)).ToList();
                            }
                            else
                                log.Info("No data obtained from smsapi");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("Error : ", exception);
            }
            return null;
        }

        /// <summary>
        /// Below method is used to reload the config section for each interval specified in the config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        public static void RefreshSection(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                log.Info("Refreshing config");
                LoadConfig();
            }
            catch (Exception e)
            {
                log.Error("Exeception in RefreshSection() " + e);
            }

        }

        /// <summary>
        /// Below method gets the skills from the channelobj and appends ',' to make it as a string
        /// </summary>
        /// <returns></returns>
        public static string SectionSkills()
        {
            try
            {
                log.Info("SectionSkills()");
                var skillValue = channelObj.SelectMany(x => x.Value).Distinct().ToList();
                string sectionSkills = string.Empty;
                sectionSkills = String.Join(",", skillValue);
                return sectionSkills;
            }
            catch (Exception e)
            {
                log.Error("Exception in SectionSkills(): " + e);
                return "";
            }
        }



        #region notused
        public void GetConnectionString()
        {
            log.Debug("GetConnectionString()");
            try
            {
                string pwdString = string.Empty, userIdString = string.Empty;
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
                        log.Warn("Failed to decrypt the password");
                        decryptedPassword = pwdString;
                    }
                }
                if (stringarray.Contains("user"))
                {
                    userIdString = stringarray.ElementAt(usrIdndex);
                    decryptedUserID = StringEncryptor.Decrypt(userIdString);
                    if (decryptedUserID == null || String.IsNullOrEmpty(decryptedUserID))
                    {
                        log.Warn("Failed to decrypt the password");
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
        #endregion
    }
}
