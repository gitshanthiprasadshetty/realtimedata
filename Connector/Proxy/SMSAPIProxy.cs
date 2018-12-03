using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.Proxy
{
    public class SMSAPIProxy
    {
        static SMSAPI.ServiceClient _smsapi = new SMSAPI.ServiceClient();

        public static List<SMSAPI.HuntGroupType> GetSkills()
        {
            try
            {
                return _smsapi.GetSkillList().ToList();
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
    }
}
