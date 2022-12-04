namespace TreasureIsland

open System

type PlayerUnit =
  {
    Id: Guid
    Key: PlayerUnitId
    GameId: GameId
    Name: string
    Icon: string
    Items: Item list
    IsEnabledToWalk: bool
    Position: Position
    RegenRate: int
    BaseAttack: int
    BaseProtection: int
    Health: int
    IsAlive: bool }

[<CLIMutable>]
type Game =
  { Id: Guid
    Version: int64
    Tick: int
    Status: GameStatus
    Items: Item list
    Field: GameField
    Mode: GameMode
    PlayerUnits: PlayerUnit list
    PlayerUnitIds: PlayerUnitId list
    ActiveUnit: Guid option }
  member this.Key()=
    this.Id |> GameId.create
  member this.Apply(e: GameTick) = { this with Tick = this.Tick + 1 }

  member this.Apply(e: GameEvent, meta: Marten.Events.IEvent) =
    match e with
    | GameStarted -> { this with Status = GameStatus.Running }
    | GameCreated e ->
      { Id = meta.Id
        Status = GameStatus.Initializing
        Field = e.GameField
        Mode = e.Mode
        Version = 0
        Tick = 0
        Items = []
        PlayerUnits = []
        PlayerUnitIds = []
        ActiveUnit = None }
    | _ -> this

  member this.Apply(e: ItemDropped) =
    let position =
      this.Field.Fields
      |> List.tryFind (fun v -> v.Position = e.Position)

    let field =
      { this.Field with
          Fields =
            this.Field.Fields
            |> List.map (fun v ->

              if v.Position = e.Position then
                { v with Items = v.Items @ [ e.Item ] }
              else
                v

            ) }
    // let field = this.Field.Fields.FirstOrDefault(fun v -> v.Position = e.Position)
    // if field <> null then
    //   field.Items.Add(e.Item)
    //
    // Items.Add(e);
    { this with Items = this.Items @ [ e.Item ] }

  member this.Apply(e: ActiveUnitChanged) =
    { this with ActiveUnit = Some e.UnitId }

  // member this.Apply( e:ItemRemoved)=
  // {
  //     var item = Items.FirstOrDefault(v => v.Item == e.Item && v.Position == e.Position);
  //     if (item != null)
  //     {
  //         Items.Remove(item);
  //     }
  //
  //     var field = Field.Fields.FirstOrDefault(v => v.Position == e.Position);
  //     if (field != null)
  //     {
  //         var i = field.Items.FirstOrDefault(v => v == e.Item);
  //         if (i != null) { field.Items.Remove(i); }
  //     }
  // }

  member this.Apply(e: PlayerJoined) =
    { this with PlayerUnitIds = this.PlayerUnitIds @ [ e.PlayerId ] }

  member this.Apply(e: GameAborted) =
    { this with Status = GameStatus.Aborted }

  member this.Apply(e: GameDrawn) = { this with Status = GameStatus.Ended }

  member this.Apply(e: GameEnded) = { this with Status = GameStatus.Ended }
