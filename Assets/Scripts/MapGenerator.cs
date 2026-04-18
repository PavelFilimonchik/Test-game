using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int Width  = 10;
    public int Height = 10;

    public static Dictionary<Vector2Int, GameObject> TileObjects =
        new Dictionary<Vector2Int, GameObject>();

    private Tile[,] tiles;

    void Start()
    {
        TileObjects.Clear();
        GenerateMap();
        RenderMap();
    }

    void GenerateMap()
    {
        tiles = new Tile[Width, Height];
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                tiles[x, y] = new Tile(x, y, RandomTileType());
    }

    TileType RandomTileType()
    {
        int r = Random.Range(0, 10);
        if (r < 4) return TileType.Meadow;
        if (r < 7) return TileType.Forest;
        return TileType.Field;
    }

    void RenderMap()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                SpawnTile(tiles[x, y]);
    }

    void SpawnTile(Tile tile)
    {
        Vector3    worldPos = new Vector3(tile.X, 0f, tile.Y);
        Vector2Int key      = new Vector2Int(tile.X, tile.Y);

        // Земля — Plane (10×10 в Unity, масштабируем в 1×1)
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = $"Tile {tile.X},{tile.Y}";
        ground.transform.parent     = transform;
        ground.transform.position   = worldPos;
        ground.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
        ground.GetComponent<MeshRenderer>().material.color = GroundColor(tile.Type);
        Destroy(ground.GetComponent<Collider>()); // убираем коллайдер земли

        TileObjects[key] = ground;

        if (tile.Type == TileType.Forest)
            SpawnTree(worldPos, ground.transform);
    }

    void SpawnTree(Vector3 pos, Transform parent)
    {
        // Ствол
        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name                   = "Trunk";
        trunk.transform.parent       = parent;
        trunk.transform.position     = pos + new Vector3(0f, 0.3f, 0f);
        trunk.transform.localScale   = new Vector3(1.5f, 3f, 1.5f);
        trunk.GetComponent<MeshRenderer>().material.color = new Color(0.4f, 0.25f, 0.1f);
        Destroy(trunk.GetComponent<Collider>());

        // Крона
        var foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.name                 = "Foliage";
        foliage.transform.parent     = parent;
        foliage.transform.position   = pos + new Vector3(0f, 1.0f, 0f);
        foliage.transform.localScale = Vector3.one * 7f;
        foliage.GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.45f, 0.1f);
        Destroy(foliage.GetComponent<Collider>());
    }

    Color GroundColor(TileType type)
    {
        switch (type)
        {
            case TileType.Meadow: return new Color(0.35f, 0.75f, 0.25f);
            case TileType.Forest: return new Color(0.15f, 0.45f, 0.15f);
            case TileType.Field:  return new Color(0.75f, 0.65f, 0.30f);
            default:              return Color.white;
        }
    }
}
