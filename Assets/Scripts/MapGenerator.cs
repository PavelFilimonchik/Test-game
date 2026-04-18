using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int Width  = 32;
    public int Height = 32;

    public static Dictionary<Vector2Int, GameObject> TileObjects  = new Dictionary<Vector2Int, GameObject>();
    public static HashSet<Vector2Int>                WaterTiles   = new HashSet<Vector2Int>();

    private Tile[,] tiles;

    void Start()
    {
        TileObjects.Clear();
        WaterTiles.Clear();
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
        int r = Random.Range(0, 100);
        if (r <  5) return TileType.Water;   //  5%
        if (r < 15) return TileType.Rock;    // 10%
        if (r < 35) return TileType.Forest;  // 20%
        return TileType.Meadow;              // 65%
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

        // Земля
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = $"Tile {tile.X},{tile.Y}";
        ground.transform.parent     = transform;
        ground.transform.position   = worldPos;
        ground.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
        ground.GetComponent<MeshRenderer>().material.color = GroundColor(tile.Type);
        Destroy(ground.GetComponent<Collider>());
        TileObjects[key] = ground;

        if (tile.Type == TileType.Water)
            WaterTiles.Add(key);

        // 3D объекты поверх земли
        switch (tile.Type)
        {
            case TileType.Forest: SpawnTree(worldPos);  break;
            case TileType.Rock:   SpawnRock(worldPos);  break;
        }
    }

    void SpawnTree(Vector3 pos)
    {
        // Ствол
        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name                 = "Trunk";
        trunk.transform.parent     = transform;
        trunk.transform.position   = pos + new Vector3(0f, 0.35f, 0f);
        trunk.transform.localScale = new Vector3(0.12f, 0.35f, 0.12f);
        trunk.GetComponent<MeshRenderer>().material.color = new Color(0.38f, 0.22f, 0.08f);
        Destroy(trunk.GetComponent<Collider>());

        // Крона
        var foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.name                 = "Foliage";
        foliage.transform.parent     = transform;
        foliage.transform.position   = pos + new Vector3(0f, 0.95f, 0f);
        foliage.transform.localScale = new Vector3(0.65f, 0.75f, 0.65f);
        foliage.GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.42f, 0.1f);
        Destroy(foliage.GetComponent<Collider>());
    }

    void SpawnRock(Vector3 pos)
    {
        var rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.name                 = "Rock";
        rock.transform.parent     = transform;
        rock.transform.position   = pos + new Vector3(0f, 0.15f, 0f);
        rock.transform.localScale = new Vector3(0.45f, 0.3f, 0.45f);
        rock.transform.rotation   = Quaternion.Euler(0f, Random.Range(0f, 45f), 0f);
        rock.GetComponent<MeshRenderer>().material.color = new Color(0.52f, 0.52f, 0.56f);
        Destroy(rock.GetComponent<Collider>());
    }

    Color GroundColor(TileType type)
    {
        switch (type)
        {
            case TileType.Meadow: return new Color(0.35f, 0.72f, 0.25f);
            case TileType.Forest: return new Color(0.18f, 0.48f, 0.15f);
            case TileType.Rock:   return new Color(0.55f, 0.55f, 0.55f);
            case TileType.Water:  return new Color(0.15f, 0.45f, 0.85f);
            default:              return Color.white;
        }
    }
}
