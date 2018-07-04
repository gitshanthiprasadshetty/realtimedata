using System.Runtime.Serialization;

namespace SIPDataCollector.Models
{
    /// <summary>
    /// Holds agent related data
    /// </summary>
    public class AgentData
    {
        /// <summary>
        /// Avaya Login-Id
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Agent State-{ACW,Default,Available..}
        /// </summary>
        public string State { get; set; }

        public int TotalStaffedAgents { get; set; }

    }
}
