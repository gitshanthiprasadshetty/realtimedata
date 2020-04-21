using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Models
{
    public class BcmsSystem
    {
        public string Date { get; set; }
        public string SkillName { get; set; }
        public string CallsWaiting { get; set; }
        public string OldestCall { get; set; }
        public string AvgSpeedAnswer { get; set; }
        public string AvailableAgents { get; set; }
        public string AbandandCalls { get; set; }
        public string AvgAbandTime { get; set; }
        public string AcdCalls { get; set; }
        public string AvgTalkTime { get; set; }
        public string AvgAfterCall { get; set; }
        public string PercentageSL { get; set; }
    }
}
