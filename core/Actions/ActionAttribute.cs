using System;
using System.Collections;

namespace Glow.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionAttribute : Attribute
    {
        public string Route { get; set; }
        public string Policy { get; set; }
        public bool AllowAnoymous { get; set; }
    }

    public class ActionMeta
    {
        public Type Input { get; set; }
        public Type Output { get; set; }
        public ActionAttribute ActionAttribute { get; set; }
    }
}