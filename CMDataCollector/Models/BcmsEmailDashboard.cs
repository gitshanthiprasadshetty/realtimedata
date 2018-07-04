using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Models
{
    public class BcmsEmailDashboard
    {
        public string Skill { get; set; }
        public string SkillName { get; set; }
        public string AgentsStaffed { get; set; }
        public string AgentsAvailable { get; set; }
        public string TotalEmailsInQueue { get; set; }
        public string TotalEmailsAssigned { get; set; }
        public string ActiveTime { get; set; }
        public string HoldTime { get; set; }        
        public string QueueWaitTime { get; set; }
    }
}
