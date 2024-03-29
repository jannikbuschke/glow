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

        private string ParsePath(string tokenPath)
        {
            if (tokenPath.Contains("['"))
            {
                // MinimumLevel.Override['Microsoft.EntityFrameworkCore']
                var split = tokenPath.Split("['");
                var first = split[0]
                    .Replace("[", ":")
                    .Replace("].", ":")
                    .Replace(".", ":");
                return first + ":" + split[1].Replace("']", "");
            }
            else
            {
                var path = tokenPath
                    .Replace("[", ":")
                    .Replace("].", ":")
                    .Replace(".", ":");
                return path;
            }
        }

        private void AddAllValues(JToken token, Dictionary<string, object> result)
        {
            if (token.Path.Contains("['"))
            {
                var split = token.Path.Split("['");
            }

            var path = ParsePath(token.Path);

            switch (token.Type)
            {
                case JTokenType.String:
                    {
                        var value = token.Value<string>();
                        result[path] = value;
                        break;
                    }
                case JTokenType.Integer:
                    {
                        result[path] = token.Value<long>();
                        break;
                    }
                case JTokenType.Float:
                    {
                        result[path] = token.Value<double>();
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
                        foreach (JToken item in o.Children())
                        {
                            AddAllValues(item, result);
                        }

                        break;
                    }
                case JTokenType.Array:
                    {
                        var array = token as JArray;
                        foreach (JToken item in array.Children())
                        {
                            AddAllValues(item, result);
                        }

                        break;
                    }
                case JTokenType.Null:
                    {
                        //no-op
                        break;
                    }
                case JTokenType.Boolean:
                    {
                        result[path] = token.Value<bool>();
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