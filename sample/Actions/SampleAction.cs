using System;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.TypeScript;
using MediatR;

namespace Glow.Sample.Actions
{
    [GenerateTsInterface]
    [Action(Policy = Policies.Privileged, Route = "api/actions/sample")]
    public class SampleAction : IRequest
    {
        public string Foo { get; set; }
    }

    [Action(Policy = Policies.Privileged, Route = "api/actions/sample-2")]
    public class SampleAction2 : IRequest<Response>
    {
        public string Message { get; set; }
    }

    public class Response
    {
        public string Value { get; set; }
    }

    public class SampleActionHandler
        : IRequestHandler<SampleAction>
        , IRequestHandler<SampleAction2, Response>
    {
        public Task<Unit> Handle(SampleAction request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Response> Handle(SampleAction2 request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response { Value = "Hello World " + request.Message });
        }
    }
}