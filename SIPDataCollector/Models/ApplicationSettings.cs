using System;
using System.Collections.Generic;
using System.Text;

namespace SIPDataCollector.Models
{
   public class ApplicationSettings
    {
        public static string CMDbConn { get; set; }
        public static string DashboardRefreshTime { get; set; }
        public static string acceptableSL { get; set; }
        public static string DBRefreshTime { get; set; }
        public static string TmacServers { get; set; }
        public static string ReloadConfigTime { get; set; }
        public static string GetAgentSkillInfo { get; set; }
        public static string isVDNMonitor { get; set; }
        public static string ExtraAUXCodes { get; set; }
        public static string channelName { get; set; }
    }
}
