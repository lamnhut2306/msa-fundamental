using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSA.Common.Contracts.Settings
{
    public class ServiceEndpointSetting
    {
        public string BankService { get; set; }
        
        public string OrderService { get; set; }
        
        public string ProductService { get; set; }
    }
}