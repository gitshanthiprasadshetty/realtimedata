﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Connector.TMACWorkQ {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="WorkItem", Namespace="http://schemas.datacontract.org/2004/07/TetherfiWorkAssignment")]
    [System.SerializableAttribute()]
    public partial class WorkItem : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime AddedTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AgentIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ChannelField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreatedByField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CustomerIdentifierField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ItemIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string KeyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int OrderIndexField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReasonField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RouteDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RouteTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SkillField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int StatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SubChannelField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime AddedTime {
            get {
                return this.AddedTimeField;
            }
            set {
                if ((this.AddedTimeField.Equals(value) != true)) {
                    this.AddedTimeField = value;
                    this.RaisePropertyChanged("AddedTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AgentID {
            get {
                return this.AgentIDField;
            }
            set {
                if ((object.ReferenceEquals(this.AgentIDField, value) != true)) {
                    this.AgentIDField = value;
                    this.RaisePropertyChanged("AgentID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Channel {
            get {
                return this.ChannelField;
            }
            set {
                if ((object.ReferenceEquals(this.ChannelField, value) != true)) {
                    this.ChannelField = value;
                    this.RaisePropertyChanged("Channel");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreatedBy {
            get {
                return this.CreatedByField;
            }
            set {
                if ((object.ReferenceEquals(this.CreatedByField, value) != true)) {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomerIdentifier {
            get {
                return this.CustomerIdentifierField;
            }
            set {
                if ((object.ReferenceEquals(this.CustomerIdentifierField, value) != true)) {
                    this.CustomerIdentifierField = value;
                    this.RaisePropertyChanged("CustomerIdentifier");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ItemID {
            get {
                return this.ItemIDField;
            }
            set {
                if ((object.ReferenceEquals(this.ItemIDField, value) != true)) {
                    this.ItemIDField = value;
                    this.RaisePropertyChanged("ItemID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Key {
            get {
                return this.KeyField;
            }
            set {
                if ((object.ReferenceEquals(this.KeyField, value) != true)) {
                    this.KeyField = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int OrderIndex {
            get {
                return this.OrderIndexField;
            }
            set {
                if ((this.OrderIndexField.Equals(value) != true)) {
                    this.OrderIndexField = value;
                    this.RaisePropertyChanged("OrderIndex");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Reason {
            get {
                return this.ReasonField;
            }
            set {
                if ((object.ReferenceEquals(this.ReasonField, value) != true)) {
                    this.ReasonField = value;
                    this.RaisePropertyChanged("Reason");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RouteDate {
            get {
                return this.RouteDateField;
            }
            set {
                if ((object.ReferenceEquals(this.RouteDateField, value) != true)) {
                    this.RouteDateField = value;
                    this.RaisePropertyChanged("RouteDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RouteTime {
            get {
                return this.RouteTimeField;
            }
            set {
                if ((object.ReferenceEquals(this.RouteTimeField, value) != true)) {
                    this.RouteTimeField = value;
                    this.RaisePropertyChanged("RouteTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Skill {
            get {
                return this.SkillField;
            }
            set {
                if ((object.ReferenceEquals(this.SkillField, value) != true)) {
                    this.SkillField = value;
                    this.RaisePropertyChanged("Skill");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Status {
            get {
                return this.StatusField;
            }
            set {
                if ((this.StatusField.Equals(value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SubChannel {
            get {
                return this.SubChannelField;
            }
            set {
                if ((object.ReferenceEquals(this.SubChannelField, value) != true)) {
                    this.SubChannelField = value;
                    this.RaisePropertyChanged("SubChannel");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TMACWorkQ.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AddWorkItem", ReplyAction="http://tempuri.org/IService/AddWorkItemResponse")]
        string AddWorkItem(string channel, string skill, string itemid, string insertBy, string subchannel, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AddWorkItem", ReplyAction="http://tempuri.org/IService/AddWorkItemResponse")]
        System.Threading.Tasks.Task<string> AddWorkItemAsync(string channel, string skill, string itemid, string insertBy, string subchannel, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AddWorkItemSync", ReplyAction="http://tempuri.org/IService/AddWorkItemSyncResponse")]
        string AddWorkItemSync(Connector.TMACWorkQ.WorkItem item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AddWorkItemSync", ReplyAction="http://tempuri.org/IService/AddWorkItemSyncResponse")]
        System.Threading.Tasks.Task<string> AddWorkItemSyncAsync(Connector.TMACWorkQ.WorkItem item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RemoveWorkItemSync", ReplyAction="http://tempuri.org/IService/RemoveWorkItemSyncResponse")]
        string RemoveWorkItemSync(string key);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RemoveWorkItemSync", ReplyAction="http://tempuri.org/IService/RemoveWorkItemSyncResponse")]
        System.Threading.Tasks.Task<string> RemoveWorkItemSyncAsync(string key);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/UpdateWorkItemStatusSync", ReplyAction="http://tempuri.org/IService/UpdateWorkItemStatusSyncResponse")]
        void UpdateWorkItemStatusSync(string key, int status);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/UpdateWorkItemStatusSync", ReplyAction="http://tempuri.org/IService/UpdateWorkItemStatusSyncResponse")]
        System.Threading.Tasks.Task UpdateWorkItemStatusSyncAsync(string key, int status);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RemoveWorkItemByItemIDAndChannel", ReplyAction="http://tempuri.org/IService/RemoveWorkItemByItemIDAndChannelResponse")]
        int RemoveWorkItemByItemIDAndChannel(string channel, string itemid, string removedBy, string reason);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RemoveWorkItemByItemIDAndChannel", ReplyAction="http://tempuri.org/IService/RemoveWorkItemByItemIDAndChannelResponse")]
        System.Threading.Tasks.Task<int> RemoveWorkItemByItemIDAndChannelAsync(string channel, string itemid, string removedBy, string reason);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RemoveWorkItemByKey", ReplyAction="http://tempuri.org/IService/RemoveWorkItemByKeyResponse")]
        int RemoveWorkItemByKey(string key, string removedBy, string reason);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RemoveWorkItemByKey", ReplyAction="http://tempuri.org/IService/RemoveWorkItemByKeyResponse")]
        System.Threading.Tasks.Task<int> RemoveWorkItemByKeyAsync(string key, string removedBy, string reason);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetWorkItem", ReplyAction="http://tempuri.org/IService/GetWorkItemResponse")]
        string GetWorkItem(string channel, string agentid, string[] skills, string status);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetWorkItem", ReplyAction="http://tempuri.org/IService/GetWorkItemResponse")]
        System.Threading.Tasks.Task<string> GetWorkItemAsync(string channel, string agentid, string[] skills, string status);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ReloadEmailQueue", ReplyAction="http://tempuri.org/IService/ReloadEmailQueueResponse")]
        void ReloadEmailQueue();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ReloadEmailQueue", ReplyAction="http://tempuri.org/IService/ReloadEmailQueueResponse")]
        System.Threading.Tasks.Task ReloadEmailQueueAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ReloadQueue", ReplyAction="http://tempuri.org/IService/ReloadQueueResponse")]
        void ReloadQueue();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ReloadQueue", ReplyAction="http://tempuri.org/IService/ReloadQueueResponse")]
        System.Threading.Tasks.Task ReloadQueueAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ClearQueue", ReplyAction="http://tempuri.org/IService/ClearQueueResponse")]
        int ClearQueue(string queue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ClearQueue", ReplyAction="http://tempuri.org/IService/ClearQueueResponse")]
        System.Threading.Tasks.Task<int> ClearQueueAsync(string queue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ClearAllQueue", ReplyAction="http://tempuri.org/IService/ClearAllQueueResponse")]
        void ClearAllQueue();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/ClearAllQueue", ReplyAction="http://tempuri.org/IService/ClearAllQueueResponse")]
        System.Threading.Tasks.Task ClearAllQueueAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetQueueDetails", ReplyAction="http://tempuri.org/IService/GetQueueDetailsResponse")]
        string GetQueueDetails(string queue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetQueueDetails", ReplyAction="http://tempuri.org/IService/GetQueueDetailsResponse")]
        System.Threading.Tasks.Task<string> GetQueueDetailsAsync(string queue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetQueueCount", ReplyAction="http://tempuri.org/IService/GetQueueCountResponse")]
        string GetQueueCount(string queue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetQueueCount", ReplyAction="http://tempuri.org/IService/GetQueueCountResponse")]
        System.Threading.Tasks.Task<string> GetQueueCountAsync(string queue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemForAgentByChannelAndItemID", ReplyAction="http://tempuri.org/IService/PullWorkItemForAgentByChannelAndItemIDResponse")]
        string PullWorkItemForAgentByChannelAndItemID(string channel, string itemid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemForAgentByChannelAndItemID", ReplyAction="http://tempuri.org/IService/PullWorkItemForAgentByChannelAndItemIDResponse")]
        System.Threading.Tasks.Task<string> PullWorkItemForAgentByChannelAndItemIDAsync(string channel, string itemid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemForAgentByGlobalKey", ReplyAction="http://tempuri.org/IService/PullWorkItemForAgentByGlobalKeyResponse")]
        string PullWorkItemForAgentByGlobalKey(string key, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemForAgentByGlobalKey", ReplyAction="http://tempuri.org/IService/PullWorkItemForAgentByGlobalKeyResponse")]
        System.Threading.Tasks.Task<string> PullWorkItemForAgentByGlobalKeyAsync(string key, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetWorkItems", ReplyAction="http://tempuri.org/IService/GetWorkItemsResponse")]
        string GetWorkItems(string channel, string[] skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetWorkItems", ReplyAction="http://tempuri.org/IService/GetWorkItemsResponse")]
        System.Threading.Tasks.Task<string> GetWorkItemsAsync(string channel, string[] skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetWorkItemsBySubChannel", ReplyAction="http://tempuri.org/IService/GetWorkItemsBySubChannelResponse")]
        string GetWorkItemsBySubChannel(string subchannel, string[] skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetWorkItemsBySubChannel", ReplyAction="http://tempuri.org/IService/GetWorkItemsBySubChannelResponse")]
        System.Threading.Tasks.Task<string> GetWorkItemsBySubChannelAsync(string subchannel, string[] skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemsForAgentByChannelAndCustomerId", ReplyAction="http://tempuri.org/IService/PullWorkItemsForAgentByChannelAndCustomerIdResponse")]
        string PullWorkItemsForAgentByChannelAndCustomerId(string channel, string customerid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemsForAgentByChannelAndCustomerId", ReplyAction="http://tempuri.org/IService/PullWorkItemsForAgentByChannelAndCustomerIdResponse")]
        System.Threading.Tasks.Task<string> PullWorkItemsForAgentByChannelAndCustomerIdAsync(string channel, string customerid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AddWorkItemWithCustomerIdentifier", ReplyAction="http://tempuri.org/IService/AddWorkItemWithCustomerIdentifierResponse")]
        string AddWorkItemWithCustomerIdentifier(string channel, string skill, string itemid, string insertBy, string subchannel, string customerIdentifier, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AddWorkItemWithCustomerIdentifier", ReplyAction="http://tempuri.org/IService/AddWorkItemWithCustomerIdentifierResponse")]
        System.Threading.Tasks.Task<string> AddWorkItemWithCustomerIdentifierAsync(string channel, string skill, string itemid, string insertBy, string subchannel, string customerIdentifier, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemsForAgent", ReplyAction="http://tempuri.org/IService/PullWorkItemsForAgentResponse")]
        string PullWorkItemsForAgent(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PullWorkItemsForAgent", ReplyAction="http://tempuri.org/IService/PullWorkItemsForAgentResponse")]
        System.Threading.Tasks.Task<string> PullWorkItemsForAgentAsync(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SearchWorkItems", ReplyAction="http://tempuri.org/IService/SearchWorkItemsResponse")]
        string SearchWorkItems(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SearchWorkItems", ReplyAction="http://tempuri.org/IService/SearchWorkItemsResponse")]
        System.Threading.Tasks.Task<string> SearchWorkItemsAsync(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SetWorkItemAssignedToAgent", ReplyAction="http://tempuri.org/IService/SetWorkItemAssignedToAgentResponse")]
        void SetWorkItemAssignedToAgent(string channel, string itemid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SetWorkItemAssignedToAgent", ReplyAction="http://tempuri.org/IService/SetWorkItemAssignedToAgentResponse")]
        System.Threading.Tasks.Task SetWorkItemAssignedToAgentAsync(string channel, string itemid, string agentid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/UpdateWorkItemStatus", ReplyAction="http://tempuri.org/IService/UpdateWorkItemStatusResponse")]
        void UpdateWorkItemStatus(string channel, string itemid, string updateby);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/UpdateWorkItemStatus", ReplyAction="http://tempuri.org/IService/UpdateWorkItemStatusResponse")]
        System.Threading.Tasks.Task UpdateWorkItemStatusAsync(string channel, string itemid, string updateby);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : Connector.TMACWorkQ.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<Connector.TMACWorkQ.IService>, Connector.TMACWorkQ.IService {
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AddWorkItem(string channel, string skill, string itemid, string insertBy, string subchannel, string data) {
            return base.Channel.AddWorkItem(channel, skill, itemid, insertBy, subchannel, data);
        }
        
        public System.Threading.Tasks.Task<string> AddWorkItemAsync(string channel, string skill, string itemid, string insertBy, string subchannel, string data) {
            return base.Channel.AddWorkItemAsync(channel, skill, itemid, insertBy, subchannel, data);
        }
        
        public string AddWorkItemSync(Connector.TMACWorkQ.WorkItem item) {
            return base.Channel.AddWorkItemSync(item);
        }
        
        public System.Threading.Tasks.Task<string> AddWorkItemSyncAsync(Connector.TMACWorkQ.WorkItem item) {
            return base.Channel.AddWorkItemSyncAsync(item);
        }
        
        public string RemoveWorkItemSync(string key) {
            return base.Channel.RemoveWorkItemSync(key);
        }
        
        public System.Threading.Tasks.Task<string> RemoveWorkItemSyncAsync(string key) {
            return base.Channel.RemoveWorkItemSyncAsync(key);
        }
        
        public void UpdateWorkItemStatusSync(string key, int status) {
            base.Channel.UpdateWorkItemStatusSync(key, status);
        }
        
        public System.Threading.Tasks.Task UpdateWorkItemStatusSyncAsync(string key, int status) {
            return base.Channel.UpdateWorkItemStatusSyncAsync(key, status);
        }
        
        public int RemoveWorkItemByItemIDAndChannel(string channel, string itemid, string removedBy, string reason) {
            return base.Channel.RemoveWorkItemByItemIDAndChannel(channel, itemid, removedBy, reason);
        }
        
        public System.Threading.Tasks.Task<int> RemoveWorkItemByItemIDAndChannelAsync(string channel, string itemid, string removedBy, string reason) {
            return base.Channel.RemoveWorkItemByItemIDAndChannelAsync(channel, itemid, removedBy, reason);
        }
        
        public int RemoveWorkItemByKey(string key, string removedBy, string reason) {
            return base.Channel.RemoveWorkItemByKey(key, removedBy, reason);
        }
        
        public System.Threading.Tasks.Task<int> RemoveWorkItemByKeyAsync(string key, string removedBy, string reason) {
            return base.Channel.RemoveWorkItemByKeyAsync(key, removedBy, reason);
        }
        
        public string GetWorkItem(string channel, string agentid, string[] skills, string status) {
            return base.Channel.GetWorkItem(channel, agentid, skills, status);
        }
        
        public System.Threading.Tasks.Task<string> GetWorkItemAsync(string channel, string agentid, string[] skills, string status) {
            return base.Channel.GetWorkItemAsync(channel, agentid, skills, status);
        }
        
        public void ReloadEmailQueue() {
            base.Channel.ReloadEmailQueue();
        }
        
        public System.Threading.Tasks.Task ReloadEmailQueueAsync() {
            return base.Channel.ReloadEmailQueueAsync();
        }
        
        public void ReloadQueue() {
            base.Channel.ReloadQueue();
        }
        
        public System.Threading.Tasks.Task ReloadQueueAsync() {
            return base.Channel.ReloadQueueAsync();
        }
        
        public int ClearQueue(string queue) {
            return base.Channel.ClearQueue(queue);
        }
        
        public System.Threading.Tasks.Task<int> ClearQueueAsync(string queue) {
            return base.Channel.ClearQueueAsync(queue);
        }
        
        public void ClearAllQueue() {
            base.Channel.ClearAllQueue();
        }
        
        public System.Threading.Tasks.Task ClearAllQueueAsync() {
            return base.Channel.ClearAllQueueAsync();
        }
        
        public string GetQueueDetails(string queue) {
            return base.Channel.GetQueueDetails(queue);
        }
        
        public System.Threading.Tasks.Task<string> GetQueueDetailsAsync(string queue) {
            return base.Channel.GetQueueDetailsAsync(queue);
        }
        
        public string GetQueueCount(string queue) {
            return base.Channel.GetQueueCount(queue);
        }
        
        public System.Threading.Tasks.Task<string> GetQueueCountAsync(string queue) {
            return base.Channel.GetQueueCountAsync(queue);
        }
        
        public string PullWorkItemForAgentByChannelAndItemID(string channel, string itemid, string agentid) {
            return base.Channel.PullWorkItemForAgentByChannelAndItemID(channel, itemid, agentid);
        }
        
        public System.Threading.Tasks.Task<string> PullWorkItemForAgentByChannelAndItemIDAsync(string channel, string itemid, string agentid) {
            return base.Channel.PullWorkItemForAgentByChannelAndItemIDAsync(channel, itemid, agentid);
        }
        
        public string PullWorkItemForAgentByGlobalKey(string key, string agentid) {
            return base.Channel.PullWorkItemForAgentByGlobalKey(key, agentid);
        }
        
        public System.Threading.Tasks.Task<string> PullWorkItemForAgentByGlobalKeyAsync(string key, string agentid) {
            return base.Channel.PullWorkItemForAgentByGlobalKeyAsync(key, agentid);
        }
        
        public string GetWorkItems(string channel, string[] skills) {
            return base.Channel.GetWorkItems(channel, skills);
        }
        
        public System.Threading.Tasks.Task<string> GetWorkItemsAsync(string channel, string[] skills) {
            return base.Channel.GetWorkItemsAsync(channel, skills);
        }
        
        public string GetWorkItemsBySubChannel(string subchannel, string[] skills) {
            return base.Channel.GetWorkItemsBySubChannel(subchannel, skills);
        }
        
        public System.Threading.Tasks.Task<string> GetWorkItemsBySubChannelAsync(string subchannel, string[] skills) {
            return base.Channel.GetWorkItemsBySubChannelAsync(subchannel, skills);
        }
        
        public string PullWorkItemsForAgentByChannelAndCustomerId(string channel, string customerid, string agentid) {
            return base.Channel.PullWorkItemsForAgentByChannelAndCustomerId(channel, customerid, agentid);
        }
        
        public System.Threading.Tasks.Task<string> PullWorkItemsForAgentByChannelAndCustomerIdAsync(string channel, string customerid, string agentid) {
            return base.Channel.PullWorkItemsForAgentByChannelAndCustomerIdAsync(channel, customerid, agentid);
        }
        
        public string AddWorkItemWithCustomerIdentifier(string channel, string skill, string itemid, string insertBy, string subchannel, string customerIdentifier, string data) {
            return base.Channel.AddWorkItemWithCustomerIdentifier(channel, skill, itemid, insertBy, subchannel, customerIdentifier, data);
        }
        
        public System.Threading.Tasks.Task<string> AddWorkItemWithCustomerIdentifierAsync(string channel, string skill, string itemid, string insertBy, string subchannel, string customerIdentifier, string data) {
            return base.Channel.AddWorkItemWithCustomerIdentifierAsync(channel, skill, itemid, insertBy, subchannel, customerIdentifier, data);
        }
        
        public string PullWorkItemsForAgent(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid, string agentid) {
            return base.Channel.PullWorkItemsForAgent(itemid, channel, skill, datacontains, subchannel, customerid, agentid);
        }
        
        public System.Threading.Tasks.Task<string> PullWorkItemsForAgentAsync(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid, string agentid) {
            return base.Channel.PullWorkItemsForAgentAsync(itemid, channel, skill, datacontains, subchannel, customerid, agentid);
        }
        
        public string SearchWorkItems(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid) {
            return base.Channel.SearchWorkItems(itemid, channel, skill, datacontains, subchannel, customerid);
        }
        
        public System.Threading.Tasks.Task<string> SearchWorkItemsAsync(string itemid, string channel, string skill, string datacontains, string subchannel, string customerid) {
            return base.Channel.SearchWorkItemsAsync(itemid, channel, skill, datacontains, subchannel, customerid);
        }
        
        public void SetWorkItemAssignedToAgent(string channel, string itemid, string agentid) {
            base.Channel.SetWorkItemAssignedToAgent(channel, itemid, agentid);
        }
        
        public System.Threading.Tasks.Task SetWorkItemAssignedToAgentAsync(string channel, string itemid, string agentid) {
            return base.Channel.SetWorkItemAssignedToAgentAsync(channel, itemid, agentid);
        }
        
        public void UpdateWorkItemStatus(string channel, string itemid, string updateby) {
            base.Channel.UpdateWorkItemStatus(channel, itemid, updateby);
        }
        
        public System.Threading.Tasks.Task UpdateWorkItemStatusAsync(string channel, string itemid, string updateby) {
            return base.Channel.UpdateWorkItemStatusAsync(channel, itemid, updateby);
        }
    }
}
