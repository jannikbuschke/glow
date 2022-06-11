using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Notifications;
using Glow.Invoices.Api.Test;
using Glow.Sample.TreasureIsland.Projections;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glow.Sample.TreasureIsland.Api;

public class MartenSubscription : IProjection
{
    private readonly IEnumerable<IMartenEventsConsumer> consumers;
    private readonly ILogger<MartenSubscription> logger;

    public MartenSubscription(
        IEnumerable<IMartenEventsConsumer> consumers,
        ILogger<MartenSubscription> logger
    )
    {
        this.consumers = consumers;
        this.logger = logger;
    }

    public void Apply(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams
    )
    {
        throw new NotImplementedException("Subscriptions should work only in the async scope");
    }

    public async Task ApplyAsync(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams,
        CancellationToken ct
    )
    {
        // logger.LogInformation("apply events in custom projection async");
        try
        {
            foreach (var consumer in consumers)
            {
                await consumer.ConsumeAsync(operations, streams, ct);
            }
        }
        catch (Exception exc)
        {
            logger.LogError("Error while processing Marten Subscription: {ExceptionMessage}", exc.Message);
            throw;
        }
    }
}

public class MartenSignalrConsumer : IMartenEventsConsumer
{
    private readonly IServiceProvider sp;

    public MartenSignalrConsumer(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public async Task ConsumeAsync(IDocumentOperations documentOperations, IReadOnlyList<StreamAction> streamActions, CancellationToken ct)
    {
        using var scope = sp.CreateScope();
        var logger = scope.GetService<ILogger<MartenSignalrConsumer>>();

        var session = scope.GetService<IDocumentSession>();

        var games = await session.Query<Game>().ToListAsync();
        var current = games.FirstOrDefault();
        if (current != null)
        {
            var playerIds = current.Players;
            var players = session.LoadMany<Player>(playerIds);
            var notification = new CurrentGameState(current.Id, current.Items, players, current);

            var svc = scope.GetService<IClientNotificationService>();
            await svc.PublishNotification(notification);
        }

        // foreach (var actions in streamActions)
        // {
        //     logger.LogInformation("publish stream actions");
        //     foreach (var e in actions.Events)
        //     {
        //         // logger.LogInformation("publish event {@event}", e);
        //
        //         if (e.Data is IClientNotification clientNotification)
        //         {
        //             await svc.PublishNotification(clientNotification);
        //         }
        //     }
        // }
    }
}

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(
        IDocumentOperations documentOperations,
        IReadOnlyList<StreamAction> streamActions,
        CancellationToken ct
    );
}