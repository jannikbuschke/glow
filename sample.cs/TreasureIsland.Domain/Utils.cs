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
        new Item("Sword", "🗡"), new Item("Axe", "🪓"), new Item("Shield", "🛡"), new Item("Food", "🍓"),
    };

    public static IEnumerable<Item> Get()
    {
        return items;
    }
}