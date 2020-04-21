using System;
using System.Collections.Generic;
using SIPDataCollector.Models;

namespace SIPDataCollector
{
    public interface ISipDataCollector
    {
        List<RealtimeData> GetBcmsData();

        RealtimeData GetBcmsDataForSkill(string skillId);

        void PullDataFromAlternateServer();

        List<SkillExtensionInfo> GetSkillAndExtensions();

        void VdnInformation(string callid, string queue, string queuetime, string abandontime);

        void VdnInformationForAbandon(string callid, string queue, string queuetime, string abandontime);
    }
}
