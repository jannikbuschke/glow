using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Glow.Core.EfCore
{
    public abstract class AbstractResetDatabaseHandler<T> where T: DbContext
    {
        private readonly T ctx;

        public AbstractResetDatabaseHandler(T ctx)
        {
            this.ctx = ctx;
        }

        public async Task<Unit> Handle(ResetDatabase request, CancellationToken cancellationToken)
        {
            await ctx.ResetDatabase(request);
            await SeedDatabase(ctx);
            return Unit.Value;
        }

        protected abstract Task SeedDatabase(T ctx);
    }
}