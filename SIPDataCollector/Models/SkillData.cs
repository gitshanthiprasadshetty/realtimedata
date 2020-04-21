using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{
    internal class SkillData
    {
        internal string Date { get; set; }
        internal int skillId { get; set; }
        internal string SkillName { get; set; }
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
        internal int AvgSpeedAnswer { get; set; }
        internal int AvgTalkTime { get; set; }
        internal int HoldTime { get; set; }
        internal int TotalAuxTime { get; set; }
        internal int FlowIn { get; set; }
        internal int FlowOut { get; set; }
        internal int AvgStaffedTime { get; set; }
        internal int TotalStaffedTime { get; set; }
        internal int CallsHandledWithinThreshold { get; set; }
        internal int CallsAbandAfterThreshold { get; set; }
        internal int PassedCalls { get; set; }
        internal int TransferCalls { get; set; }
        internal int TotalAbandTime { get; set; }
        internal int SpeedOfAnswer { get; set; }
        internal int TotalTalkTime { get; set; }
        internal int TotalStaffedAgents { get; set; }

    }
}
