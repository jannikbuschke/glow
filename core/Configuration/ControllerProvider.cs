using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Glow.Configurations
{
    public class ControllerProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IEnumerable<Assembly> assemblies;

        public ControllerProvider(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            IEnumerable<Type> candidates = assemblies
                .SelectMany(v => v.GetExportedTypes()
                .Where(x => x.GetCustomAttributes(typeof(ConfigurationAttribute), true).Any()));

            foreach (Type candidate in candidates)
            {
                feature.Controllers.Add(
                    typeof(UpdateController<>).MakeGenericType(candidate).GetTypeInfo()
                );
            }
        }
    }
}
