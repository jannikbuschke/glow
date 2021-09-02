using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Glow.Core.Actions
{
    public class ActionsControllerProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IEnumerable<Assembly> assemblies;

        public ActionsControllerProvider(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies ?? throw new NullReferenceException();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            Serilog.Log.Logger.Information("Populate " + nameof(ActionsControllerProvider));

            // TODO: use assemblies.GetActions()
            IEnumerable<Type> candidates = assemblies
                .SelectMany(v => v.GetExportedTypes()
                    .Where(x => x.GetCustomAttributes(typeof(ActionAttribute), true).Any()));

            foreach (Type candidate in candidates)
            {
                try
                {
                    Type[] interfaces = candidate.GetInterfaces();

                    if (interfaces.Contains(typeof(IRequest)))
                    {
                        Type returnType = typeof(Unit);
                        feature.Controllers.Add(
                            typeof(ActionController<,>).MakeGenericType(candidate, returnType).GetTypeInfo()
                        );
                    }
                    else
                    {
                        Type returnType = interfaces.Where(v =>
                                v.IsGenericType && v.GetGenericTypeDefinition() == typeof(IRequest<>))
                            .FirstOrDefault()
                            .GenericTypeArguments
                            ?.FirstOrDefault();

                        feature.Controllers.Add(
                            typeof(ActionController<,>).MakeGenericType(candidate, returnType).GetTypeInfo()
                        );
                    }

                    //var g = candidate.GenericTypeArguments;

                    //feature.Controllers.Add(
                    //    typeof(ActionController<,>).MakeGenericType(candidate).GetTypeInfo()
                    //);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not create Action " + candidate.FullName);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}