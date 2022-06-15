using System;
using System.Collections.Generic;
using System.Linq;

namespace Glow.Sample;

public class Player
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public List<Item> Items { get; set; } = new();
    public bool IsEnabledToWalk { get; set; }
    public Position Position { get; set; }

    public int RegenRate { get; set; }
    public int BaseAttack { get; set; }
    public int BaseProtection { get; set; }

    public int Health { get; set; }

    public void Apply(DamageTaken e)
    {
        Health -= e.Damage;
        if (Health <= 0)
        {
            Health = 0;
        }
    }

    public void Apply(PlayerCreated e)
    {
        Id = e.Id;
        Name = e.Name;
        Icon = e.Icon;
        GameId = e.GameId;
        Health = 100;
        Position = new Position(0, 0, 0);
    }

    public void Apply(PlayerInitialized e)
    {
        Items = new();
        Position = e.Position;
    }

    public void Apply(PlayerMoved e)
    {
        IsEnabledToWalk = false;
        Position = e.Position;
    }

    public void Apply(PlayerEnabledForWalk e)
    {
        IsEnabledToWalk = true;
    }

    public void Apply(ItemPicked e)
    {
        Items.Add(e.Item);
        BaseAttack = 1 + Items.Sum(v => v.AttackModifier.BaseAttack);
        BaseProtection = Items.Sum(v => v.Protection.BaseDamageReduction);
        RegenRate = Items.Sum(v => v.Regeneration);
    }
}