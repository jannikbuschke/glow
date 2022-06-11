using System;
using System.Collections.Generic;
using System.Linq;
using Glow.NotificationsCore;
using Glow.Sample.TreasureIsland.Domain;
using Glow.TypeScript;

namespace Glow.Sample.TreasureIsland.Projections;

[GenerateTsInterface]
public record CurrentGameState(Guid GameId, List<ItemDropped> CurrentItems, IReadOnlyList<Player> Players, Game Game) : IClientNotification;

public class Player
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public Position Position { get; set; }
    public List<Item> Items { get; set; } = new();
    public bool IsEnabledToWalk { get; set; }

    public void Apply(PlayerCreated e)
    {
        Id = e.Id;
        Name = e.Name;
        Icon = e.Icon;
        Position = e.Position;
    }

    public void Apply(PlayerInitialized e)
    {
        Position = e.Position;
        Items = new();
    }

    public void Apply(PlayerMoved e)
    {
        Position = e.Position;
        IsEnabledToWalk = false;
    }

    public void Apply(PlayerEnabledForWalk e)
    {
        IsEnabledToWalk = true;
    }

    public void Apply(ItemPicked e)
    {
        Items.Add(e.Item);
    }
}

public class Game
{
    public Guid Id { get; set; }

    public GameStatus Status { get; set; }

    public List<Guid> Players { get; set; } = new();

    public List<ItemDropped> Items { get; set; } = new();

    public GameField Field { get; set; }

    public GameMode Mode { get; set; }

    public void Apply(ItemDropped e)
    {
        Items.Add(e);
    }

    public void Apply(GameStarted e)
    {
        Status = GameStatus.Running;
    }

    public void Apply(GameCreated e)
    {
        Id = e.Id;
        Status = GameStatus.Initializing;
        Field = e.Field;
        Mode = e.Mode;
    }

    public void Apply(PlayerJoined e)
    {
        Players.Add(e.PlayerId);
    }

    public bool ShouldDelete(GameEnded e)
    {
        return true;
    }

    public void Apply(PlayerPickedItem e)
    {
        var item = Items.FirstOrDefault(v => v.Item == e.Item && v.Position == e.Position);
        if (item != null)
        {
            Items.Remove(item);
        }
    }
}