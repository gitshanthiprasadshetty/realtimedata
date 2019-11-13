using System.Runtime.Serialization;

namespace CMDataCollector.Models
{
    [DataContract]
    public class AgentData
    {
        /// <summary>
        /// Agent's avaya Login-Id
        /// </summary>
        [DataMember]
        public string LoginId { get; set; }

        /// <summary>
        /// Status/state of Logged-in agent
        /// </summary>
        [DataMember]
        public string State { get; set; }
        public string StationID { get; set; }
    }
}
