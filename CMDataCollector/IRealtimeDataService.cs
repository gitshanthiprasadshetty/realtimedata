using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using CMDataCollector.Models;
using System.Runtime.Serialization;

namespace CMDataCollector
{
    [ServiceContract]
    public interface IRealtimeDataService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetRealtimeDataForAllSkills", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<RealtimeData> MonitorBcms();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetRealtimeDataForSkill/{skillToMonitor}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        RealtimeData MonitorBcmsForSkill(string skillToMonitor);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/MonitorTrunkTraffic", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<TrunkGroupTraffic> MonitorTrunkTraffic();

        [OperationContract]
        [WebInvoke(Method ="GET", UriTemplate = "/MonitorAllHuntTraffics", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<HuntGroupTraffic> MonitorAllHuntTraffics();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/MonitorHuntTraffic/{huntGroupNumber}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        HuntGroupTraffic MonitorHuntTraffic(string huntGroupNumber);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/MonitorBcmsSystem/{skillName}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<BcmsSystem> MonitorBcmsSystem(string skillName);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/IsRunning", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        bool IsRunning();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/IsConnectedToCM", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        bool IsConnectedToCM();
    }
}
