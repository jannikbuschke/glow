using System.Collections.Generic;

namespace EfConfigurationProvider.Core
{
    public class Options
    {
        private readonly Dictionary<string, string> partialReadPolicies = new Dictionary<string, string>();
        private readonly Dictionary<string, string> partialWritePolicies = new Dictionary<string, string>();

        public string GlobalPolicy { get; set; }
        public string ReadAllPolicy { get; set; }
        public string WriteAllPolicy { get; set; }

        public void SetPartialReadPolicy(string path, string policy)
        {
            partialReadPolicies[path] = policy;
        }

        public string GetPartialReadPolicy(string path)
        {
            return partialReadPolicies.GetValueOrDefault(path);
        }

        public void SetPartialWritePolicy(string path, string policy)
        {
            partialWritePolicies[path] = policy;
        }

        public string GetWriteReadPolicy(string path)
        {
            return partialWritePolicies.GetValueOrDefault(path);
        }
    }
}
