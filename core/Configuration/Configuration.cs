using System;
using System.Collections.Generic;

namespace Glow.Configurations
{
    public class Configuration
    {
        public int Id { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public DateTime Created { get; set; }
        public string User { get; set; }
    }
}
