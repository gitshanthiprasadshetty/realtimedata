using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{

    public class AsyncChat
    {
        public string caseId { get; set; }
        public string sessionId { get; set; }
        public string agentId { get; set; }
        public DateTime? assignedDateTime { get; set; }
        public string queueId { get; set; }
        public string queueName { get; set; }
        public DateTime? queuedDateTime { get; set; }
        public string customerId { get; set; }
        public int status { get; set; }
        public string statusName { get; set; }
        public DateTime? statusUpdatedDateTime { get; set; }
        public int state { get; set; }
        public string stateName { get; set; }
        public string intent { get; set; }
        public string createdBy { get; set; }
        public string createdDateTime { get; set; }
        public string updatedBy { get; set; }
        public DateTime? updateDateTime { get; set; }
        public string lastCustomerMessage { get; set; }
        public DateTime? lastCustomerMessageDatetime { get; set; }
        public string lastAgentMessage { get; set; }
        public DateTime? lastAgentMessageDatetime { get; set; }
        public string expiredBy { get; set; }
        public DateTime? expiredDateTime { get; set; }
        public int transferCount { get; set; }
        public string otherData { get; set; }
        public List<SlaDetails> slaDetails { get; set; }
    }
    public class Response
    {
        public string resultCode { get; set; }
        public string errorMessage { get; set; }
        public List<AsyncChat> data { get; set; }
    }
    public class SlaDetails
    {
        public string caseId { get; set; }
        public string slaId { get; set; }
        public string slaName { get; set; }
        public DateTime? timestamp { get; set; }
        public int slaGiven { get; set; }
        public int slaElapsed { get; set; }
        public int slaMet { get; set; }
        public int status { get; set; }
    }
    public class RawData
    {
        public string createdDateTime { get; set; }
        public bool fromDB { get; set; }
    }
}
