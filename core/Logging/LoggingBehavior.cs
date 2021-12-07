using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Glow.Glue.AspNetCore
{
    public static class GlowLoggingConstants
    {
        public const string HttpContextRequestItemName = "MediatR.Request";

    }
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Stopwatch stopWatch = new Stopwatch();

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                httpContextAccessor.HttpContext.Items.TryAdd(GlowLoggingConstants.HttpContextRequestItemName, request);
            }
            stopWatch.Start();
            logger.LogDebug($"[Handling Request]: {typeof(TRequest).Name}");
            logger.LogTrace("Parameters {@values}", request);
            TResponse response = await next();
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            logger.LogInformation("[Handled Request]: {requestName} ({timeMs} ms)", typeof(TRequest).Name,
                (long)ts.TotalMilliseconds);
            logger.LogTrace("Response payload {@values}", response);
            return response;
        }
    }
}