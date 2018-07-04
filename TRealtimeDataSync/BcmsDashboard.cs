using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BcmsDataSync
{
    public class BcmsDashboard
    {
        private string skill;
        private string date;
        private string skillName;
        private string callsWaiting;
        private string acceptedSl;
        private string oldestCall;
        private string sl;

        private string staff;
        private string avail;
        private string acd;
        private string acw;
        private string aux;
        private string extn;
        private string other;
        private List<AgentData> agentData;

        private string abandCalls;
        private string avgAbandTime;
        private string acdCallsSummary;

        [DataMember]
        public string Skill
        {
            get { return skill; }
            set { skill = value; }
        }

        [DataMember]
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        [DataMember]
        public string SkillName
        {
            get { return skillName; }
            set { skillName = value; }
        }

        [DataMember]
        public string CallsWaiting
        {
            get { return callsWaiting; }
            set { callsWaiting = value; }
        }

        [DataMember]
        public string AccptedSL
        {
            get { return acceptedSl; }
            set { acceptedSl = value; }
        }

        [DataMember]
        public string OldestCall
        {
            get { return oldestCall; }
            set { oldestCall = value; }
        }

        [DataMember]
        public string SL
        {
            get { return sl; }
            set { sl = value; }
        }

        [DataMember]
        public string Staff
        {
            get { return staff; }
            set { staff = value; }
        }

        [DataMember]
        public string Avail
        {
            get { return avail; }
            set { avail = value; }
        }

        [DataMember]
        public string ACD
        {
            get { return acd; }
            set { acd = value; }
        }

        [DataMember]
        public string ACW
        {
            get { return acw; }
            set { acw = value; }
        }

        [DataMember]
        public string AUX
        {
            get { return aux; }
            set { aux = value; }
        }

        [DataMember]
        public string Extn
        {
            get { return extn; }
            set { extn = value; }
        }

        [DataMember]
        public string Other
        {
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
    }

    [DataContract]
    public class AgentData
    {
        //public string AgentName { get; set; }
        [DataMember]
        public string LoginId { get; set; }
        //public string Station { get; set; }
        [DataMember]
        public string State { get; set; }
        //public string Time { get; set; }
        //public string AcdCalls { get; set; }
        //public string ExtnInCalls { get; set; }
        //public string ExtnOutCalls { get; set; }
    }
}
