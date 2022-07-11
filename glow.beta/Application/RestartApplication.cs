using System;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.Core.Authentication;
using Glow.Core.StringExtensions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glow.Core.Application
{
    [Action(AllowAnonymous = true, Route = "api/glow/restart-application")]
    public class RestartApplication : IRequest
    {
    }

    public static class IConfigurationExtensions
    {
        public static bool AllowRestart(this IConfiguration cfg)
        {
            return cfg.GetValue<bool>("AllowConfiguration") == true;
        }
    }


    public class RestartApplicationHandler : IRequestHandler<RestartApplication, Unit>
    {
        private readonly IConfiguration configuration;
        private readonly IApplicationLifetime appLifetime;
        private readonly ILogger<RestartApplicationHandler> logger;

        public RestartApplicationHandler(IConfiguration configuration, IApplicationLifetime appLifetime, ILogger<RestartApplicationHandler> logger)
        {
            this.configuration = configuration;
            this.appLifetime = appLifetime;
            this.logger = logger;
        }

        public Task<Unit> Handle(RestartApplication request, CancellationToken cancellationToken)
        {
            if (configuration.AllowRestart())
            {
                appLifetime.StopApplication();
                return Unit.Task;
            }
            else
            {
                logger.LogInformation("Skip setting openid connect values (AllowConfiguration = false)");
                throw new BadRequestException("Not possible, editing is not enabled");
            }
        }
    }
}