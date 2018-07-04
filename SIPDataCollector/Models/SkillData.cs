using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{
    internal class SkillData
    {
        internal string skillID { get; set; }
        internal int ACDTime { get; set; }
        internal int ACWTime { get; set; }
        internal int AbandCalls { get; set; }
        internal decimal SLPercentage { get; set; }
        internal int TotalACDInteractions { get; set; }
        internal int AvgHandlingTime { get; set; }
        internal decimal AbandonPercentage { get; set; }
    }
}
