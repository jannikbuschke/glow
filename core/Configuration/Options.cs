using System.Collections.Generic;

namespace Glow.Configurations
{
    public class ConfigurationOptions
    {
        private readonly Dictionary<string, string> partialReadPolicies = new Dictionary<string, string>();
        private readonly Dictionary<string, string> partialWritePolicies = new Dictionary<string, string>();

        public string GlobalPolicy { get; set; }
        public string ReadAllPolicy { get; set; }
        public string WriteAllPolicy { get; set; }

        public void SetPartialReadPolicy(string configurationId, string policy)
        {
            partialReadPolicies[configurationId] = policy;
        }

        public string GetPartialReadPolicy(string configurationId)
        {
            return partialReadPolicies.GetValueOrDefault(configurationId);
        }

        public void SetPartialWritePolicy(string configurationId, string policy)
        {
            partialWritePolicies[configurationId] = policy;
        }

        public string GetWriteReadPolicy(string configurationId)
        {
            return partialWritePolicies.GetValueOrDefault(configurationId);
        }
    }
}
