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
open FsToolkit.ErrorHandling
open Glow.SampleData

[<Action(Route = "api/move-player", AllowAnonymous = true)>]
type MoveOrAttack =
  { GameId: GameId
    UnitId: PlayerUnitId
    Direction: Direction }
  interface IRequest<Result<MediatR.Unit, Error>>

[<Action(Route = "api/start-game", AllowAnonymous = true)>]
type StartGameRequest =
  { Data: StartGame }
  interface IRequest<Result<MediatR.Unit, Error>>

[<Action(Route = "api/restart-game", AllowAnonymous = true)>]
type RestartGame =
  { Data: unit }
  interface IRequest<MediatR.Unit>

type CreatePlayerResult =
  { PlayerId: PlayerId
    PlayerUnitId: PlayerUnitId
    GameId: GameId }

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
  interface IRequest<Result<Game, Error>>

type GameSampleData() =
  interface SampleData with
    member this.Create() =
      let fields = GameFieldGenerator.hexagon 3
      let units = [{ PlayerUnit.Id = System.Guid.Parse("66b80427-03d6-4c3a-b850-771dc4c321ba") |> PlayerUnitId.create
                     PlayerId = System.Guid.Parse("66b80427-03d6-4c3a-b850-771dc4c321ba") |> PlayerId
                     Name = "Unit 1"
                     Icon = "😑"
                     Position = fields.GetRandomPosition()
                     AssetId = "hexe.png"
                     Items = []
                     Health = Health.full
                     IsAlive = true };
                   { PlayerUnit.Id = System.Guid.Parse("de0e29d7-6365-46f8-babe-27e7da5cb9be") |> PlayerUnitId.create
                     PlayerId = System.Guid.Parse("3b301796-3eb2-4cc6-a4ce-f4a81e8bff6c") |> PlayerId
                     Name = "Unit 2"
                     Icon = "🤪"
                     Position = fields.GetRandomPosition()
                     AssetId = "warrior-highlight.png"
                     Items = []
                     Health = Health.full
                     IsAlive = true }
                   { PlayerUnit.Id = System.Guid.Parse("4b1b6da5-dcd3-4a28-b45a-ef084b24e3ee") |> PlayerUnitId.create
                     PlayerId = System.Guid.Parse("acf1f866-ff5e-46c6-b8fc-cbb5f7cc172b") |> PlayerId
                     Name = "Unit 2"
                     Icon = "🤪"
                     Position = fields.GetRandomPosition()
                     AssetId = "young-wizard.png"
                     Items = []
                     Health = Health.full
                     IsAlive = true }
                   { PlayerUnit.Id = System.Guid.Parse("3dcf1748-3f56-4862-ac75-b73d1764ecf7") |> PlayerUnitId.create
                     PlayerId = System.Guid.Parse("3755e222-011f-4750-b1bd-182676ff7cca") |> PlayerId
                     Name = "Unit 2"
                     Icon = "🤪"
                     Position = fields.GetRandomPosition()
                     AssetId = "ghost.png"
                     Items = []
                     Health = Health.full
                     IsAlive = true }
                     ]
      [("sample",
       {| InitializedGame =
          { Id = System.Guid.Parse("5a51d49e-95d7-4fa4-95f1-4499c1002e80")
            Version = 0
            Tick = 0
            Status = GameStatus.Initializing
            Items = []
            Field = GameFieldGenerator.hexagon 3
            Mode = GameMode.RoundBased
            PlayerUnits = units |> List.map(fun v -> v.Id, v) |> Map.ofSeq
            Players = Map.empty
            ActiveUnit = Some(units |> List.head |> fun v -> v.Id) } |})]

module Helper =
  let GenerateGameTickEvents (session: IDocumentSession) (current: Game) =
    task {

      let alive =
        current.PlayerUnits
        |> Map.values
        |> Seq.filter (fun x -> x.IsAlive)
        |> Seq.distinctBy (fun v -> v.PlayerId)
        |> Seq.toList

      match alive with
      | [] ->
        session.AppendGameEvent(current.Key(), GameEvent.GameDrawn { Data = () })
        |> ignore

        ()
      | [ x ] ->
        session.AppendGameEvent(current.Key(), GameEvent.GameEnded { GameEnded.Winner = alive.First().PlayerId })
        |> ignore

        ()
      | _ ->

        if Utils.Chance(0.25) then
          session.AppendGameEvent(
            current.Key(),
            GameEvent.ItemDropped
              { Position = current.Field.GetRandomPosition()
                Item = Utils.GetRandomItem() }
          )
          |> ignore

        let nextActivePlayerIndex =
          (current.Tick + 1) % alive.Length

        let nextActivePlayer =
          alive[nextActivePlayerIndex]

        session.AppendGameEvent(current.Key(), GameEvent.ActiveUnitChanged { UnitId = nextActivePlayer.Id })
        |> ignore

        session.AppendGameEvent(current.Key(), GameEvent.GameTick { Data = () })
        |> ignore

        ()

    }

type Handler(session: IDocumentSession, mediator: IMediator, notificationService: IClientNotificationService, logger: ILogger<MoveOrAttack>) =

  let validateGameState (game: Game) =
    match game.Status with
    | Initializing -> Result.Ok()
    | _ -> Result.Error "Game is not in initializing state"

  let initialize playerUnits game =
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

  interface IRequestHandler<StartGameRequest, Result<MediatR.Unit, Error>> with
    member this.Handle(request, _) =
      taskResult {
        let! game = session.GetGameAsync(request.Data.GameId)

        do!
          game
          |> validateGameState
          |> Result.mapError Error.InvalidState
        // let! gameState = session.GetCurrentGameStateAsync(request.Data.GameId)
        // let playerUnits = gameState.Units.Values
        // do initialize playerUnits game

        session.AppendGameEvent(request.Data.GameId, GameEvent.GameStarted)
        |> ignore

        do! session.SaveChangesAsync()
        return Unit.Value
      }

  interface IRequestHandler<MoveOrAttack, Result<MediatR.Unit, Error>> with
    member this.Handle(request, cancellationToken) =
      taskResult {
        let! game = session.GetGame(request.GameId)
        let! playerUnit =
          game.PlayerUnits
          |> Map.values
          |> Seq.toList
          |> List.tryFind (fun v -> v.Id = request.UnitId)
          |> Result.requireSome (Error.NotFound "Player not found")

        do!
          (Some request.UnitId) = game.ActiveUnit
          |> Result.requireTrue (Error.InvalidState "Not active player")

        let newPosition =
          request.Direction.AddTo(playerUnit.Position)

        logger.LogInformation("current player position: {@position}", playerUnit.Position)
        logger.LogInformation("direction              : {@position}", request.Direction)
        logger.LogInformation("new position           : {@position}", newPosition)

        let! field =
          game.Field.Fields
          |> List.tryFind (fun v -> v.Position = newPosition)
          |> Result.requireSome (Error.InvalidRequest "Position not found")

        do!
          field.Tile.Walkable
          |> Result.requireTrue (Error.InvalidRequest "Tile is not walkable")

        logger.LogInformation("request {@request}", request)
        logger.LogInformation("tile {@tile}", field.Tile)
        logger.LogInformation("walkable {@walkable}", field.Tile.Walkable)
        logger.LogInformation("not walkable {@walkable}", not field.Tile.Walkable)

        match
          game.PlayerUnits
          |> Map.values
          |> Seq.toList
          |> List.tryFind (fun v -> v.Position = newPosition)
        with
        | None ->
          let e: UnitMoved =
            { UnitId = playerUnit.Id
              OldPosition = playerUnit.Position
              Position = newPosition }

          session.AppendGameEvent(game.Key(), GameEvent.UnitMoved e)
          |> ignore

          if field.Items.Length > 0 then
            let item = field.Items.Head

            session.AppendGameEvent(
              game.Key(),
              GameEvent.ItemPicked
                { Item = item
                  Position = field.Position }
            )
            |> ignore

        | Some targetUnit -> ()
        // let baseAttack = playerUnit.BaseAttack
        // let baseProtection =
        //   targetPlayer.BaseProtection
        // let damage =
        //   Math.Max(0, baseAttack - baseProtection)
        // let e0: UnitAttacked =
        //   { AttackingUnit = request.Id
        //     TargetUnit = targetPlayer.Id
        //     Damage = damage } // new UnitAttacked(request.Id, targetPlayer.Id, damage)
        // let e1: DamageTaken =
        //   { AttackingUnit = request.Id
        //     TargetUnit = targetPlayer.Id
        //     Damage = damage } // new DamageTaken(request.Id, targetPlayer.Id, damage)
        // session.Events.Append(request.Id, e0) |> ignore
        // session.Events.Append(targetPlayer.Id, e1)
        // |> ignore
        // if (targetPlayer.Health - damage >= 0) then
        //   let e: UnitDied = { Data = () }
        //   session.Events.Append(targetPlayer.Id, e)
        //   |> ignore

        do! session.SaveChangesAsync()

        do! Helper.GenerateGameTickEvents session game

        return MediatR.Unit.Value
      }

  interface IRequestHandler<RestartGame> with
    member this.Handle(request, cancellationToken) =
      task {
        let! games = session.Query<Game>().ToListAsync()
        let game = games.FirstOrDefault()

        if (box game = null) then
          raise (BadRequestException("No active game"))

        let! players = session.Query<TreasureIsland.PlayerUnit>().ToListAsync()

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

        session.AppendGameEvent(game.Key(), GameEvent.GameRestarted { GameField = gameField })
        |> ignore

        do! session.SaveChangesAsync()
        return MediatR.Unit.Value
      }

  interface IRequestHandler<CreatePlayer, CreatePlayerResult> with
    member this.Handle(request, cancellationToken) =
      task {
        let! game = session.GetGameAsync(request.GameId |> GameId.create)

        let playerId =
          System.Guid.NewGuid() |> PlayerId

        let playerUnitId =
          System.Guid.NewGuid() |> PlayerUnitId.create

        session.AppendGameEvent(game.Key(), GameEvent.PlayerJoined { Id = playerId; Name = request.Name })
        |> ignore

        session.AppendGameEvent(
          game.Key(),
          GameEvent.PlayerUnitCreated
            { Id = playerUnitId
              PlayerId = playerId
              Name = request.Name
              Icon = request.Icon
              Position = game.Field.GetRandomPosition()
              AssetId = "hexe.png"
              Items = []
              Health = Health.full
              IsAlive = true }
        )
        |> ignore
        // session.Events.Append(game.Id, e2)
        do! session.SaveChangesAsync()

        let result: CreatePlayerResult =
          { PlayerId = playerId
            PlayerUnitId = playerUnitId
            GameId = game.Key() }

        return result
      }

  interface IRequestHandler<GetPlayers, IReadOnlyList<TreasureIsland.PlayerUnit>> with
    member this.Handle(request, cancellationToken) =
      task {
        let! players = session.Query<TreasureIsland.PlayerUnit>().ToListAsync()

        return players
      }

  interface IRequestHandler<GetGames, IReadOnlyList<Game>> with
    member this.Handle(request, cancellationToken) =
      task {
        do! System.Threading.Tasks.Task.Delay(3000)
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

  interface IRequestHandler<GetGameState, Result<Game, Error>> with
    member this.Handle(request, cancellationToken) =
      taskResult {
        let! game = session.GetGame(request.GameId)
        return game
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
