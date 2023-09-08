using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSA.Common.Contracts.Settings
{
    public class ServiceUrlSetting
    {
        public string IdentityServiceUrl { get; set; }

        public string ProductServiceUrl { get; set; }
        
        public string OrderServiceUrl { get; set; }
    }
}