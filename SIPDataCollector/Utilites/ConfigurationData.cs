using ConfigurationProvider;
using Connector.Proxy;
using Connector.SMSAPI;
using SIPDataCollector.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;

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
        
        /// <summary>
        /// 
        /// </summary>
        static readonly Dictionary<string, List<string>> channelObj = new Dictionary<string, List<string>>();

        internal static List<string> auxCodes;

        internal static List<string> TmacServers;

        internal static  Dictionary<string, int> acceptableSlObj = new Dictionary<string, int>();

        internal static int acceptableSL;

        public static List<int> totalSkillIds = new List<int>();
        public static int ReloadConfigTime { get; set; }
        /// <summary>
        /// Dashboard refresh time
        /// </summary>
        public static int DBRefreshTime { get; set; }
        /// <summary>
        /// Method to load all data from config.
        /// </summary>
        public static void LoadConfig()
        {
            log.Debug("BcmsSIPManager.ConfigurationData[LoadConfig]");
            try
            {
                string sectionSkills = "";  //variable contains all the combined skill id values
                ConfigurationManager.RefreshSection("appSettings");
                Channel();
                sectionSkills = SectionSkills();
                ////ConntnString = ConfigurationSettings.AppSettings["CMDbConn"].ToString();
                ConntnString = ConnectionStrings.DecryptConnectionString(ConfigurationManager.AppSettings["CMDbConn"]);
                //skillsToMonitor = ConfigurationSettings.AppSettings["skillsToMonitorForSIP"];
                skillsToMonitor = sectionSkills;
                DashboardRefreshTime = Convert.ToInt32(ConfigurationManager.AppSettings["DashboardRefreshTime"]);
                acceptableSL = Convert.ToInt32(ConfigurationManager.AppSettings["acceptableSL"]);
                DBRefreshTime = Convert.ToInt32(ConfigurationManager.AppSettings["DBRefreshTime"]);
                TmacServers = ConfigurationManager.AppSettings["TmacServers"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ReloadConfigTime = Convert.ToInt32(ConfigurationManager.AppSettings["ReloadConfigTime"]);
                //if (!string.IsNullOrEmpty(skillsToMonitor) || skillsToMonitor.ToLower() != "na")
                //    skillList = skillsToMonitor.Split(',');

                skillList = FormatSkills(skillsToMonitor);

                //modified by Anish In SIP we don't have to check for SMSAPI skills
                skillList = skillsToMonitor.Split(',').ToList();

                log.Info($"Monitoring: {String.Join(", ", skillList)}");
                auxCodes = DataAccess.GetAuxCodes();
                auxCodes.Add("Default");
                acceptableSlObj = DataAccess.GetAcceptableLevels();
            }
            catch (Exception ex)
            {
                log.Error("Error in BcmsSIPManager.ConfigurationData[LoadConfig] : " + ex);
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
            
            }
            catch (Exception ex)
            {
                log.Error("Error in Channel() : " + ex);
            }
        }

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

        private static List<string> FormatSkills(string skillList)
        {
            try
            {
                log.Info("Skills List: " + skillList);
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

                            //List<int> nums = new List<int>();
                            //for (int i = 0; i < (int)strArrays.Length-1; i++)
                            //{
                            //    if (!strArrays[i].Contains("-")) //added to check if the skill is added like 100;101-110. this will modify it to 100-100;101-110 Nov 13,2019
                            //    {
                            //        strArrays[i] = strArrays[i] + "-" + strArrays[i];
                            //    }

                            //    //log.Debug(strArrays[i].Split(new char[] { '-' })[0]);
                            //    nums.AddRange(Enumerable.Range(Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]),
                            //        Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[1]) - Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]) + 1));
                            //}
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

        //static void FetchSIPExtnSkillData(string skillIds)
        //{
        //    try
        //    {
        //        log.Debug("FetchExtenSkillData()");

        //        string sql = @"select SkillID,SkillExtension,SkillName from TMAC_Skills with (nolock) Where SkillID in (" + skillIds + ")";

        //        log.Info("sql = " + sql);

        //        DataTable result = SqlDataAccess.ExecuteDataTable(sql, ConntnString);

        //        if (result != null)
        //        {
        //            log.Debug("Skill to Extension mapping");
        //            foreach (DataRow entry in result.Rows)
        //            {
        //                // add to dictionary object to maintain a skill-Extn mapping.
        //                try
        //                {
        //                    _skillExtnInfo.Add(entry.ItemArray[1].ToString(), new SIPDataCollector.Models.SkillExtensionInfo
        //                    {
        //                        SkillId = Convert.ToInt32(entry.ItemArray[0]),
        //                        ExtensionId = Convert.ToInt32(entry.ItemArray[1]),
        //                        SkillName = entry.ItemArray[2].ToString()
        //                    });
        //                }
        //                catch (Exception ex)
        //                {
        //                    log.Warn("Warning! while adding values to dictionary");
        //                    log.Error("Message : ", ex);
        //                }
        //            }
        //            log.Debug("completed mapping of skill-extension");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error in FetchSIPExtnSkillData: ", ex);
        //    }
        //}

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
                SIPManager.GetInstance().MapSkillExtnData();
            }
            catch (Exception e)
            {
                log.Error("Exeception in RefreshSection() " + e);
            }

        }

        public static string SectionSkills()
        {
            try
            {
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
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
