using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Glow.Glue.AspNetCore
{
    public class AddRequestAndIntentToHttpContextItems<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<AddRequestAndIntentToHttpContextItems<TRequest, TResponse>> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AddRequestAndIntentToHttpContextItems(ILogger<AddRequestAndIntentToHttpContextItems<TRequest, TResponse>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            httpContextAccessor.HttpContext?.AddRequestToItems(request);
            httpContextAccessor.HttpContext?.AddUserIntentToItems(request.GetType().Name);
            return next();
        }
    }
}
