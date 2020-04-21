using System.Collections.Generic;
using CMDataCollector.Models;

namespace CMDataCollector
{
    public interface IRealtimeDataService
    {
       //"/GetRealtimeDataForAllSkills"
        List<RealtimeData> MonitorBcms();

        //"/GetRealtimeDataForSkill/{skillToMonitor}"
        RealtimeData MonitorBcmsForSkill(string skillToMonitor);

        //"/MonitorTrunkTraffic"
        List<TrunkGroupTraffic> MonitorTrunkTraffic();

        //"/MonitorAllHuntTraffics"
        List<HuntGroupTraffic> MonitorAllHuntTraffics();

        //"/MonitorHuntTraffic/{huntGroupNumber}"
        HuntGroupTraffic MonitorHuntTraffic(string huntGroupNumber);

        //"/MonitorBcmsSystem/{skillName}"
        List<BcmsSystem> MonitorBcmsSystem(string skillName);

        //"/IsRunning"
        bool IsRunning();

        //"/IsConnectedToCM"
        bool IsConnectedToCM();
    }
}
