namespace TreasureIsland

open System
open System.Collections.Generic
open System.Linq
open Glow.Core.Actions
open Glow.Core.Notifications
open Glow.Glue.AspNetCore
open Marten
open MediatR
open Marten.Linq.MatchesSql
open Microsoft.Extensions.Logging

[<Action(Route = "api/move-player", AllowAnonymous = true)>]
type MoveOrAttack =
  { Id: Guid
    Direction: Direction }
  interface IRequest<MediatR.Unit>

[<Action(Route = "api/start-game", AllowAnonymous = true)>]
type StartGameRequest =
  { Data: StartGame }
  interface IRequest<MediatR.Unit>

[<Action(Route = "api/restart-game", AllowAnonymous = true)>]
type RestartGame =
  { Data: unit }
  interface IRequest<MediatR.Unit>

type CreatePlayerResult = { PlayerUnitId: PlayerUnitId; GameId: GameId }

[<Action(Route = "api/ti/create-player", AllowAnonymous = true)>]
type CreatePlayer =
  { Name: string
    Icon: string
    GameId: Guid }
  interface IRequest<CreatePlayerResult>

// [Action(Route = "api/ti/join", AllowAnonymous = true)]
// type Join(Guid PlayerId, Guid GameId) : IRequest<MediatR.Unit>;

[<Action(Route = "api/ti/get-players", AllowAnonymous = true)>]
type GetPlayers =
  { Data: unit }
  interface IRequest<IReadOnlyList<TreasureIsland.PlayerUnit>>

[<Action(Route = "api/ti/get-games", AllowAnonymous = true)>]
type GetGames =
  { Status: GameStatus }
  interface IRequest<IReadOnlyList<Game>>

[<Action(Route = "api/ti/get-game-sate", AllowAnonymous = true)>]
type GetGameState =
  { GameId: GameId }
  interface IRequest<CurrentGameState>

module Helper =
  let GenerateGameTickEvents (session: IDocumentSession) (players: IReadOnlyList<TreasureIsland.PlayerUnit>) (current: Game) =
    task {
      let alive =
        players.Where(fun v -> v.IsAlive).ToList()

      if (alive.Count = 0) then
        session.AppendGameEvent(current.Key(), GameEvent.GameDrawn { Data = () }) |> ignore
        return ()

      if (alive.Count = 1) then

        if (alive.Count = players.Count) then
          return ()
        else

          session.AppendGameEvent(current.Key(), GameEvent.GameEnded { GameEnded.Winner = alive.First().Id }) |> ignore
          return ()

      // alive
      // |> Seq.iter (fun player ->
      //   let regen =
      //     player.Items.Sum(fun v -> v.Regeneration)
      //
      //   let e: UnitHealed = { Health = regen }
      //   session.Events.Append(player.Id, e)
      //   ())

      if (Utils.Chance(0.25)) then

        let position =
          current.Field.GetRandomPosition()

        let item = Utils.GetRandomItem()
        session.AppendGameEvent(current.Key(), GameEvent.ItemDropped { Position = position; Item = item }) |> ignore

      let nextActivePlayerIndex =
        (current.Tick + 1) % alive.Count

      let nextActivePlayer =
        alive[nextActivePlayerIndex]

      // select player

      session.AppendGameEvent(current.Key(), GameEvent.ActiveUnitChanged { UnitId = nextActivePlayer.Id }) |> ignore
      session.AppendGameEvent(current.Key(), GameEvent.GameTick { Data = () }) |> ignore
    }

type Handler(session: IDocumentSession, mediator: IMediator, notificationService: IClientNotificationService, logger: ILogger<MoveOrAttack>) =
  interface IRequestHandler<StartGameRequest> with
    member this.Handle(request, cancellationToken) =
      task {
        let! game = session.GetGameAsync(request.Data.GameId)

        match game.Status with
        | Initializing -> () // do nothing
        | _ -> raise (BadRequestException("Game is not in initializing state"))

        let! gameState = session.GetCurrentGameStateAsync(request.Data.GameId)

        let playerUnits = gameState.Units.Values


        // let gameField = GameFieldGenerator.OrientedRectangle(18, 10);


        let blockedPositions = List<Position>()

        playerUnits
        |> Seq.iter (fun player ->
          let p = game.Field.GetRandomPosition()
          // TODO: Check if blocked
          if blockedPositions.Contains(p) then
            raise (BadRequestException("Position blocked"))

          blockedPositions.Add(p)

          // let e: PlayerUnitEvent =
          //   PlayerUnitEvent.PlayerUnitInitialized { Position = p }
          //
          // session.AppendPlayerUnitEvent(player.Key, e)
          // |> ignore
          // session.Events.Append(player.Id, e)
          ())

        session.AppendGameEvent(request.Data.GameId, GameEvent.GameStarted)
        |> ignore

        do! session.SaveChangesAsync()
        return Unit.Value
      }

  interface IRequestHandler<MoveOrAttack> with
    member this.Handle(request, cancellationToken) =
      task {
        let! player = session.LoadAsync<TreasureIsland.PlayerUnit>(request.Id)

        if box player = null then
          raise (BadRequestException("Player not found"))

        let! game = session.GetGameAsync(player.GameId)

        if box game = null then
          raise (BadRequestException("Game not found"))

        match game.ActiveUnit with
        | Some id ->
          if id <> player.Id then
            raise (BadRequestException("Player is not active"))
        | None -> raise (BadRequestException("Player is not active (no one is)"))

        let! players = session.LoadManyAsync<TreasureIsland.PlayerUnit>(game.PlayerUnitIds |> List.map(fun (PlayerUnitId id)->id))

        let newPosition =
          request.Direction.AddTo(player.Position)

        logger.LogInformation("current player position: {@position}", player.Position)
        logger.LogInformation("direction              : {@position}", request.Direction)
        logger.LogInformation("new position           : {@position}", newPosition)

        let field =
          game.Field.Fields.FirstOrDefault(fun v -> v.Position = newPosition)

        if (box field = null) then
          raise (BadRequestException("Field not found"))

        logger.LogInformation("request {@request}", request)
        logger.LogInformation("tile {@tile}", field.Tile)
        logger.LogInformation("walkable {@walkable}", field.Tile.Walkable)
        logger.LogInformation("not walkable {@walkable}", not field.Tile.Walkable)

        if (not field.Tile.Walkable) then
          raise (BadRequestException("Field not walkable"))

        let targetPlayer =
          players.FirstOrDefault(fun v -> v.Position = newPosition)

        if (box targetPlayer = null) then
          let e: UnitMoved =
            { UnitId = request.Id
              OldPosition = player.Position
              Position = newPosition }

          session.Events.Append(request.Id, e) |> ignore

          let newField =
            game.Field.Fields.FirstOrDefault(fun v -> v.Position = newPosition)

          if (box newField <> null && newField.Items.Length > 0) then
            let item = newField.Items.First()
            let e1: ItemPicked = { Item = item }
            session.Events.Append(request.Id, e1) |> ignore

            let e2: ItemRemoved =
              { Item = item
                Position = newField.Position }

            session.Events.Append(game.Id, e2) |> ignore

        else
          let baseAttack = player.BaseAttack

          let baseProtection =
            targetPlayer.BaseProtection

          let damage =
            Math.Max(0, baseAttack - baseProtection)

          let e0: UnitAttacked =
            { AttackingUnit = request.Id
              TargetUnit = targetPlayer.Id
              Damage = damage } // new UnitAttacked(request.Id, targetPlayer.Id, damage)

          let e1: DamageTaken =
            { AttackingUnit = request.Id
              TargetUnit = targetPlayer.Id
              Damage = damage } // new DamageTaken(request.Id, targetPlayer.Id, damage)

          session.Events.Append(request.Id, e0) |> ignore

          session.Events.Append(targetPlayer.Id, e1)
          |> ignore

          if (targetPlayer.Health - damage >= 0) then
            let e: UnitDied = { Data = () }

            session.Events.Append(targetPlayer.Id, e)
            |> ignore

        let nextPlayerId = player.Id
        let e: UnitEnabledForWalk = { Data = () }
        session.Events.Append(nextPlayerId, e) |> ignore

        do! Helper.GenerateGameTickEvents session players game

        do! session.SaveChangesAsync()

        return MediatR.Unit.Value
      }

  interface IRequestHandler<RestartGame> with
    member this.Handle(request, cancellationToken) =
      task {
        let! games = session.Query<Game>().ToListAsync()
        let game = games.FirstOrDefault()

        if (box game = null) then
          raise (BadRequestException("No active game"))

        let! players =
          session
            .Query<TreasureIsland.PlayerUnit>()
            .ToListAsync()

        // let gameField = GameFieldGenerator.OrientedRectangle(18, 10);
        let gameField = GameFieldGenerator.hexagon 5

        let positions =
          gameField.Fields.Select(fun v -> v.Position)

        let blockedPositions = new List<Position>()

        players
        |> Seq.iter (fun player ->
          let p = gameField.GetRandomPosition()
          // TODO: Check if blocked
          if blockedPositions.Contains(p) then
            raise (BadRequestException("Position blocked"))

          blockedPositions.Add(p)

          // session.Events.Append(player.Id, e)
          ())

        session.AppendGameEvent(game.Key(), GameEvent.GameRestarted { GameField = gameField }) |> ignore
        do! session.SaveChangesAsync()
        return MediatR.Unit.Value
      }

  interface IRequestHandler<CreatePlayer, CreatePlayerResult> with
    member this.Handle(request, cancellationToken) =
      task {
        let! game = session.GetGameAsync(request.GameId |> GameId.create)

        let id = System.Guid.NewGuid() |> PlayerUnitId.create

        session.AppendGameEvent(game.Key(), GameEvent.PlayerJoined  { PlayerId = id }) |> ignore
        session.AppendGameEvent(game.Key(), GameEvent.PlayerUnitCreated { PlayerUnitId = id
                                                                          Name = request.Name
                                                                          Icon = request.Icon
                                                                          Position = game.Field.GetRandomPosition() }) |> ignore
        // session.Events.Append(game.Id, e2)
        do! session.SaveChangesAsync()

        let result: CreatePlayerResult =
          { PlayerUnitId = id; GameId = game.Key() }

        return result
      }

  interface IRequestHandler<GetPlayers, IReadOnlyList<TreasureIsland.PlayerUnit>> with
    member this.Handle(request, cancellationToken) =
      task {
        let! players =
          session
            .Query<TreasureIsland.PlayerUnit>()
            .ToListAsync()

        return players
      }

  interface IRequestHandler<GetGames, IReadOnlyList<Game>> with
    member this.Handle(request, cancellationToken) =
      task {
        let! games =
          session
            .Query<Game>()
            .Where(fun v ->
              v.MatchesSql("data -> 'Status' ->> 'Case' = ?", "Initializing")
              // box request.Status = null
              // || v.Status = GameStatus.Initializing
              )
            .ToListAsync()

        return games
      }

  interface IRequestHandler<GetGameState, CurrentGameState> with
    member this.Handle(request, cancellationToken) =
      task {
        let! state = session.GetCurrentGameStateAsync(request.GameId)
        return state
      }
// interface IRequestHandler<Join, MediatR.Unit> with

// public async Task<MediatR.Unit> Handle(Join request, CancellationToken cancellationToken)
// {
//     session.Events.Append(request.GameId, new PlayerJoined(request.PlayerId));
//     await session.SaveChangesAsync();
//
//     await Task.Delay(1000);
//     var game = await session.LoadAsync<Game>(request.GameId);
//     if (game?.Units.Count == 2)
//     {
//         await mediator.Send(new RestartGame());
//     }
//
//     return MediatR.Unit.Value;
// }
