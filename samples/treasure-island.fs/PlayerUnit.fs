namespace TreasureIsland

open System
open System.Linq

[<CLIMutable>]
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

  member this.Apply(e: DamageTaken) = { this with Health = e.Damage }

  member this.Apply(e: PlayerUnitCreated, meta: Marten.Events.IEvent) =
    { Id = meta.Id
      Key = PlayerUnitId meta.Id
      Name = e.Name
      Icon = e.Icon
      GameId = e.GameId
      Health = 100
      IsAlive = true
      Position = { Q = 0; R = 0; S = 0 }
      Items = []
      IsEnabledToWalk = true
      RegenRate = 0
      BaseAttack = 5
      BaseProtection = 1 }

  member this.Apply(e: PlayerUnitInitialized) =
    { this with
        Items = []
        Position = e.Position }

  member this.Apply(e: UnitMoved) =
    { this with
        IsEnabledToWalk = false
        Position = e.Position }

  member this.Apply(e: UnitEnabledForWalk) = { this with IsEnabledToWalk = true }

  member this.Apply(e: ItemPicked) =
    { this with
        Items = this.Items @ [ e.Item ]
        BaseAttack =
          1
          + this.Items.Sum(fun v -> v.AttackModifier.BaseAttack)
        BaseProtection = this.Items.Sum(fun v -> v.Protection.BaseDamageReduction)
        RegenRate = this.Items.Sum(fun v -> v.Regeneration) }

  member this.Apply(e: UnitDied) = { this with IsAlive = false }
