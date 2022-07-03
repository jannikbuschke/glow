using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Notifications;
using Glow.Invoices.Api.Test;
using Glow.NotificationsCore;
using Glow.Sample;
using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glow.Core.Marten;

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
        var svc = scope.GetService<IClientNotificationService>();

        if (current != null)
        {
            var playerIds = current.Units;
            var players = session.LoadMany<Unit>(playerIds);
            var dict = players.ToDictionary(v => v.Id, v => v);
            var notification = new CurrentGameState(current.Id, dict, current);

            await svc.PublishNotification(notification);
        }

        foreach (var actions in streamActions)
        {
            // logger.LogInformation("publish stream actions");
            foreach (var e in actions.Events)
            {
                // logger.LogInformation("publish event {@event}", e);

                logger.LogInformation("Publish event " + e.Data.GetType().Name);
                if (e.Data is IClientNotification clientNotification)
                {
                    await svc.PublishNotification(clientNotification);
                }
            }
        }
    }
}