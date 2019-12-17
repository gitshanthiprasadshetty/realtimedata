using Newtonsoft.Json;
using SIPDataCollector.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class RealtimeData
{
    [DataMember(Name ="skillId")]
    public int SkillId { get; set; }
    [DataMember(Name = "skillExtensionId")]
    public int SkillExtensionId { get; set; }
    [DataMember(Name = "skillName")]
    public string SkillName { get; set; }
    [DataMember(Name = "interactionsInQueue")]
    public int InteractionsInQueue { get; set; }
    [DataMember(Name = "activeInteractions")]
    public int ActiveInteractions { get; set; }
    [DataMember(Name = "activeInteractionsSummary")]
    public int ActiveInteractionsSummary { get; set; }
    [DataMember(Name = "abandonInteractionsSummary")]
    public int AbandonedInteractionsSummary { get; set; }
    [DataMember(Name = "oldestInteractionWaitTime")]
    public int OldestInteractionWaitTime { get; set; }
    [DataMember(Name = "averageAbandonTime")]
    public int AverageAbandonedTime { get; set; }
    [DataMember(Name = "abandonPercentage")]
    public decimal AbandonPercentage { get; set; }
    [DataMember(Name = "slPercentage")]
    public decimal SLPercentage { get; set; }
    [DataMember(Name = "aht")]
    public int AverageHandlingTime { get; set; }
    [DataMember(Name = "totalAgentStaffed")]
    public int TotalAgentsStaffed { get; set; }
    [DataMember(Name = "totalAgentAvailable")]
    public int TotalAgentsAvailable { get; set; }
    [DataMember(Name = "totalAgentACW")]
    public int TotalAgentsInACW { get; set; }
    [DataMember(Name = "totalAgentAux")]
    public int TotalAgentsInAUX { get; set; }
    [DataMember(Name = "agentList")]
    public List<AgentData> AgentStats { get; set; }
    [DataMember(Name = "acceptedSL")]
    public int AcceptedSL { get; set; }
    [DataMember(Name = "channel")]
    public string Channel { get; set; }
    [DataMember(Name = "AvgSpeedAnswer")]
    public int AvgSpeedAnswer { get; set; }
    [DataMember(Name = "activeTime")]
    public int InteractionsActiveTime { get; set; }
    [DataMember(Name = "holdTime")]
    public int InteractionsHoldTime { get; set; }
    [DataMember(Name = "InteractionsQueueTime")]
    public int InteractionsQueueTime { get; set; }
    [DataMember(Name = "TotalInteractionsQueueTime")]
    public int TotalInteractionsQueueTime { get; set; }
    [DataMember(Name = "acwTime")]
    public int InteractionsAcwTime { get; set; }
    [DataMember(Name = "callsAnsweredWithinSL")]
    public int CallsAnsweredWithinSLA { get; set; }
}

//public class RealtimeData
//{
//    public int SkillId { get; set; }
//    public int SkillExtensionId { get; set; }

//    public string SkillName { get; set; }

//    public int CallsWaiting { get; set; }

//    public int TotalAgentsStaffed { get; set; }

//    public int TotalAvailableAgents { get; set; }

//    public int ActiveCalls { get; set; }

//    public int AbandonedCalls { get; set; }

//    public string AverageAbandonedTime { get; set; }

//    public int TotalActiveInteractions { get; set; }

//    public List<AgentData> AgentData { get; set; }

//    public int TotalAgentsInACW { get; set; }

//    public int TotalAgentsInAUX { get; set; }

//    public string OldestCallWaitTime { get; set; }

//    public decimal SLPercentage { get; set; }

//    public string AverageHandlingTime { get; set; }

//    public int AcceptedSL { get; set; }
//    public string Channel { get; set; }

//    public decimal AbandonPercentage { get; set; }
//}
