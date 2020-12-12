using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.TypeScript;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;

namespace Glow.Core.Typescript
{
    public class GenerateTsModelsAtStartupV2 : IHostedService
    {
        private readonly IApiDescriptionGroupCollectionProvider descriptionGroupCollectionProvider;
        private readonly IApiDescriptionProvider descriptionProvider;
        private readonly IWebHostEnvironment environment;
        private readonly AssembliesToScan assembliesToScan;
        private readonly ILogger<GenerateTsModelsAtStartup> logger;
        private readonly Options options;

        public GenerateTsModelsAtStartupV2(
            IApiDescriptionGroupCollectionProvider descriptionGroupCollectionProvider,
            IApiDescriptionProvider descriptionProvider,
            IWebHostEnvironment environment,
            AssembliesToScan assembliesToScan,
            ILogger<GenerateTsModelsAtStartup> logger,
            Options options
        )
        {
            this.descriptionGroupCollectionProvider = descriptionGroupCollectionProvider;
            this.descriptionProvider = descriptionProvider;
            this.environment = environment;
            this.assembliesToScan = assembliesToScan;
            this.logger = logger;
            this.options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!environment.IsDevelopment())
            {
                Console.WriteLine("Skip TS model generation");
                return Task.CompletedTask;
            }
            Console.WriteLine("Generate TS models");
            IEnumerable<Type> profileTypes = assembliesToScan.Value
                .SelectMany(v => v.GetTypes())
                .Where(v => v.IsSubclassOf(typeof(TypeScriptProfile)));

            var builder = new TypeCollection();
            // duplication?
            profileTypes.Select(v => Activator.CreateInstance(v) as TypeScriptProfile)
                .ForEach(type =>
                {
                    builder.AddRange(type.Types);
                });

            IEnumerable<Type> additionalTypes = assembliesToScan.Value
                .SelectMany(v => v.GetExportedTypes()
                .Where(x => x.GetCustomAttributes(typeof(GenerateTsInterface), true).Any()));

            builder.AddRange(additionalTypes);
  
            //builder.Insert(0, "/* eslint-disable prettier/prettier */");
            var path = $"{options.GetPath()}ts-models.ts";

            Render.ToDisk(builder.Generate(), path);

            //File.WriteAllText(path, text);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
