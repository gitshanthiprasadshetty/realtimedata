using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMDataCollector.Models
{
    [DataContract]
    public class BcmsDashboard
    {
        /// <summary>
        /// SkillId
        /// </summary>
        private string skill;

        /// <summary>
        /// Date
        /// </summary>
        private string date;

        /// <summary>
        /// Skill Name
        /// </summary>
        private string skillName;

        /// <summary>
        /// Total calls waiting or in queue under this skill.
        /// </summary>
        private string callsWaiting;

        /// <summary>
        /// 
        /// </summary>
        private string acceptedSl;

        /// <summary>
        /// Oldest call in waiting list/Queue for this skill
        /// </summary>
        private string oldestCall;

        /// <summary>
        /// Service Level percentage.
        /// </summary>
        private string sl;

        /// <summary>
        /// Service Level percentage.
        /// </summary>
        private string perInSl;

        /// <summary>
        /// Total num of currently staffedin agents under this skill
        /// </summary>
        private string staff;

        /// <summary>
        /// Total number of currently available agents for this skill
        /// </summary>
        private string avail;

        /// <summary>
        /// Total number of active calls going-on currently for this skill
        /// </summary>
        private string acd;

        /// <summary>
        /// 
        /// </summary>
        private string acw;

        /// <summary>
        /// 
        /// </summary>
        private string aux;

        /// <summary>
        /// 
        /// </summary>
        private string extn;

        /// <summary>
        /// 
        /// </summary>
        private string other;

        /// <summary>
        /// List of all agents currently logged-in who has this skill
        /// </summary>
        private List<AgentData> agentData;

        /// <summary>
        /// Total abandand calls for this skill.
        /// </summary>
        private string abandCalls;

        /// <summary>
        /// Total abandand calls for this skill.
        /// </summary>
        private string abandCallsSummary;

        /// <summary>
        /// Avg of all Abandand calls.
        /// </summary>
        private string avgAbandTime;

        /// <summary>
        /// Summary of all ACD calls of last 24hrs.
        /// </summary>
        private string acdCallsSummary;

        private decimal abnPercent;
        private string avgHandlingTime;

        /// <summary>
        /// 
        /// </summary>
        private string channel;

        [DataMember]
        public string Skill
        {
            get { return skill; }
            set { skill = value; }
        }

        [DataMember]
        public string Date {
            get { return date; }
            set { date = value; }
        }

        [DataMember]
        public string SkillName {
            get { return skillName; }
            set { skillName = value; }
        }

        [DataMember]
        public string CallsWaiting {
            get { return callsWaiting; }
            set { callsWaiting = value; }
        }

        [DataMember]
        public string AccptedSL {
            get { return acceptedSl; }
            set { acceptedSl = value; }
        }

        [DataMember]
        public string OldestCall {
            get { return oldestCall; }
            set { oldestCall = value; }
        }

        [DataMember]
        public string SL {
            get { return sl; }
            set { sl = value; }
        }

        [DataMember]
        public string PercentageInSL
        {
            get { return perInSl; }
            set { perInSl = value; }
        }

        [DataMember]
        public string Staff {
            get { return staff; }
            set { staff = value; }
        }

        [DataMember]
        public string Avail {
            get { return avail; }
            set { avail = value; }
        }

        [DataMember]
        public string ACD {
            get { return acd; }
            set { acd = value; }
        }

        [DataMember]
        public string ACW {
            get { return acw; }
            set { acw = value; }
        }

        [DataMember]
        public string AUX {
            get { return aux; }
            set { aux = value; }
        }

        [DataMember]
        public string Extn {
            get { return extn; }
            set { extn = value; }
        }

        [DataMember]
        public string Other {
            get { return other; }
            set { other = value; }
        }

        [DataMember]
        public string AbandCalls
        {
            get { return abandCalls; }
            set { abandCalls = value; }
        }

        [DataMember]
        public string AbandCallsSummary
        {
            get { return abandCallsSummary; }
            set { abandCallsSummary = value; }
        }

        [DataMember]
        public string AvgAbandTime
        {
            get { return avgAbandTime; }
            set { avgAbandTime = value; }
        }

        [DataMember]
        public string AcdCallsSummary
        {
            get { return acdCallsSummary; }
            set { acdCallsSummary = value; }
        }

        [DataMember]
        public List<AgentData> AgentData
        {
            get { return agentData; }
            set { agentData = value; }
        }

        [DataMember]
        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        [DataMember]
        public decimal AbandonPercentage
        {
            get { return abnPercent; }
            set { abnPercent = value; }
        }

        [DataMember]
        public string AvgHandlingTime
        {
            get { return avgHandlingTime; }
            set { avgHandlingTime = value; }
        }
    }
}
