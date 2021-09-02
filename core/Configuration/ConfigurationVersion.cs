using System;
using System.Collections.Generic;

namespace Glow.Configurations
{
    public class ConfigurationVersion
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Values { get; set; }
        public DateTime Created { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
    }
}