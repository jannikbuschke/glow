using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Glow.Configurations
{
    public class ConfigurationControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                Type genericType = controller.ControllerType.GenericTypeArguments[0];
                ConfigurationAttribute configuration = genericType.GetCustomAttribute<ConfigurationAttribute>();

                if (configuration?.Route != null)
                {
                    if (configuration.ReadPolicy == null)
                    {
                        if (configuration.Policy == null)
                        {
                            throw new Exception($"You need to set a policy for configuration '{configuration.Id}'");
                        }

                        controller.Filters.Add(new AuthorizeFilter(configuration.Policy));
                    }
                    else
                    {
                        controller.Actions.Single(v => v.ActionName == "Post").Filters
                            .Add(new AuthorizeFilter(configuration.Policy));
                        controller.Filters.Add(new AuthorizeFilter(configuration.ReadPolicy));
                    }

                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel =
                            new AttributeRouteModel(new RouteAttribute("api/configurations/" + configuration.Id)),
                    });
                }
            }
        }
    }
}