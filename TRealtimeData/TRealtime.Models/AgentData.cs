using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRealtime.Models
{
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

        public string StationID { get; set; }

    }
}
