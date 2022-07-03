using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.Core.Notifications;
using Glow.Glue.AspNetCore;
using Glow.Sample.TreasureIsland.Domain;
using Glow.Validation;
using Marten;
using MediatR;

namespace Glow.Sample.TreasureIsland.Api;

[Action(Route = "api/ti/move-player", AllowAnonymous = true)]
public record MoveOrAttack([NotEmpty] Guid Id, Direction Direction) : IRequest<MediatR.Unit>;

[Action(Route = "api/ti/restart-game", AllowAnonymous = true)]
public record RestartGame() : IRequest<MediatR.Unit>;

[Action(Route = "api/ti/create-player", AllowAnonymous = true)]
public record CreatePlayer(string Name, string Icon) : IRequest<CreatePlayerResult>;

public record CreatePlayerResult(Guid Id, Guid GameId);

[Action(Route = "api/ti/join", AllowAnonymous = true)]
public record Join(Guid PlayerId, Guid GameId) : IRequest<MediatR.Unit>;

[Action(Route = "api/ti/get-players", AllowAnonymous = true)]
public record GetPlayers() : IRequest<IEnumerable<Unit>>;

[Action(Route = "api/ti/get-games", AllowAnonymous = true)]
public record GetGames(GameStatus? Status) : IRequest<IReadOnlyList<Game>>;

public class Handler : IRequestHandler<MoveOrAttack>,
                       IRequestHandler<RestartGame>,
                       IRequestHandler<Join, MediatR.Unit>,
                       IRequestHandler<CreatePlayer, CreatePlayerResult>,
                       IRequestHandler<GetPlayers, IEnumerable<Unit>>,
                       IRequestHandler<GetGames, IReadOnlyList<Game>>
{
    private readonly IDocumentSession session;

    private readonly IMediator mediator;
    // private readonly IClientNotificationService notificationService;

    public Handler(IDocumentSession session, IMediator mediator, IClientNotificationService notificationService)
    {
        this.session = session;
        this.mediator = mediator;
        // this.notificationService = notificationService;
    }

    private void GenerateGameTickEvents(IReadOnlyList<Unit> players, Game current)
    {
        var alive = players.Where(v => v.IsAlive).ToList();
        if (alive.Count == 0)
        {
            session.Events.Append(current.Id, new GameDrawn());

            return;
        }

        if (alive.Count == 1)
        {
            if (alive.Count == players.Count)
            {
                // only one player
                // return;
            }
            else
            {
                session.Events.Append(current.Id, new GameEnded(alive.First().Id));
                return;
            }
        }

        foreach (var player in alive)
        {
            var regen = player.Items.Sum(v => v.Regeneration);
            session.Events.Append(player.Id, new UnitHealed(regen));
        }

        if (Utils.Chance(0.25))
        {
            var position = current.Field.GetRandomPosition();
            var item = Utils.GetRandomItem();
            var itemDropped = new ItemDropped(position, item);

            session.Events.Append(current.Id, itemDropped);
        }

        var nextActivePlayerIndex = (current.Tick + 1) % alive.Count;
        var nextActivePlayer = alive[nextActivePlayerIndex];

        // select player

        session.Events.Append(current.Id, new ActiveUnitChanged(nextActivePlayer.Id));
        session.Events.Append(current.Id, new GameTick());
    }

    public async Task<MediatR.Unit> Handle(MoveOrAttack request, CancellationToken cancellationToken)
    {
        var player = await session.LoadAsync<Unit>(request.Id);
        if (player == null) { throw new BadRequestException("Player not found"); }

        var game = await session.LoadAsync<Game>(player.GameId);
        if (game == null) { throw new BadRequestException("Game not found"); }

        var players = await session.LoadManyAsync<Unit>(game.Units);

        var newPosition = request.Direction.AddTo(player.Position);

        var field = game.Field.Fields.FirstOrDefault(v => v.Position == newPosition);
        if (field == null) { throw new BadRequestException("Field not found"); }

        if (!field.Tile.Walkable) { throw new BadRequestException("Field not walkable"); }

        var targetPlayer = players.FirstOrDefault(v => v.Position == newPosition);
        if (targetPlayer == null)
        {
            var e = new UnitMoved(request.Id, player.Position, newPosition);

            session.Events.Append(request.Id, e);

            var newField = game.Field.Fields.FirstOrDefault(v => v.Position == newPosition);
            if (newField != null && newField.Items.Count > 0)
            {
                var item = newField.Items.First();
                var e1 = new ItemPicked(item);
                session.Events.Append(request.Id, e1);
                var e2 = new ItemRemoved(item, newField.Position);
                session.Events.Append(game.Id, e2);
            }
        }
        else
        {
            var baseAttack = player.BaseAttack;
            var baseProtection = targetPlayer.BaseProtection;

            var damage = Math.Max(0, baseAttack - baseProtection);
            var e0 = new UnitAttacked(request.Id, targetPlayer.Id, damage);
            var e1 = new DamageTaken(request.Id, targetPlayer.Id, damage);
            session.Events.Append(request.Id, e0);
            session.Events.Append(targetPlayer.Id, e1);
            if (targetPlayer.Health - damage >= 0)
            {
                session.Events.Append(targetPlayer.Id, new UnitDied { });
            }
        }

        var nextPlayerId = player.Id;
        session.Events.Append(nextPlayerId, new UnitEnabledForWalk());

        GenerateGameTickEvents(players, game);

        await session.SaveChangesAsync();

        return MediatR.Unit.Value;
    }

    public async Task<MediatR.Unit> Handle(RestartGame request, CancellationToken cancellationToken)
    {
        var games = await session.Query<Game>().ToListAsync();
        var game = games.FirstOrDefault();
        if (game == null)
        {
            throw new BadRequestException("No active game");
        }

        var players = await session.Query<Unit>().ToListAsync();

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
            session.Events.Append(player.Id, new PlayerInitialized(player.Id, p));
        }

        session.Events.Append(game.Id, new GameRestarted(gameField));
        await session.SaveChangesAsync();
        return MediatR.Unit.Value;
    }

    public async Task<MediatR.Unit> Handle(Join request, CancellationToken cancellationToken)
    {
        // var position = Utils.RandomPosition();
        session.Events.Append(request.GameId, new PlayerJoined(request.PlayerId));
        // session.Events.Append(request.PlayerId, new PlayerInitialized(position));
        await session.SaveChangesAsync();

        await Task.Delay(1000);
        var game = await session.LoadAsync<Game>(request.GameId);
        if (game?.Units.Count == 2)
        {
            await mediator.Send(new RestartGame());
        }

        return MediatR.Unit.Value;
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

        var e1 = new UnitCreated(id, game.Id, request.Name, request.Icon);
        session.Events.StartStream(id, e1);
        var e2 = new PlayerJoined(id);
        session.Events.Append(game.Id, e2);
        await session.SaveChangesAsync();
        return new CreatePlayerResult(id, game.Id);
    }

    public async Task<IEnumerable<Unit>> Handle(GetPlayers request, CancellationToken cancellationToken)
    {
        var players = await session.Query<Unit>().ToListAsync();
        return players;
    }

    public async Task<IReadOnlyList<Game>> Handle(GetGames request, CancellationToken cancellationToken)
    {
        var games = await session
            .Query<Game>()
            .Where(v => request.Status == null || v.Status == GameStatus.Initializing)
            .ToListAsync();
        return games;
    }
}