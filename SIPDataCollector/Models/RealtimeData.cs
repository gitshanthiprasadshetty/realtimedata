using System.Collections.Generic;

public class RealtimeData
{
    public int SkillId { get; set; }
    public int SkillExtensionId { get; set; }

    public string SkillName { get; set; }

    public int CallsWaiting { get; set; }

    public int TotalAgentsStaffed { get; set; }

    public int TotalAvailableAgents { get; set; }

    public int ActiveCalls { get; set; }

    public int AbandonedCalls { get; set; }

    public int AverageAbandonedTime { get; set; }

    public int TotalActiveInteractions { get; set; }

    public List<AgentData> AgentData { get; set; }

    public int TotalAgentsInACW { get; set; }

    public int TotalAgentsInAUX { get; set; }

    public int OldestCallWaitTime { get; set; }

    public decimal SLPercentage { get; set; }

    public int AverageHandlingTime { get; set; }

    public int AcceptedSL { get; set; }
    public string Channel { get; set; }

    public decimal AbandonPercentage { get; set; }

}
