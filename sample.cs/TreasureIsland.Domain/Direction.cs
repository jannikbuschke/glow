namespace Glow.Sample.TreasureIsland.Domain;

public record Direction(int R, int Q, int S)
{
    public Position AddTo(Position p)
    {
        return new Position(p.R + R, p.Q + Q, p.S + S);
    }
}