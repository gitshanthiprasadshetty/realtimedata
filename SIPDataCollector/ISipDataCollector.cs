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
        [WebInvoke(Method = "GET", UriTemplate = "/GetBcmsDataForSkill/{skillId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        RealtimeData GetBcmsDataForSkill(string skillId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/PullDataFromAlternateServer", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void PullDataFromAlternateServer();
    }
}
