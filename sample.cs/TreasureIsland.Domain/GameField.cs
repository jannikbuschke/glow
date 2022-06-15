using System.Collections.Generic;
using Glow.Sample.TreasureIsland.Domain;

namespace Glow.Sample;

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