using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Glow.Core.Actions
{
    public class ActionsControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                Type genericType = controller.ControllerType.GenericTypeArguments[0];
                ActionAttribute attribute = genericType.GetCustomAttribute<ActionAttribute>();
                if (attribute != null)
                {
                    // TODO: remove ApiControllerAttribute
                    controller.Filters.Add(new ApiControllerAttribute());
                    controller.Filters.Add(new AuthorizeFilter(attribute.Policy));
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(attribute.Route)),
                    });
                }
            }
        }
    }
}