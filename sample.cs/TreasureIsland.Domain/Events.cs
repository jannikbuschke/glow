using System;
using Glow.NotificationsCore;

namespace Glow.Sample;

public record UnitHealed(int Health);

public record ItemPicked(Item Item) : IClientNotification;

public record ItemRemoved(Item Item, Position Position) : IClientNotification;

public record ItemDropped(Position Position, Item Item) : IClientNotification;

public record PlayerJoined(Guid PlayerId) : IClientNotification;

public record PlayerInitialized(Guid UnitId, Position Position) : IClientNotification;

public record UnitCreated(Guid Id, Guid GameId, string Name, string Icon) : IClientNotification;

public record UnitMoved(Guid UnitId, Position OldPosition, Position Position) : IClientNotification;

public record UnitAttacked(Guid AttackingUnit, Guid TargetUnit, int Damage) : IClientNotification;

public record GameTick() : IClientNotification;
public record UnitDied() : IClientNotification;

public record DamageTaken(Guid AttackingUnit, Guid TargetUnit, int Damage) : IClientNotification;

public enum GameMode
{
    RoundBased = 0
}

public enum GameStatus
{
    Initializing = 1, Running = 2, Paused = 3, Ended = 4, Aborted = 5
}

public record ActiveUnitChanged(Guid UnitId) : IClientNotification;
public record UnitEnabledForWalk() : IClientNotification;

public record GameCreated(Guid Id, GameField Field, GameMode Mode = GameMode.RoundBased) : IClientNotification;

public record GameStarted() : IClientNotification;

public record GameRestarted(GameField Field) : IClientNotification;

public record GameDrawn() : IClientNotification;
public record GameAborted() : IClientNotification;
public record GameEnded(Guid Winner) : IClientNotification;