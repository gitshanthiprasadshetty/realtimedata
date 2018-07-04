using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{
    /// <summary>
    /// Bcms Dashboard Fields
    /// </summary>
    public class BcmsDataForSIP
    {
        private string skill;
        private string skillName;
        private string callsWaiting;

        private string staff;
        private string avail;
        private string acd;
        private string channel;
        private int acceptedSl;

        private List<AgentData> agentData;

        private string abandCalls;
        private string avgAbandTime;
        private string acdCallsSummary;

        private string acw;
        private string aux;
        private string extn;
        private string other;
        private string oldestCall;
        private string sl;
        private int avgHandlingTime;
        private string date;
        private decimal abnPercent;

        public string Skill
        {
            get { return skill; }
            set { skill = value; }
        }
               

        public string SkillName
        {
            get { return skillName; }
            set { skillName = value; }
        }


        public string CallsWaiting
        {
            get { return callsWaiting; }
            set { callsWaiting = value; }
        } 


        public string Staff
        {
            get { return staff; }
            set { staff = value; }
        }


        public string Avail
        {
            get { return avail; }
            set { avail = value; }
        }


        public string ACD
        {
            get { return acd; }
            set { acd = value; }
        }   

  
        public string AbandCalls
        {
            get { return abandCalls; }
            set { abandCalls = value; }
        }


        public string AvgAbandTime
        {
            get { return avgAbandTime; }
            set { avgAbandTime = value; }
        }


        public string TotalACDInteractions
        {
            get { return acdCallsSummary; }
            set { acdCallsSummary = value; }
        }


        public List<AgentData> AgentData
        {
            get { return agentData; }
            set { agentData = value; }
        }

        public string ACW
        {
            get { return acw; }
            set { acw = value; }
        }

        public string AUX
        {
            get { return aux; }
            set { aux = value; }
        }

        public string Extn
        {
            get { return extn; }
            set { extn = value; }
        }

        public string Other
        {
            get { return other; }
            set { other = value; }
        }

        public string OldestCall
        {
            get { return oldestCall; }
            set { oldestCall = value; }
        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public string SLPercentage
        {
            get { return sl; }
            set { sl = value; }
        }

        public int AvgHandlingTime
        {
            get { return avgHandlingTime; }
            set { avgHandlingTime = value; }
        }

        public int AccptedSL
        {
            get { return acceptedSl; }
            set { acceptedSl = value; }
        }

        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public decimal AbandonPercentage
        {
            get { return abnPercent; }
            set { abnPercent = value; }
        }
    }
}
