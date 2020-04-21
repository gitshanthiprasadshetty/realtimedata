using CMDataCollector;
using CMDataCollector.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TRealtimeData.RestMethods
{
    [ApiController]
    [Route("api/TRealtimeDataRest")]
    public class CMRestServiceController: Controller
    {
        private static Logger.Logger log = new Logger.Logger(typeof(CMRestServiceController));
        #region CM
        [HttpGet]
        [Route("[action]")]
        public List<CMDataCollector.Models.RealtimeData> MonitorBcms()
        {
            log.Debug("Calling MonitorBcms()");
            return CMDataService.GetInstance().MonitorBcms();
        }

        [HttpGet]
        [Route("[action]/{skillToMonitor}")]
        public CMDataCollector.Models.RealtimeData MonitorBcmsForSkill(string skillToMonitor)
        {
            log.Debug("Calling MonitorBcmsForSkill()");
            return CMDataService.GetInstance().MonitorBcmsForSkill(skillToMonitor);
        }

        [HttpGet]
        [Route("[action]")]
        public List<TrunkGroupTraffic> MonitorTrunkTraffic()
        {
            log.Debug("Calling MonitorTrunkTraffic()");
            return CMDataService.GetInstance().MonitorTrunkTraffic();
        }

        [HttpGet]
        [Route("[action]")]
        public List<HuntGroupTraffic> MonitorAllHuntTraffics()
        {
            log.Debug("Calling MonitorAllHuntTraffics()");
            return CMDataService.GetInstance().MonitorAllHuntTraffics();
        }

        [HttpGet]
        [Route("[action]/{huntGroupNumber}")]
        public HuntGroupTraffic MonitorHuntTraffic(string huntGroupNumber)
        {
            log.Debug("Calling MonitorHuntTraffic()");
            return CMDataService.GetInstance().MonitorHuntTraffic(huntGroupNumber);
        }

        [HttpGet]
        [Route("[action]/skillName")]
        public List<BcmsSystem> MonitorBcmsSystem(string skillName)
        {
            log.Debug("Calling MonitorBcmsSystem()");
            return CMDataService.GetInstance().MonitorBcmsSystem(skillName);
        }

        [HttpGet]
        [Route("[action]")]
        public bool IsRunning()
        {
            log.Debug("Calling IsRunning()");
            return CMDataService.GetInstance().IsRunning();
        }

        [HttpGet]
        [Route("[action]")]
        bool IsConnectedToCM()
        {
            log.Debug("Calling IsConnectedToCM()");
            return CMDataService.GetInstance().IsConnectedToCM();
        }
        #endregion


    }
}
