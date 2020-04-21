
using System.Configuration;

namespace SIPDataCollector.Utilites
{
    public class BcmsSIPConfigSection: ConfigurationSection
    {
        [ConfigurationProperty("BCMSSkill")]
        public ServiceCollection BCMSServiceItems
        {
            get { return ((ServiceCollection)(base["BCMSSkill"])); }
        }
    }

    public class BCMSInstanceData : ConfigurationSection
    {
        [ConfigurationProperty("ChannelName", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string ChannelName
        {
            get { return (string)base["ChannelName"]; }
            set { base["ChannelName"] = value; }
        }

        [ConfigurationProperty("SkillId", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string SkillId
        {
            get { return (string)base["SkillId"]; }
            set { base["SkillId"] = value; }
        }
    }

    [ConfigurationCollection(typeof(BCMSInstanceData))]
    public class ServiceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BCMSInstanceData();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BCMSInstanceData)(element)).ChannelName;
        }

        public BCMSInstanceData this[int idx]
        {
            get
            {
                return (BCMSInstanceData)BaseGet(idx);
            }
        }

        public BCMSInstanceData this[string name]
        {
            get
            {
                return (BCMSInstanceData)BaseGet(name);
            }
        }
    }
}
