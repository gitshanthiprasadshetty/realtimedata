using ConfigurationProvider;
using Connector.Proxy;
using Connector.SMSAPI;
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
                Channel();
                var skillValue = channelObj.Select(y => y.Value).ToList();

                for (int i = 0; i < skillValue.Count; i++)
                {
                    sectionSkills += skillValue[i][0] + ";";
                }
                ////ConntnString = ConfigurationSettings.AppSettings["CMDbConn"].ToString();
                ConntnString = ConnectionStrings.DecryptConnectionString(ConfigurationSettings.AppSettings["CMDbConn"]);
                //skillsToMonitor = ConfigurationSettings.AppSettings["skillsToMonitorForSIP"];
                skillsToMonitor = sectionSkills;
                DashboardRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DashboardRefreshTime"]);
                acceptableSL = Convert.ToInt32(ConfigurationSettings.AppSettings["acceptableSL"]);
                DBRefreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DBRefreshTime"]);
                TmacServers = ConfigurationSettings.AppSettings["TmacServers"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //if (!string.IsNullOrEmpty(skillsToMonitor) || skillsToMonitor.ToLower() != "na")
                //    skillList = skillsToMonitor.Split(',');

                skillList = FormatSkills(skillsToMonitor);

                auxCodes = DataAccess.GetAuxCodes();
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
                var section = (SIPDataCollector.Utilites.BcmsSIPConfigSection)ConfigurationManager.GetSection("TRealTimeDataServiceSettings");
                foreach (BCMSInstanceData data in section.BCMSServiceItems)
                {
                    if (data.SkillId.Contains(","))
                    {
                        log.Debug("Add skills for channel : " + data.ChannelName);
                        channelObj.Add(data.ChannelName, data.SkillId.Split(',').ToList());
                    }
                    else
                    {
                        List<string> result = FormatSkills(data.SkillId);
                        channelObj.Add(data.ChannelName, result);
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

                if (skillList != null && skillList.Count() >= 0 && !string.IsNullOrEmpty(skillList))
                {
                    string[] strArrays = skillList?.Split(new char[] { ';' });
                    if ((strArrays != null ? true : strArrays.Length != 0))
                    {
                        if (strArrays[0] != "")
                        {
                            List<int> nums = new List<int>();
                            for (int i = 0; i < (int)strArrays.Length; i++)
                            {
                                nums.AddRange(Enumerable.Range(Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]),
                                    Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[1]) - Convert.ToInt32(strArrays[i].Split(new char[] { '-' })[0]) + 1));
                            }
                            List<HuntGroupType> result = SMSAPIProxy.GetSkills();

                            if (result != null)
                            {
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
