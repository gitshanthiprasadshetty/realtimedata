using CMDataCollector.Models;
using ConfigurationProvider;
using Connector.DbLayer;
using Connector.Proxy;
using Connector.SMSAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Timers;

namespace CMDataCollector.Utilities
{
    //*note: while doing dynamic skill loading , make sure that we pass comma seperated valus to 'skillsToMonitor' variable .
    public class ConfigurationData
    {
        #region Config Fields
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger.Logger log = new Logger.Logger(typeof(ConfigurationData));

        /// <summary>
        /// IP Address of CM
        /// </summary>
        public static string ServerAddress { get; set; }

        /// <summary>
        /// CM Login username
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// CM Login Password
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// CM connection Port
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// Total skills to monitor to get bcms data
        /// </summary>
        public static string skillsToMonitor { get; set; }

        /// <summary>
        /// Total commands to run :{bcms='monitor bcms skill n', trunk='monitor bcms system'}
        /// </summary>
        public static string[] CommandsToRun { get; set; }

        /// <summary>
        /// Type of monitor commands to run 
        /// </summary>
        public static string CommandType { get; set; }

        /// <summary>
        /// total number of skills to be taken per connection
        /// </summary>
        public static int skillsPerConnection { get; set; }

        /// <summary>
        /// total skilllist to be monitored is stored in array
        /// </summary>
        public static List<string> skillList { get; set; }

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
        static readonly Dictionary<string, SkillExtensionInfo> _skillExtnInfo = new Dictionary<string, SkillExtensionInfo>();

        /// <summary>
        /// DB Connection string
        /// </summary>
        public static string ConntnString { get; set; }

        /// <summary>
        /// freq
        /// </summary>
        public static int HuntFrequency { get; set; }

        /// <summary>
        /// Range of hunt group to be monitored.
        /// </summary>
        public static List<int> HuntGroups { get; set; }

        public static List<int> Skills { get; set; }

        public static string TlsVersion { get; set; }
        public static List<int> totalSkillIds = new List<int>();
        #endregion

        static string userName = ConfigurationManager.AppSettings["userName"].ToString();
        static string password = ConfigurationManager.AppSettings["password"].ToString();

        /// <summary>
        /// Load all config data
        /// </summary>
        public static void LoadConfig()
        {
            log.Debug("LoadConfig()");
            try
            {
                string sectionSkills = "";  //variable contains all the combined skill id values
                ConfigurationManager.RefreshSection("appSettings");
                Channel();
                sectionSkills = SectionSkills();
                DecryptCredentials();
                ConntnString = ConnectionStrings.DecryptConnectionString(ConfigurationManager.AppSettings["CMDbConn"]);
                ServerAddress = ConfigurationManager.AppSettings["serverAddress"];
                //skillsToMonitor = ConfigurationManager.AppSettings["skillsToMonitor"];
                skillsToMonitor = sectionSkills;
                //skillList = skillsToMonitor.Split(',');
                DashboardRefreshTime = Convert.ToInt32(ConfigurationManager.AppSettings["DashboardRefreshTime"]);
                ReportRefreshTime = Convert.ToInt32(ConfigurationManager.AppSettings["HistoricalReportRefreshTime"]);
                ActionOnCMConFailure = ConfigurationManager.AppSettings["ActionOnCMConFailure"].ToLower();
                ConnectionType = ConfigurationManager.AppSettings["Type"];
                var val = ConfigurationManager.AppSettings["CommandsToRun"];
                CommandsToRun = val.Split(',');
                CommandType = ConfigurationManager.AppSettings["CommandType"];
                HuntFrequency = Convert.ToInt32(ConfigurationManager.AppSettings["HuntFrequency"]);
                TlsVersion = ConfigurationManager.AppSettings["TlsVersion"];
                skillList=FormatSkills(skillsToMonitor);
                try
                {
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                }
                catch (Exception)
                {
                    Port = 22;
                }
                try
                {
                    string[] HuntRange = ConfigurationManager.AppSettings["HuntRange"].Split(';');
                    HuntGroups = new List<int>();
                    for (int i = 0; i < HuntRange.Length; i++)
                    {
                        HuntGroups.AddRange(Enumerable.Range(Convert.ToInt32(HuntRange[i].Split('-')[0]),
                            (Convert.ToInt32(HuntRange[i].Split('-')[1]) - Convert.ToInt32(HuntRange[i].Split('-')[0])) + 1));
                    }

                    MaxTriesOnCMConFailure = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTriesOnConnectionFailure"]);
                    skillsPerConnection = Convert.ToInt32(ConfigurationManager.AppSettings["skillsPerConnection"]);
                    HAEnabled = Convert.ToInt32(ConfigurationManager.AppSettings["HAEnabled"]);


                    if (CommandType.ToLower() == "traffic")
                    {
                        var result = Connector.Proxy.SMSAPIProxy.GetSkills();
                        if (result != null)
                        {
                            log.Info("Total Skills Obtained from SMSAPI : " + result.Count);
                            Skills = result.Select(x => Convert.ToInt32(x.group_NumberField)).ToList();
                            log.Info("Total Skills from config info : " + HuntGroups.Count);
                            HuntGroups = HuntGroups?.Where(x => Skills.Contains(x)).ToList();
                            log.Info("Total Skills to monitor after applying filter : " + HuntGroups.Count);
                        }
                        else
                            log.Info("No data obtained from smsapi");
                    }
                }
                catch (Exception)
                {
                    skillsPerConnection = GetSkillsPerConnection();
                    HAEnabled = 0;
                    MaxTriesOnCMConFailure = 0;
                }
                FetchExtenSkillData();

            }
            catch (Exception ex)
            {
                log.Error("Error in LoadConfig() : " + ex);
            }
        }

        static int GetSkillsPerConnection()
        {
            log.Debug("GetSkillsPerConnection()");
            try
            {
                if (skillList.Count() > 2)
                    return skillList.Count() / 2;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetSkillsPerConnection() : " + ex);
            }
            return 1;
        }

        /// <summary>
        /// Gives total number of connections to be established to CM.
        /// </summary>
        public static int GetNumberOfConnectionsToEstablish()
        {
            log.Debug("GetNumberOfConnectionsToEstablish()");
            try
            {
                int totalConnections = 0;
                double count = 0;
                List<int> totalSkills = null;

                if (CommandType.ToLower() == "bcms")
                    totalSkills = skillList?.Select(x => Int32.TryParse(x, out int n) ? n : (int?)null).Where(n => n.HasValue)?.Select(n => n.Value).ToList();
                else if (CommandType.ToLower() == "traffic")
                {
                    totalSkills = HuntGroups;
                    log.Info("Total Skills to monitor : " + totalSkills.Count);
                    log.Info("HuntFrequency : " + HuntFrequency);
                    if (totalSkills.Count > (HuntFrequency * 32))
                        skillsPerConnection = HuntFrequency * 32;
                    else
                        return 1;
                }

                count = (double)totalSkills.Count / skillsPerConnection;
                string s = count.ToString("0.00");
                string[] sl = s.Split('.');
                if (Convert.ToInt32(sl[1]) > 0 && Convert.ToInt32(sl[0]) > 0) totalConnections = Convert.ToInt32(sl[0]) + 1;
                if (Convert.ToInt32(sl[1]) > 0 && Convert.ToInt32(sl[0]) < 0) totalConnections = 1;
                if (Convert.ToInt32(sl[1]) == 0 && Convert.ToInt32(sl[0]) > 0) totalConnections = Convert.ToInt32(sl[0]);

                log.Debug("Total Connections to make : " + totalConnections);
                return totalConnections;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetNumberOfConnectionsToEstablish() : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// Decrypts the username and password
        /// </summary>
        public static void DecryptCredentials()
        {
            log.Debug("DecryptCredentials()");
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
                log.Error("Error in DecryptCredentials() : " + ex);
            }
        }

        /// <summary>
        /// Map skills based on channel and stores as in-memory data.
        /// </summary>
        public static void Channel()
        {
            log.Debug("Channel");
            try
            {
                // clear the in-memory data.
                channelObj.Clear();
                ConfigurationManager.RefreshSection("TRealTimeDataServiceSettings");
                var section = (ConfigSection)ConfigurationManager.GetSection("TRealTimeDataServiceSettings");
                List<int> nums = new List<int>();
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
            }
            catch (Exception ex)
            {
                log.Error("Error in Channel() : " + ex);
            }
        }

        /// <summary>
        /// Gets the channel for given skill
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>return channel name for given skillId</returns>
        public static string GetChannel(string skillId)
        {
            log.Debug("GetChannel()" + skillId);
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

        /// <summary>
        /// skill to extension map
        /// </summary>
        static void FetchExtenSkillData()
        {
            try
            {
                if (channelObj != null)
                {
                    //var emailSkillIds = channelObj.FirstOrDefault(x => x.Key.ToLower() == "email").Value.ToList();
                    //StringBuilder builder = new StringBuilder();
                    string skillIds = string.Empty;
                    //for (int i = 0; i < channelObj.Count; i++)
                    //{
                    //    var skills = channelObj.ElementAtOrDefault(i).Value;
                    //    if (skills != null || skills.Count > 0)
                    //    {
                    //        skillIds += string.Join(",", skills);
                    //        skillIds = skillIds.EndsWith(",") ? skillIds : skillIds + ",";
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(skillIds))
                    //{

                    //    if (skillIds.StartsWith(","))
                    //        skillIds = skillIds.Remove(0, 1);

                    //    if (skillIds.EndsWith(","))
                    //        skillIds = skillIds.Substring(0, skillIds.Length - 1);
                    //}

                    if (ConnectionType.ToLower() == "cm" && !string.IsNullOrEmpty(skillIds))
                        FetchCMExtnSkillData(skillIds);
                    //else if (ConnectionType.ToLower() == "sip" && !string.IsNullOrEmpty(skillIds))
                    //    FetchSIPExtnSkillData(skillIds);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in FetchExtenSkillData: ", ex);
            }
        }

        static void FetchCMExtnSkillData(string skillIds)
        {
            log.Debug("FetchCMExtnSkillData()");
            try
            {

                //string[] values = skillIds.Split(',');
                var skilldata = Connector.Proxy.SMSAPIProxy.GetSkills();
                Connector.SMSAPI.HuntGroupType huntGroupData = null;
                if (skilldata != null && totalSkillIds != null)
                {
                    foreach (var skill in totalSkillIds)
                    {
                        huntGroupData = skilldata.FirstOrDefault(x => x.group_NumberField == skill.ToString());
                        if (huntGroupData != null)
                        {
                            _skillExtnInfo.Add(huntGroupData.group_ExtensionField, new SkillExtensionInfo
                            {
                                SkillId = Convert.ToInt32(huntGroupData.group_NumberField),
                                ExtensionId = Convert.ToInt32(huntGroupData.group_ExtensionField),
                                SkillName = huntGroupData.group_NameField,
                                Channel = GetChannel(huntGroupData.group_NumberField)
                            });
                        }
                        else
                            log.Info("Failed to add to SkillExtnInfo , No huntgroup data obtained from smsapi for skill : " + skill);
                    }
                    log.Debug("completed mapping of skill-extension");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in FetchCMExtnSkillData: ", ex);
            }
        }

        /*
        static void FetchSIPExtnSkillData(string skillIds)
        {
            try
            {
                log.Debug("FetchExtenSkillData()");

                string sql = @"select SkillID,SkillExtension,SkillName from TMAC_Skills with (nolock) Where SkillID in (" + skillIds + ")";

                log.Info("sql = " + sql);

                DataTable result = SqlDataAccess.ExecuteDataTable(sql, ConntnString);

                if (result != null)
                {
                    log.Debug("Skill to Extension mapping");
                    foreach (DataRow entry in result.Rows)
                    {
                        // add to dictionary object to maintain a skill-Extn mapping.
                        try
                        {
                            _skillExtnInfo.Add(entry.ItemArray[1].ToString(), new SIPDataCollector.Models.SkillExtensionInfo
                            {
                                SkillId = Convert.ToInt32(entry.ItemArray[0]),
                                ExtensionId = Convert.ToInt32(entry.ItemArray[1]),
                                SkillName = entry.ItemArray[2].ToString()
                            });
                        }
                        catch (Exception ex)
                        {
                            log.Warn("Warning! while adding values to dictionary");
                            log.Error("Message : ", ex);
                        }
                    }
                    log.Debug("completed mapping of skill-extension");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in FetchSIPExtnSkillData: ", ex);
            }
        }
        */
        public static string GetExtensionId(string skillId)
        {
            log.Debug("GetExtensionId()" + skillId);
            try
            {
                if (_skillExtnInfo != null)
                    return _skillExtnInfo.FirstOrDefault(x => x.Value.SkillId.ToString() == skillId).Key;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetExtensionId: " + ex);
            }
            return null;
        }

        private static List<string> FormatSkills(string skillList)
        {
            try
            {

                if (skillList != null && skillList.Count() >= 0 && !string.IsNullOrEmpty(skillList))
                {
                    string[] strArrays = skillList?.Split(new char[] { ';' });
                    if ((strArrays != null ? true : strArrays.Length != 0))
                    {
                        if (strArrays[0] != "")
                        {
                            List<int> nums = new List<int>();
                            for (int i = 0; i < (int)strArrays.Length-1; i++)
                            {
                                if (!strArrays[i].Contains("-")) //added to check if the skill is added like 100;101-110. this will modify it to 100-100;101-110 Nov 13,2019
                                {
                                    strArrays[i] = strArrays[i] + "-" + strArrays[i];
                                }

                                //log.Debug(strArrays[i].Split(new char[] { '-' })[0]);
                                nums.AddRange(Enumerable.Range(Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]),
                                    Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[1]) - Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]) + 1));
                            }
                            
                            List<HuntGroupType> result = SMSAPIProxy.GetSkills();

                            if (result != null)
                            {
                                nums =nums.Distinct().ToList(); //removing repeated skills numbers, added on Nov 13,2019
                                totalSkillIds = nums;
                                log.Info("Total Skills Obtained from SMSAPI : " + result.Count);
                                List<int> Skills = result.Select(x => Convert.ToInt32(x.group_NumberField)).ToList();
                                log.Info("Total Skills from config info : " + nums.Count);
                                nums = nums?.Where(x => Skills.Contains(x)).ToList();
                                log.Info("Total Skills to monitor after applying filter : " + nums.Count);
                                return nums.Select(x => Convert.ToString(x)).ToList();
                            }
                            else
                                log.Info("No data obtained from smsapi");

                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ConfigurationData.log.Error("Error : ", exception);
            }
            return null;
        }

        public static string SectionSkills()
        {
            var skillValue = channelObj.Select(y => y.Value).ToList();
            string sectionSkills = "";
            for (int i = 0; i < skillValue.Count; i++)
            {
                sectionSkills += skillValue[i][0] + ";";
            }
            return sectionSkills;
        }

        public static void RefreshSection(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                log.Info("Refreshing config");
                //Channel();
                //string sectionSkills = SectionSkills();
                //skillsToMonitor = sectionSkills;
                //skillList = FormatSkills(skillsToMonitor);
                //FetchExtenSkillData();
                LoadConfig();
            }
            catch (Exception e)
            {
                log.Error("Exeception in RefreshSection() " + e);
            }

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