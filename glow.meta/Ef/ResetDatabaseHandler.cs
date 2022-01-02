using System;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using Glow.TypeScript;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Glow.Core.EfCore;


public abstract class AbstractResetDatabaseHandler<T> where T : DbContext
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

[Action(Policy = "Admin", Route = "api/glow/db/reset-database")]
[GenerateTsInterface]
public class ResetDatabase : IRequest
{
    public bool DeleteDatabase { get; set; }
    public bool IKnowWhatIAmDoing { get; set; }
    public Func<Task> AfterCreated { get; set; }
}

public static class DbContextExtensions
{
    public static async Task ResetDatabase(this DbContext v, ResetDatabase request)
    {
        try
        {
            if (!request.IKnowWhatIAmDoing)
            {
                throw new BadRequestException("");
            }

            if (request.DeleteDatabase)
            {
                await v.Database.EnsureDeletedAsync();
            }

            await v.Database.MigrateAsync();
            if (request.AfterCreated != null)
            {
                await request.AfterCreated.Invoke();
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}