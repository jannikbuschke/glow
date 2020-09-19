using System;
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
                    controller.Filters.Add(new AuthorizeFilter(configuration.Policy));
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute("api/configurations/" + configuration.Id)),
                    });
                }
            }
        }
    }
}
