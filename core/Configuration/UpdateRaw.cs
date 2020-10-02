using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                var obj = JObject.FromObject(Value);
                var result = new Dictionary<string, object>();
                foreach (JToken v in obj.Children())
                {
                    AddAllValues(v, result);
                }
                return result;
            }
        }

        private void AddAllValues(JToken token, Dictionary<string,object> result)
        {
            var path = token.Path.Replace(".", ":");
            switch (token.Type)
            {
                case JTokenType.String:
                    {
                        result[path] = token.Value<string>();
                        break;
                    }
                case JTokenType.Integer:
                    {
                        result[path] = token.Value<long>();
                        break;
                    }
                case JTokenType.Property:
                    {
                        var property = token as JProperty;
                        AddAllValues(property.Value, result);
                        break;
                    }
                case JTokenType.Object:
                    {
                        var o = token as JObject;
                        foreach (var item in o.Children())
                        {
                            AddAllValues(item, result);
                        }
                        break;
                    }
                default:
                    {
                        throw new System.Exception($"Not Supported JTokenType '{token.Type}'");
                    }
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
