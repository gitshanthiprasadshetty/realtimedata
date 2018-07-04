using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{
    /// <summary>
    /// Holds Agent-{skill} information
    /// </summary>
    public class AgentSkillInfo
    {
        /// <summary>
        /// Avaya Login-id
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// Total List of Skills assigned to Agent.
        /// </summary>
        public List<string> Skills { get; set; }
    }
}
