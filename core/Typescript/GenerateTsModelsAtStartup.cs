using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.TypeScript;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;

namespace Glow.Core.Typescript
{
    public class GenerateTsModelsAtStartupV2 : BackgroundService
    {
        private readonly IWebHostEnvironment environment;
        private readonly TsGenerationOptions[] options;
        private readonly ILogger<GenerateTsModelsAtStartupV2> logger;

        public GenerateTsModelsAtStartupV2(
            IWebHostEnvironment environment,
            TsGenerationOptions[] options,
            ILogger<GenerateTsModelsAtStartupV2> logger
        )
        {
            this.environment = environment;
            this.options = options;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // https://github.com/dotnet/runtime/issues/36063#issuecomment-671110933
            await Task.Yield();

            foreach (TsGenerationOptions option in options)
            {
                try
                {
                    await GenerateTypes(option);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while generating types " + e.Message);
                    logger.LogError(e, "Error while generating TS client code");
                }
            }
        }

        private Task GenerateTypes(TsGenerationOptions option)
        {
            if (!environment.IsDevelopment())
            {
                Console.WriteLine("Skip TS model generation");
                return Task.CompletedTask;
            }

            Console.WriteLine("Generate TS models");
            IEnumerable<Type> profileTypes = option.Assemblies
                .SelectMany(v => v.GetTypes())
                .Where(v => v.IsSubclassOf(typeof(TypeScriptProfile)));

            var builder = new TypeCollectionBuilder();
            // duplication?
            profileTypes.Select(v => Activator.CreateInstance(v) as TypeScriptProfile)
                .ForEach(type =>
                {
                    builder.AddRange(type.Types);
                });

            IEnumerable<Type> additionalTypes = option.Assemblies
                .SelectMany(v => v.GetExportedTypes()
                    .Where(x => x.GetCustomAttributes(typeof(GenerateTsInterface), true).Any()));

            var actions = option.Assemblies.GetActions();

            builder.AddRange(actions.Select(v => v.Input));
            builder.AddRange(actions.Select(v => v.Output));

            builder.AddRange(additionalTypes);

            if (builder.Count == 0)
            {
                Console.WriteLine("Skipping Ts generation, as there are not types");
            }
            else
            {
                //builder.Insert(0, "/* eslint-disable prettier/prettier */");

                var path = option.GetPath();

                Console.WriteLine(path);
                TypeCollection typeCollection = builder.Generate(option.Update);
                RenderTypes.ToDisk(typeCollection, path);
                RenderApi.Render(typeCollection, option);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}