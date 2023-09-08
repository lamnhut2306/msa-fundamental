using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSA.Common.Contracts.Settings
{
    public class MongoDbSetting
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}