using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.TypeScript;
using Glow.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Sample.Forms
{
    [GenerateTsInterface]
    [Action(Route="api/form/create-user", AllowAnonymous = true)]
    public class CreateUser : IRequest<Unit>
    {
        [Required, MinLength(3)]
        public string DisplayName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    public class CreateUserHandler : IRequestHandler<CreateUser, Unit>
    {
        private readonly DataContext ctx;

        public CreateUserHandler(DataContext ctx)
        {
            this.ctx = ctx;
        }

        public Task<Unit> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            // left out
            return Unit.Task;
        }
    }
}