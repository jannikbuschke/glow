using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;

namespace Glow.Core.Actions
{
    public static class GetActionsExtension
    {
        public static IEnumerable<ActionMeta> GetActions(this IEnumerable<Assembly> assemblies)
        {
            IEnumerable<Type> candidates = assemblies
                .SelectMany(v => v.GetExportedTypes()
                    .Where(x => x.GetCustomAttributes(typeof(ActionAttribute), true).Any()));

            foreach (Type candidate in candidates)
            {
                ActionAttribute attribute = candidate.GetCustomAttribute<ActionAttribute>();

                Type[] interfaces = candidate.GetInterfaces();

                if (interfaces.Contains(typeof(IRequest)))
                {
                    yield return new ActionMeta()
                    {
                        Input = candidate,
                        Output = typeof(Unit),
                        ActionAttribute = attribute
                    };
                }
                else
                {
                    Type returnType = interfaces
                        .FirstOrDefault(v => v.IsGenericType && v.GetGenericTypeDefinition() == typeof(IRequest<>))
                        ?.GenericTypeArguments
                        ?.FirstOrDefault();

                    yield return new ActionMeta() { Input = candidate, Output = returnType, ActionAttribute = attribute };
                }
            }
        }
    }
}