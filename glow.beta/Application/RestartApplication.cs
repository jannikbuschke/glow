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

namespace Glow.Core.Application
{
    [Action(Policy = DefaultPolicies.Admin, Route = "api/glow/restart-application")]
    public class RestartApplication : IRequest
    {
    }

    public class RestartApplicationHandler : IRequestHandler<RestartApplication, Unit>
    {
        private readonly IConfiguration configuration;
        private readonly IApplicationLifetime appLifetime;

        public RestartApplicationHandler(IConfiguration configuration, IApplicationLifetime appLifetime)
        {
            this.configuration = configuration;
            this.appLifetime = appLifetime;
        }

        public Task<Unit> Handle(RestartApplication request, CancellationToken cancellationToken)
        {
            appLifetime.StopApplication();
            return Unit.Task;
        }
    }
}