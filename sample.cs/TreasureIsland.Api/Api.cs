using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.Core.Notifications;
using Glow.Glue.AspNetCore;
using Glow.Sample.TreasureIsland.Domain;
using Glow.Sample.TreasureIsland.Projections;
using Glow.Validation;
using Marten;
using Marten.Linq;
using MediatR;

namespace Glow.Sample.TreasureIsland.Api;

[Action(Route = "api/ti/move-player", AllowAnonymous = true)]
public record MovePlayer([NotEmpty] Guid Id, Direction Direction) : IRequest<Unit>;

[Action(Route = "api/ti/restart-game", AllowAnonymous = true)]
public record RestartGame() : IRequest<Unit>;

[Action(Route = "api/ti/create-player", AllowAnonymous = true)]
public record CreatePlayer(string Name, string Icon) : IRequest<CreatePlayerResult>;

public record CreatePlayerResult(Guid Id, Guid GameId);

[Action(Route = "api/ti/join", AllowAnonymous = true)]
public record Join(Guid PlayerId, Guid GameId) : IRequest<Unit>;

[Action(Route = "api/ti/get-players", AllowAnonymous = true)]
public record GetPlayers() : IRequest<IEnumerable<Player>>;

public class Handler : IRequestHandler<MovePlayer>,
                       IRequestHandler<RestartGame>,
                       IRequestHandler<Join, Unit>,
                       IRequestHandler<CreatePlayer, CreatePlayerResult>,
                       IRequestHandler<GetPlayers, IEnumerable<Player>>
{
    private readonly IDocumentSession session;
    // private readonly IClientNotificationService notificationService;

    public Handler(IDocumentSession session, IClientNotificationService notificationService)
    {
        this.session = session;
        // this.notificationService = notificationService;
    }

    public async Task<Unit> Handle(MovePlayer request, CancellationToken cancellationToken)
    {
        var player = await session.LoadAsync<Player>(request.Id);
        var newPosition = request.Direction.AddTo(player.Position);
        session.Events.Append(request.Id, new PlayerMoved(request.Id, newPosition));

        var nextPlayerId = player.Id;
        session.Events.Append(nextPlayerId, new PlayerEnabledForWalk());
        await session.SaveChangesAsync();

        return Unit.Value;
    }

    public async Task<Unit> Handle(RestartGame request, CancellationToken cancellationToken)
    {
        var games = await session.Query<Game>().ToListAsync();
        var game = games.FirstOrDefault();
        if (game == null)
        {
            throw new BadRequestException("No active game");
        }

        var players = await session.Query<Player>().ToListAsync();

        // var gameField = GameFieldGenerator.OrientedRectangle(18, 10);
        var gameField = GameFieldGenerator.Hexagon(5);
        var positions = gameField.Fields.Select(v => v.Position);

        var blockedPositions = new List<Position>();

        foreach (var player in players)
        {
            Position p;
            do
            {
                p = gameField.GetRandomPosition();
            } while (blockedPositions.Contains(p));

            blockedPositions.Add(p);
            session.Events.Append(player.Id, new PlayerInitialized(p));
        }

        session.Events.Append(game.Id, new GameRestarted(gameField));
        await session.SaveChangesAsync();
        return Unit.Value;
    }

    public async Task<Unit> Handle(Join request, CancellationToken cancellationToken)
    {
        // var position = Utils.RandomPosition();
        session.Events.Append(request.GameId, new PlayerJoined(request.PlayerId));
        // session.Events.Append(request.PlayerId, new PlayerInitialized(position));
        await session.SaveChangesAsync();
        return Unit.Value;
    }

    public async Task<CreatePlayerResult> Handle(CreatePlayer request, CancellationToken cancellationToken)
    {
        var games = await session.Query<Game>().ToListAsync();
        var game = games.FirstOrDefault(v => v.Status == GameStatus.Initializing || v.Status == GameStatus.Running);
        if (game == null)
        {
            throw new BadRequestException("No game found");
        }

        var id = Guid.NewGuid();

        var position = game.Field.GetRandomPosition();

        var e1 = new PlayerCreated(id, request.Name, request.Icon, position);
        session.Events.StartStream(id, e1);
        var e2 = new PlayerJoined(id);
        session.Events.Append(game.Id, e2);
        await session.SaveChangesAsync();
        // await notificationService.PublishNotification(e1);
        // await notificationService.PublishNotification(e2);
        return new CreatePlayerResult(id, game.Id);
    }

    public async Task<IEnumerable<Player>> Handle(GetPlayers request, CancellationToken cancellationToken)
    {
        var players = await session.Query<Player>().ToListAsync();
        return players;
    }
}