using System.Threading.Tasks;
using Glow.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Core.Actions
{
    [ApiController]
    // workaround: ApiController attributes need a RouteAttribute
    [Route("api/actions/__generic")]
    public class ActionController<Request, ResponsePayload> where Request : IRequest<ResponsePayload>
    {
        private readonly IMediator mediator;

        public ActionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Validatable]
        [HttpPost]
        public Task<ResponsePayload> Execute(Request request)
        {
            return mediator.Send(request);
        }
    }
}