using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JannikB.Glue.AspNetCore
{

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            logger.LogDebug($"[Handling Request]: {typeof(TRequest).Name}");
            logger.LogTrace("Parameters {@values}", request);
            TResponse response = await next();
            logger.LogInformation($"[Handled Request]: {typeof(TRequest).Name}");
            logger.LogTrace("Response payload {@values}", response);
            return response;
        }
    }
}
