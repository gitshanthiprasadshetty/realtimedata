using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.ServiceModel.Administration;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using SIPDataCollector.Models;

namespace SIPDataCollector
{
    [ServiceContract]
    public interface ISipDataCollector
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

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/VdnInformation/{callid}/{queue}/{queuetime}/{abandontime}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void VdnInformation(string callid, string queue, string queuetime, string abandontime);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/VdnInformationForAbandon/{callid}/{queue}/{queuetime}/{abandontime}", RequestFormat =WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void VdnInformationForAbandon(string callid, string queue, string queuetime, string abandontime);
    }
}
