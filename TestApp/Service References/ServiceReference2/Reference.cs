﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestApp.ServiceReference2 {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TrunkGroupTraffic", Namespace="http://schemas.datacontract.org/2004/07/CMDataCollector.Models")]
    [System.SerializableAttribute()]
    public partial class TrunkGroupTraffic : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ActiveMembersField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CallsWaitingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int QueueLengthField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TrunkGroupField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TrunkGroupSizeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int activeMembers1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string date1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int queueLength1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int trunkGroup1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int trunkGroupSize1Field;
        
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
        public int ActiveMembers {
            get {
                return this.ActiveMembersField;
            }
            set {
                if ((this.ActiveMembersField.Equals(value) != true)) {
                    this.ActiveMembersField = value;
                    this.RaisePropertyChanged("ActiveMembers");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CallsWaiting {
            get {
                return this.CallsWaitingField;
            }
            set {
                if ((this.CallsWaitingField.Equals(value) != true)) {
                    this.CallsWaitingField = value;
                    this.RaisePropertyChanged("CallsWaiting");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Date {
            get {
                return this.DateField;
            }
            set {
                if ((object.ReferenceEquals(this.DateField, value) != true)) {
                    this.DateField = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int QueueLength {
            get {
                return this.QueueLengthField;
            }
            set {
                if ((this.QueueLengthField.Equals(value) != true)) {
                    this.QueueLengthField = value;
                    this.RaisePropertyChanged("QueueLength");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TrunkGroup {
            get {
                return this.TrunkGroupField;
            }
            set {
                if ((this.TrunkGroupField.Equals(value) != true)) {
                    this.TrunkGroupField = value;
                    this.RaisePropertyChanged("TrunkGroup");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TrunkGroupSize {
            get {
                return this.TrunkGroupSizeField;
            }
            set {
                if ((this.TrunkGroupSizeField.Equals(value) != true)) {
                    this.TrunkGroupSizeField = value;
                    this.RaisePropertyChanged("TrunkGroupSize");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="activeMembers")]
        public int activeMembers1 {
            get {
                return this.activeMembers1Field;
            }
            set {
                if ((this.activeMembers1Field.Equals(value) != true)) {
                    this.activeMembers1Field = value;
                    this.RaisePropertyChanged("activeMembers1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="date")]
        public string date1 {
            get {
                return this.date1Field;
            }
            set {
                if ((object.ReferenceEquals(this.date1Field, value) != true)) {
                    this.date1Field = value;
                    this.RaisePropertyChanged("date1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="queueLength")]
        public int queueLength1 {
            get {
                return this.queueLength1Field;
            }
            set {
                if ((this.queueLength1Field.Equals(value) != true)) {
                    this.queueLength1Field = value;
                    this.RaisePropertyChanged("queueLength1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="trunkGroup")]
        public int trunkGroup1 {
            get {
                return this.trunkGroup1Field;
            }
            set {
                if ((this.trunkGroup1Field.Equals(value) != true)) {
                    this.trunkGroup1Field = value;
                    this.RaisePropertyChanged("trunkGroup1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="trunkGroupSize")]
        public int trunkGroupSize1 {
            get {
                return this.trunkGroupSize1Field;
            }
            set {
                if ((this.trunkGroupSize1Field.Equals(value) != true)) {
                    this.trunkGroupSize1Field = value;
                    this.RaisePropertyChanged("trunkGroupSize1");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference2.IBcmsDashboardService")]
    public interface IBcmsDashboardService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBcmsDashboardService/MonitorBcms", ReplyAction="http://tempuri.org/IBcmsDashboardService/MonitorBcmsResponse")]
        CMDataCollector.Models.BcmsDashboard[] MonitorBcms();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBcmsDashboardService/MonitorBcmsForSkill", ReplyAction="http://tempuri.org/IBcmsDashboardService/MonitorBcmsForSkillResponse")]
        CMDataCollector.Models.BcmsDashboard MonitorBcmsForSkill(string skillToMonitor);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBcmsDashboardService/MonitorTrunkTraffic", ReplyAction="http://tempuri.org/IBcmsDashboardService/MonitorTrunkTrafficResponse")]
        TestApp.ServiceReference2.TrunkGroupTraffic[] MonitorTrunkTraffic();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBcmsDashboardService/IsRunning", ReplyAction="http://tempuri.org/IBcmsDashboardService/IsRunningResponse")]
        bool IsRunning();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBcmsDashboardService/IsConnectedToCM", ReplyAction="http://tempuri.org/IBcmsDashboardService/IsConnectedToCMResponse")]
        bool IsConnectedToCM();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBcmsDashboardServiceChannel : TestApp.ServiceReference2.IBcmsDashboardService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BcmsDashboardServiceClient : System.ServiceModel.ClientBase<TestApp.ServiceReference2.IBcmsDashboardService>, TestApp.ServiceReference2.IBcmsDashboardService {
        
        public BcmsDashboardServiceClient() {
        }
        
        public BcmsDashboardServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BcmsDashboardServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BcmsDashboardServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BcmsDashboardServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public CMDataCollector.Models.BcmsDashboard[] MonitorBcms() {
            return base.Channel.MonitorBcms();
        }
        
        public CMDataCollector.Models.BcmsDashboard MonitorBcmsForSkill(string skillToMonitor) {
            return base.Channel.MonitorBcmsForSkill(skillToMonitor);
        }
        
        public TestApp.ServiceReference2.TrunkGroupTraffic[] MonitorTrunkTraffic() {
            return base.Channel.MonitorTrunkTraffic();
        }
        
        public bool IsRunning() {
            return base.Channel.IsRunning();
        }
        
        public bool IsConnectedToCM() {
            return base.Channel.IsConnectedToCM();
        }
    }
}
