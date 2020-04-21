using Microsoft.AspNetCore.Mvc;
using SIPDataCollector;
using SIPDataCollector.Models;
using System.Collections.Generic;

namespace TRealtimeData.RestMethods
{
    [Route("api/TRealtimeDataRest")]
    [ApiController]
    public class SIPRestServiceController: Controller
    {
        private static Logger.Logger log = new Logger.Logger(typeof(SIPRestServiceController));

        #region SIP
        [HttpGet]
        [Route("[action]")]
        public List<RealtimeData> GetBcmsData()
        {
            log.Debug("Calling Getbcmsdata()");
            return SIPManager.GetInstance().GetBcmsData();
        }
        [HttpGet]
        [Route("[action]/{skillid}")]
        public RealtimeData GetBcmsDataForSkill(string skillId)
        {
            log.Debug("Calling GetBcmsDataForSkill()");
            return SIPManager.GetInstance().GetBcmsDataForSkill(skillId);
        }
        [HttpGet]
        [Route("[action]")]
        public void PullDataFromAlternateServer()
        {
            log.Debug("Calling PullDataFromAlternateServer()");
            SIPManager.GetInstance().PullDataFromAlternateServer();
        }
        [HttpGet]
        [Route("[action]")]
        public List<SkillExtensionInfo> GetSkillAndExtensions()
        {
            log.Debug("Calling GetSkillAndExtensions()");
            return SIPManager.GetInstance().GetSkillAndExtensions();
        }
        [HttpGet]
        [Route("[action]/{callid}/{queue}/{queuetime}/{abandontime}")]
        public void VdnInformation(string callid, string queue, string queuetime, string abandontime)
        {
            log.Debug("Calling VdnInformation()");
            SIPManager.GetInstance().VdnInformation(callid,queue,queuetime,abandontime);
        }
        [HttpGet]
        [Route("[action]/{callid}/{queue}/{queuetime}/{abandontime}")]
        public void VdnInformationForAbandon(string callid, string queue, string queuetime, string abandontime)
        {
            log.Debug("Calling VdnInformationForAbandon()");
            SIPManager.GetInstance().VdnInformationForAbandon(callid, queue, queuetime, abandontime);
        }
        #endregion

    }
}
