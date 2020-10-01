using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Glow.Configurations
{
    public class UpdateRaw<T> : MediatR.IRequest
    {
        public string ConfigurationId { get; set; }
        public string Name { get; set; } = Options.DefaultName;
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
                Values = ConfigurationValues,
                Name = Name
            };
        }
    }
}
