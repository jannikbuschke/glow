using System;
using System.Threading.Tasks;
using System.Threading;
using Glow.Sample.TreasureIsland.Domain;
using Marten;
using Microsoft.Extensions.Hosting;

namespace Glow.Sample.TreasureIsland.Api;

public class DungeonWorker : BackgroundService
{
    private readonly IDocumentStore db;
    private readonly IServiceProvider provider;

    public DungeonWorker(IDocumentStore db, IServiceProvider provider)
    {
        this.db = db;
        this.provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using (var session = db.OpenSession())
        {
            var games = await session.Query<Game>().ToListAsync();
            foreach (var game in games)
            {
                if (game.Status == GameStatus.Running || game.Status == GameStatus.Initializing)
                {
                    session.Events.Append(game.Id, new GameEnded());
                }
            }

            var gameField = GameFieldGenerator.Hexagon(15);

            var id = Guid.NewGuid();
            var gameCreated = new GameCreated(id, gameField);
            session.Events.StartStream(id, gameCreated);
            await session.SaveChangesAsync(stoppingToken);
        }

        // return;
        //
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     using var scope = provider.CreateScope();
        //     var logger = scope.ServiceProvider.GetService<ILogger<DungeonWorker>>();
        //     var notifications = scope.ServiceProvider.GetRequiredService<IClientNotificationService>();
        //     logger.LogInformation("Tick");
        //     await using var session = db.OpenSession();
        //
        //     var games = await session.Query<Game>().ToListAsync();
        //     var current = games.FirstOrDefault();
        //     if (current != null)
        //     {
        //         var playerIds = current.Players;
        //         var players = session.LoadMany<Player>(playerIds);
        //         // var notification = new CurrentGameState(current.Id, current.Items, players, current);
        //         // await notifications.PublishNotification(notification);
        //
        //         if (Utils.Chance(0.5))
        //         {
        //             var position = current.Field.GetRandomPosition();
        //             var item = Utils.GetRandomItem();
        //             var itemDropped = new ItemDropped(position, item);
        //
        //             session.Events.Append(current.Id, itemDropped);
        //         }
        //
        //         foreach (var player in players)
        //         {
        //             var item = current.Items.FirstOrDefault(v => v.Position.Equals(player.Position));
        //             if (item != null)
        //             {
        //                 var e1 = new PlayerPickedItem(item.Position, item.Item, player.Id);
        //                 var e2 = new ItemPicked(item.Item);
        //                 session.Events.Append(player.Id, e2);
        //                 session.Events.Append(current.Id, e1);
        //             }
        //         }
        //     }
        //
        //
        //     await session.SaveChangesAsync();
        //
        //     await Task.Delay(5000);

        // var current = await session.Events.AggregateStreamAsync<Player>(id, token: stoppingToken);
        //
        // AnsiConsole.MarkupLine($"[red]{current.Name}: {current.Health}pts[/] | [yellow]Treasure: {current.Treasure} coins[/]");
        //
        // if (current.Status is AliveStatus.Dead)
        // {
        //     var answer = AnsiConsole.Ask<char>(":skull: Looks like you're dead. Revive (y,n)?");
        //     if (answer is 'y')
        //     {
        //         session.Events.Append(id, new Revive());
        //     }
        // }
        // else
        // {
        //     var command = AnsiConsole.Ask<char>("What would you like to do (Fight (f), Heal (h))?");
        //
        //     switch (command)
        //     {
        //         case 'f':
        //             var hit = new Hit {Damage = RandomNumberGenerator.GetInt32(1, 15)};
        //             session.Events.Append(id, hit);
        //             AnsiConsole.MarkupLine($"\tOuch! [red]{hit.Damage}pts[/] damage.");
        //
        //             // only find treasure if you're fighting
        //             if (RandomNumberGenerator.GetInt32(1, 4) is 2)
        //             {
        //                 // found treasure!
        //                 var amount = RandomNumberGenerator.GetInt32(1, 11);
        //                 AnsiConsole.MarkupLine($"\t[yellow]YOU FOUND {amount} TREASURE![/]");
        //                 session.Events.Append(id, new Treasure { Amount = amount });
        //             }
        //
        //             break;
        //         case 'h':
        //             session.Events.Append(id, new Heal {Life = 15});
        //             AnsiConsole.MarkupLine("\tHealed! [green]15pts[/]");
        //             break;
        //     }
        //
        //
        // }
        // save round
        // await session.SaveChangesAsync(stoppingToken);
        // }
    }
}