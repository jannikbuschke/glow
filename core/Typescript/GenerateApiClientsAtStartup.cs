using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Linq;
using Glow.Core.Typescript;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Glow.TypeScript
{
    public class GenerateApiClientsAtStartup : IHostedService
    {
        private readonly IApiDescriptionGroupCollectionProvider descriptionGroupCollectionProvider;
        private readonly IApiDescriptionProvider descriptionProvider;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<GenerateApiClientsAtStartup> logger;
        private readonly Options options;

        public GenerateApiClientsAtStartup(
            IApiDescriptionGroupCollectionProvider descriptionGroupCollectionProvider,
            IApiDescriptionProvider descriptionProvider,
            IWebHostEnvironment environment,
            ILogger<GenerateApiClientsAtStartup> logger,
            Options options
        )
        {
            this.descriptionGroupCollectionProvider = descriptionGroupCollectionProvider;
            this.descriptionProvider = descriptionProvider;
            this.environment = environment;
            this.logger = logger;
            this.options = options;
        }

        public static HashSet<Type> CustomTypes = new HashSet<Type>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!environment.IsDevelopment())
            {
                Console.WriteLine("Skip TS client generation");
                return Task.CompletedTask;
            }
            Console.WriteLine("Generate TS client");

            var descriptions = descriptionGroupCollectionProvider.ApiDescriptionGroups.Items.Select(v => new
            {
                v.GroupName,
                Items = v.Items.Select(v =>
                {
                    var name = v.ActionDescriptor.RouteValues["action"];
                    var controller = v.ActionDescriptor.RouteValues["controller"];
                    var id = $"{controller}_{name}_{v.GroupName}";
                    return new RequestDescription
                    {
                        Id = id,
                        ActionName = name,
                        ControllerName = controller,
                        GroupName = v.GroupName,
                        RelativePath = v.RelativePath,
                        HttpMethod = v.HttpMethod,
                        ParameterDescriptions = v.ParameterDescriptions.Select(v => new ParameterDescription
                        {
                            Name = v.Name,
                            Type = v.Type,
                            Source = v.Source
                        }),
                        //ActionDescriptor = new
                        //{
                        //    v.ActionDescriptor.DisplayName,
                        //    v.ActionDescriptor.RouteValues,
                        //}
                    };
                }).ToList()
            });
            var builder = new StringBuilder();

            foreach (var v in descriptions)
            {
                foreach (RequestDescription a in v.Items.DistinctBy(v => v.Id).OrderBy(v => v.Id).ToList())
                {
                    try
                    {
                        Log.Logger.Information(a.Id);
                        foreach (ParameterDescription item in a.ParameterDescriptions.Where(v => v.Name != "api-version"))
                        {
                            if (!item.Type.FullName.StartsWith("System"))
                            {
                                Log.Logger.Information("type {type} ", item.Type.FullName);
                                CustomTypes.Add(item.Type);
                                Extensions2.AddType(item.Type);
                            }
                        }

                        builder.AppendLine($"export module {a.ControllerName.Replace("`", "")} {{");
                        if (a.HttpMethod?.ToLower() == "post")
                        {
                            builder.Append(
    $@"  export async function {a.ActionName}_{a.GroupName}({string.Join(",", a.ParameterDescriptions.Where(v => v.Name != "api-version").Select(v => $"{v.Name}: {v.Type.ToTsType()}"))}) {{
    const response = await fetch(`/{a.RelativePath}?api-version=1.0`, {{
      method: ""POST"",
      headers: {{ ""content-type"": ""application/json"" }},
      body: JSON.stringify({a.ParameterDescriptions.Where(v => v.Name != "api-version").First().Name}),
    }})
    const data = await response.json()
    return data
  }}
");
                        }
                        else
                        {
                            builder.Append(
    $@"  export async function {a.ActionName}_{a.GroupName}({string.Join(",", a.ParameterDescriptions.Where(v => v.Name != "api-version").Select(v => $"{v.Name}: {v.Type.ToTsType()}"))}) {{
    const response = await fetch(`/{a.RelativePath.Replace("{", "${")}?api-version=1.0`)
    const data = await response.json()
    return data
  }}
");
                        }
                        builder.AppendLine($"}}");
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Warning($"Could not generate client '{a.Id}' ({e.Message})");
                    }
                }
            }

            builder.Insert(0, "\r\n");
            builder.Insert(0, @$"import {{ {string.Join(", ", CustomTypes.Select(v => v.ToTsType()))} }} from ""./ts-models""");
            builder.Insert(0, "\r\n");
            //builder.Insert(0, "/* eslint-disable prettier/prettier */");

            if (options.GenerateApi)
            {
                File.WriteAllText($"{options.GetPath()}ts-api.ts", builder.ToString());
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;

        }
    }
}

