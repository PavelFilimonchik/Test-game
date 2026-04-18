public enum TileType
{
    Meadow, // Луг
    Forest, // Лес
    Rock,   // Камни
    Water   // Вода
}

public class Tile
{
    public int X;
    public int Y;
    public TileType Type;

    public Tile(int x, int y, TileType type)
    {
        X = x;
        Y = y;
        Type = type;
    }
}
