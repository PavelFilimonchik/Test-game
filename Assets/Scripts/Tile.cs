// Типы местности на карте
public enum TileType
{
    Meadow, // Луг
    Forest, // Лес
    Field   // Пашня
}

// Данные одного тайла карты
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
