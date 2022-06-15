using System;
using Glow.NotificationsCore;

namespace Glow.Sample;

public record PlayerHealed(int Health);

public record ItemPicked(Item Item) : IClientNotification;
public record ItemRemoved(Item Item, Position Position) : IClientNotification;

public record ItemDropped(Position Position, Item Item) : IClientNotification;

public record PlayerJoined(Guid PlayerId) : IClientNotification;

public record PlayerInitialized(Guid PlayerId, Position Position) : IClientNotification;

public record PlayerCreated(Guid Id, Guid GameId, string Name, string Icon) : IClientNotification;

public record PlayerMoved(Guid PlayerId, Position OldPosition, Position Position) : IClientNotification;

public record PlayerAttacked(Guid AttackingPlayer, Guid TargetPlayer, int Damage) : IClientNotification;

public record DamageTaken(Guid AttackingPlayer, Guid TargetPlayer, int Damage) : IClientNotification;

public enum GameMode
{
    RoundBased = 0
}

public enum GameStatus
{
    Initializing = 1, Running = 2, Paused = 3, Ended = 4
}

public record PlayerEnabledForWalk() : IClientNotification;

public record GameCreated(Guid Id, GameField Field, GameMode Mode = GameMode.RoundBased) : IClientNotification;

public record GameStarted() : IClientNotification;

public record GameRestarted(GameField Field) : IClientNotification;

public record GameEnded() : IClientNotification;