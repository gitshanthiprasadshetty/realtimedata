using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TRealtimeData
{
    [ServiceContract]
    public interface IRealtimeData
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetRealtimeDataForAllSkills", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<RealtimeData> GetBcmsData();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetRealtimeDataForSkill/{skillId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        RealtimeData GetBcmsDataForSkill(string skillId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/PullDataFromAlternateServer", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void PullDataFromAlternateServer();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetSkillAndExtensions", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<SkillExtensionInfo> GetSkillAndExtensions();
    }
}
