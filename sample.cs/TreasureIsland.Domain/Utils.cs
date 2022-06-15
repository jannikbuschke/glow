using System;
using System.Collections.Generic;

namespace Glow.Sample.TreasureIsland.Domain;

public static class Utils
{
    private static Random random = new();

    public static int RandomInt(int max)
    {
        return random.Next(max);
    }

    public static bool Chance(double x)
    {
        return random.NextDouble() < x;
    }

    public static Item GetRandomItem()
    {
        var pos = random.Next() % items.Count;
        return items[pos];
    }

    private static List<Item> items = new()
    {
        // new Item("Coin", "🍕🍔🍟🌭🍿🥓🍓🍅🥭🍎🍉🍄🌶🍀❤🧡💛💚💥💢💫⛏🔨🪓🗡⚔🔪🏹🛡💣"),
        new Item("Sword", "🗡", 0, new AttackModifier(3), new Protection(0)),
        new Item("Sword", "🗡", 0, new AttackModifier(3), new Protection(0)),
        new Item("Sword", "🗡", 0, new AttackModifier(3), new Protection(0)),
        new Item("Sword", "🗡", 0, new AttackModifier(3), new Protection(0)),
        new Item("Axe", "🪓", 0, new AttackModifier(2), new Protection(0)),
        new Item("Double Sword", "🔪", 0, new AttackModifier(2), new Protection(0)),
        new Item("Bow", "🏹", 0, new AttackModifier(5), new Protection(0)),
        new Item("Knife", "⚔", 0, new AttackModifier(5), new Protection(0)),
        new Item("Axe", "🪓", 0, new AttackModifier(2), new Protection(0)),
        new Item("Axe", "🪓", 0, new AttackModifier(2), new Protection(0)),
        new Item("Axe", "🪓", 0, new AttackModifier(2), new Protection(0)),
        new Item("Amulet", "🥇", 3, new AttackModifier(0), new Protection(0)),
        new Item("Amulet", "🥈", 2, new AttackModifier(0), new Protection(0)),
        new Item("Amulet", "🥉", 1, new AttackModifier(0), new Protection(0)),
        new Item("Shield", "🛡", 0, new AttackModifier(0), new Protection(3)),
        new Item("Shield", "⛓", 0, new AttackModifier(0), new Protection(3)),
        new Item("Amulet", "🏅", 1, new AttackModifier(0), new Protection(0)),
        new Item("Amulet", "🎖", 2, new AttackModifier(0), new Protection(0)),
        new Item("Amulet", "💍", 3, new AttackModifier(0), new Protection(0)),
        // new Item("Food", "🍓"),
    };

    public static IEnumerable<Item> Get()
    {
        return items;
    }
}