using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{
    internal class SkillData
    {
        internal int skillId { get; set; }
        internal int ACDTime { get; set; }
        internal int ACWTime { get; set; }
        internal int AHTTime { get; set; }
        internal int AbandCalls { get; set; }
        internal decimal SLPercentage { get; set; }
        internal int TotalACDInteractions { get; set; }
        internal int TotalCallsHandled { get; set; }
        internal int AvgHandlingTime { get; set; }
        internal int AvgAbandTime { get; set; }
        internal decimal AbandonPercentage { get; set; }
        public int Backlog { get; set; }
        public int TotalNoFirstResponse { get; set; }
        public int TotalMetFirstResponse { get; set; }
        public int TotalNotMetFirstResponse { get; set; }
        public int AverageFirstResponse { get; set; }
    }
}
