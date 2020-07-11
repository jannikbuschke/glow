using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;

namespace Glow.Configurations
{
    public class Configurations
    {
        private readonly AssembliesCache assemblies;

        public Configurations(AssembliesCache assemblies)
        {
            this.assemblies = assemblies;
        }

        public IEnumerable<Meta> Get()
        {
            IEnumerable<Meta> attributes = assemblies
                .SelectMany(v => v.GetExportedTypes().Where(v => v.GetCustomAttributes(typeof(ConfigurationAttribute), true).Any())
                    .SelectMany(v => v.GetCustomAttributes<ConfigurationAttribute>())
                ).Select(v => v.ToPartialConfiguration());
            return attributes;
        }
    }
}
