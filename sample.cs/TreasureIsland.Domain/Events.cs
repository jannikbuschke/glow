using System;
using Glow.NotificationsCore;
using Glow.TypeScript;

namespace Glow.Sample.TreasureIsland.Domain;



[GenerateTsInterface]
public record ItemPicked(Item Item);

[GenerateTsInterface]
public record PlayerPickedItem(Position Position, Item Item, Guid PlayerId);

[GenerateTsInterface]
public record ItemDropped(Position Position, Item Item);

[GenerateTsInterface]
public record PlayerJoined(Guid PlayerId) : IClientNotification;

[GenerateTsInterface]
public record PlayerInitialized(Position Position) : IClientNotification;

[GenerateTsInterface]
public record PlayerCreated(Guid Id, string Name, string Icon, Position Position) : IClientNotification;

[GenerateTsInterface]
public record PlayerMoved(Guid Id, Position Position) : IClientNotification;

public enum GameMode
{
    RoundBased = 0
}

public enum GameStatus
{
    Initializing = 1, Running = 2, Paused = 3, Ended = 4
}

[GenerateTsInterface]
public record PlayerEnabledForWalk(): IClientNotification;

[GenerateTsInterface]
public record GameCreated(Guid Id, GameField Field, GameMode Mode = GameMode.RoundBased) : IClientNotification;

[GenerateTsInterface]
public record GameStarted() : IClientNotification;

[GenerateTsInterface]
public record GameRestarted(GameField Field) : IClientNotification;

[GenerateTsInterface]
public record GameEnded() : IClientNotification;