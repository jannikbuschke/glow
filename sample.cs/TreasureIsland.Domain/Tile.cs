namespace Glow.Sample;


public record Tile(string Color, TileName Name, bool Walkable);

public enum TileName
{
    Grass = 1, Water = 2, Mountain = 3, Wood = 4, Corn = 5
}