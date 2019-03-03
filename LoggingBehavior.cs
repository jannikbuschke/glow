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
            logger.LogDebug($"Handling {typeof(TRequest).Name}");
            logger.LogDebug("{@request}",request);
            TResponse response = await next();
            logger.LogDebug($"Handled {typeof(TResponse).Name}");
            logger.LogDebug("{@response}", response);
            return response;
        }
    }
}
