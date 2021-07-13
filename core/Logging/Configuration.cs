using System;
using System.Collections.Generic;
using System.Diagnostics;
using Glow.Configurations;
using Glow.TypeScript;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace Glow.Core.Logging
{
    public class TsProfile : TypeScriptProfile
    {
        public TsProfile()
        {
            Add<LogEventLevel>();
        }
    }

    [Configuration(
        Id = "logging",
        Policy = "Admin",
        SectionId = LoggingConfiguration.SectionId
    )]
    public class LoggingConfiguration
    {
        public const string SectionId = "Serilog";
        public MinimumLevel MinimumLevel { get; set; }

        public void OnSuccess(IServiceProvider provider)
        {
            IConfiguration? configuration = provider.GetService<IConfiguration>();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }

    public class MinimumLevel
    {
        private string @default;

        public string Default
        {
            get
            {
                return @default;
            }
            set
            {
                @default = Parse(value);
            }
        }

        private Dictionary<string, string> @override;

        public Dictionary<string, string> Override
        {
            get
            {
                return @override;
            }
            set
            {
                // todo: parse only real loglevels
                var dict = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> v in value)
                {
                    dict[v.Key] = Parse(v.Value);
                }

                @override = dict;
            }
        }

        private string Parse(string input)
        {
            if (Enum.TryParse<LogEventLevel>(input, out var level))
            {
                return input;
            }
            else
            {
                return null;
            }
        }
    }
}