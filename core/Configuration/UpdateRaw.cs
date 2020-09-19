using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glow.Configurations
{
    public class UpdateRaw<T>
    {
        public string ConfigurationId { get; set; }
        public T Value { get; set; }

        public Dictionary<string, object> ConfigurationValues
        {
            get
            {
                var json = JsonConvert.SerializeObject(Value);
                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                return values;
            }
        }

        public ConfigurationUpdate ToConfigurationUpdate()
        {
            return new ConfigurationUpdate
            {
                ConfigurationId = ConfigurationId,
                Values = ConfigurationValues
            };
        }
    }
}
