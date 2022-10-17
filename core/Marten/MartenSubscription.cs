using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.Extensions.Logging;

namespace Glow.Core.MartenSubscriptions;

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
        logger.LogInformation("apply events in custom projection async");
        try
        {
            foreach (IMartenEventsConsumer consumer in consumers)
            {
                logger.LogInformation("invoke ConsumeAsync for consumer {consumer}", consumer);

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

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(
        IDocumentOperations documentOperations,
        IReadOnlyList<StreamAction> streamActions,
        CancellationToken ct
    );
}