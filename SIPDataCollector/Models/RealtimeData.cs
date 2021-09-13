using SIPDataCollector.Models;
using System.Collections.Generic;

public class RealtimeData
{
    public int SkillId { get; set; }
    public int SkillExtensionId { get; set; }
    public string SkillName { get; set; }
    public int InteractionsInQueue { get; set; }
    public int ActiveInteractions { get; set; }
    public int ActiveInteractionsSummary { get; set; }
    public int AbandonedInteractionsSummary { get; set; }
    public int OldestInteractionWaitTime { get; set; }
    public int AverageAbandonedTime { get; set; }
    public decimal AbandonPercentage { get; set; }
    public decimal SLPercentage { get; set; }
    public int AverageHandlingTime { get; set; }
    public int TotalAgentsStaffed { get; set; }
    public int TotalAgentsAvailable { get; set; }
    public int TotalAgentsInACW { get; set; }
    public int TotalAgentsInAUX { get; set; }
    public List<AgentData> AgentStats { get; set; }
    public int AcceptedSL { get; set; }
    public string Channel { get; set; }
    public int Backlog { get; set; }
    public int TotalNoFirstResponse { get; set; }
    public int TotalMetFirstResponse { get; set; }
    public int TotalNotMetFirstResponse { get; set; }
    public int AverageFirstResponse { get; set; }
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
