using System;
using System.Collections.Generic;
using System.Linq;

namespace Glow.Sample;

public class Game
{
    public Guid Id { get; set; }
    public long Version { get; set; }

    public GameStatus Status { get; set; }

    public List<Guid> Players { get; set; } = new();

    public List<ItemDropped> Items { get; set; } = new();

    public GameField Field { get; set; }

    public GameMode Mode { get; set; }

    public void Apply(ItemDropped e)
    {
        var field = Field.Fields.FirstOrDefault(v => v.Position == e.Position);
        if (field != null) { field.Items.Add(e.Item); }

        Items.Add(e);
    }

    public void Apply(ItemRemoved e)
    {
        var item = Items.FirstOrDefault(v => v.Item == e.Item && v.Position == e.Position);
        if (item != null)
        {
            Items.Remove(item);
        }

        var field = Field.Fields.FirstOrDefault(v => v.Position == e.Position);
        if (field != null)
        {
            var i = field.Items.FirstOrDefault(v => v == e.Item);
            if (i != null) { field.Items.Remove(i); }
        }
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


}