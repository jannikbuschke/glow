using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glow.Core.Typescript;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Glow.TypeScript
{
    public class TsGenerationOptions
    {
        public string Path { get; set; }
        public bool GenerateApi { get; set; }
        public ApiOptions ApiOptions { get; set; }
        public Assembly[] Assemblies { get; set; }
        public Action<OneOf<TsType, TsEnum>> Update { get; set; }

        public string GetPath()
        {
            var configuredPath = Path ?? "web/src/";
            if (!configuredPath.EndsWith(".ts") && !configuredPath.EndsWith("/"))
            {
                throw new Exception($"configured path '{configuredPath}' must end with / or .ts");
            }

            return configuredPath;
        }
    }

    public class ApiOptions
    {
        public List<string> ApiFileFirstLines = new List<string>()
        {
            @"/* eslint-disable prettier/prettier */"
        };

        public void AddFirstLine(string line)
        {
            ApiFileFirstLines.Add(line);
        }
    }

    public static class Extensions
    {
        public static string CamelCase(this string value)
        {
            return (char.ToLowerInvariant(value[0]) + value.Substring(1))
                .Replace("_", string.Empty);
        }

        public static bool IsEnumerable(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>) || t.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static void AddTypescriptGeneration(
            this IServiceCollection services,
            params TsGenerationOptions[] options
        )
        {
            services.AddTransient<GenerateTsModelsAtStartupV2>(provider => new GenerateTsModelsAtStartupV2(
                provider.GetService<IWebHostEnvironment>(),
                options,
                provider.GetService<ILogger<GenerateTsModelsAtStartupV2>>()
            ));
            services.AddHostedService(provider => new GenerateTsModelsAtStartupV2(
                provider.GetService<IWebHostEnvironment>(),
                options,
                provider.GetService<ILogger<GenerateTsModelsAtStartupV2>>()
            ));
        }
    }
}
