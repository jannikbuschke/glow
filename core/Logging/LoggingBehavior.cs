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
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Stopwatch stopWatch = new();

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            httpContextAccessor.HttpContext?.AddRequestToItems(request);

            stopWatch.Start();
            logger.LogInformation($"[Handling Request]: {typeof(TRequest).Name}");
            logger.LogInformation("Parameters {@values}", request);
            TResponse response = await next();
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            var requestName = typeof(TRequest).Name;
            var logLevel = requestName.StartsWith("Get") ? LogLevel.Debug : LogLevel.Information;
            logger.Log(logLevel, "[Handled Request]: {requestName} ({timeMs} ms)", requestName, (long)ts.TotalMilliseconds);
            logger.LogTrace("Response payload {@values}", response);
            return response;
        }
    }
}
