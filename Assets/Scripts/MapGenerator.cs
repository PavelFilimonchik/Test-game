using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Width = 10;
    public int Height = 10;

    private Tile[,] tiles;

    void Start()
    {
        GenerateMap();
        RenderMap();
    }

    void GenerateMap()
    {
        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                TileType type = RandomTileType();
                tiles[x, y] = new Tile(x, y, type);
            }
        }
    }

    TileType RandomTileType()
    {
        int roll = Random.Range(0, 10);
        if (roll < 4) return TileType.Meadow; // 40% луг
        if (roll < 7) return TileType.Forest; // 30% лес
        return TileType.Field;                // 30% пашня
    }

    void RenderMap()
    {
        Sprite square = MakeSquareSprite();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                SpawnTile(tiles[x, y], square);
            }
        }
    }

    // Создаём белый квадратик 1×1 прямо из кода — не нужны внешние текстуры
    Sprite MakeSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    void SpawnTile(Tile tile, Sprite sprite)
    {
        GameObject obj = new GameObject($"Tile {tile.X},{tile.Y}");
        obj.transform.parent = transform;
        obj.transform.position = new Vector3(tile.X, tile.Y, 0);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = TileColor(tile.Type);
    }

    Color TileColor(TileType type)
    {
        switch (type)
        {
            case TileType.Meadow: return new Color(0.35f, 0.80f, 0.25f); // зелёный
            case TileType.Forest: return new Color(0.10f, 0.40f, 0.10f); // тёмно-зелёный
            case TileType.Field:  return new Color(0.80f, 0.68f, 0.30f); // коричнево-жёлтый
            default:              return Color.white;
        }
    }
}
