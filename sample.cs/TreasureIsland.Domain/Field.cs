using System.Collections.Generic;

namespace Glow.Sample;

public record Field(Position Position, Tile Tile, List<Item> Items)
{
    public static Field New(Position position, Tile tile)
    {
        return new Field(position, tile, new List<Item>());
    }
}