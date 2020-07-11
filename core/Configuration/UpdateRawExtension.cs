using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Glow.Configurations
{
    public static class UpdateRawExtension
    {
        public static Update ToPartialUpdate<T>(this UpdateRaw<T> value)
        {
            var json = JsonConvert.SerializeObject(value.Value);
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return new Update
            {
                ConfigurationId = value.ConfigurationId,
                Values = values.Select(v => new ConfigurationValue { Name = v.Key, Value = v.Value }).ToArray()
            };
        }
    }
}
