using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRealtimeData.Model
{
    public class MySettingsModel
    {
        public string CMDbConn { get; set; }
        public string DashboardRefreshTime { get; set; }
        public string acceptableSL { get; set; }
        public string DBRefreshTime { get; set; }
        public string TmacServers { get; set; }
        public string ReloadConfigTime { get; set; }
        public string GetAgentSkillInfo { get; set; }
        public string isVDNMonitor { get; set; }
        public string ExtraAUXCodes { get; set; }
    }
}
