using System.Collections.Generic;

namespace Glow.Sample.TreasureIsland.Domain;

public record GameField(IReadOnlyList<Field> Fields)
{
    public Position GetRandomPosition()
    {
        return GetRandomField().Position;
    }

    public Field GetRandomField()
    {
        return Fields[Utils.RandomInt(Fields.Count)];
    }
}