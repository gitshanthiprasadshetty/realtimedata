using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace SIPDataCollector.Models
{
    [DataContract]
    public class AgentData
    {
        /// <summary>
        /// Avaya Login-Id
        /// </summary>
        [DataMember(Name = "loginId")]
        public string LoginId { get; set; }

        /// <summary>
        /// Agent State-{ACW,Default,Available..}
        /// </summary>
        [DataMember(Name = "status")]
        public string State { get; set; }
        [DataMember(Name = "stationId")]
        public string StationID { get; set; }

    }
}