using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    private Vector2Int? selectedTile = null;
    private GameObject  tileHighlight;

    void Update()
    {
        HandleTileClick();
        HandleBuildKey();
    }

    void HandleTileClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (Unit.Selected != null) return;

        Ray   ray    = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float dist))
        {
            Vector3    wp      = ray.GetPoint(dist);
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(wp.x), Mathf.RoundToInt(wp.z));

            if (MapGenerator.TileObjects.ContainsKey(gridPos))
            {
                SelectTile(gridPos);
                Debug.Log($"Клетка ({gridPos.x},{gridPos.y}) | [1] Ферма (5 зол → +3 дер/ход) | [2] Шахта (10 зол → +5 зол/ход)");
            }
            else
                ClearSelection();
        }
    }

    void HandleBuildKey()
    {
        if (selectedTile == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TryBuild(selectedTile.Value, "Ферма",  5,  0, gpt:0, wpt:3, new Color(0.4f, 0.8f, 0.2f));

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TryBuild(selectedTile.Value, "Шахта", 10,  0, gpt:5, wpt:0, new Color(0.8f, 0.6f, 0.1f));
    }

    void TryBuild(Vector2Int pos, string name, int goldCost, int woodCost,
                  int gpt, int wpt, Color color)
    {
        foreach (var b in FindObjectsByType<Building>(FindObjectsSortMode.None))
        {
            var bp = new Vector2Int(Mathf.RoundToInt(b.transform.position.x),
                                   Mathf.RoundToInt(b.transform.position.z));
            if (bp == pos) { Debug.Log("Здесь уже есть постройка!"); return; }
        }
        foreach (var u in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            var up = new Vector2Int(Mathf.RoundToInt(u.transform.position.x),
                                   Mathf.RoundToInt(u.transform.position.z));
            if (up == pos) { Debug.Log("Клетка занята юнитом!"); return; }
        }

        if (MapGenerator.WaterTiles.Contains(pos))
        {
            Debug.Log("Нельзя строить на воде!");
            return;
        }

        if (!ResourceManager.Instance.Spend(goldCost, woodCost)) return;

        var root = new GameObject(name);
        root.transform.position = new Vector3(pos.x, 0f, pos.y);

        // Стены — куб
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.transform.parent        = root.transform;
        body.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        body.transform.localScale    = new Vector3(0.7f, 0.7f, 0.7f);
        body.GetComponent<MeshRenderer>().material.color = color;
        Destroy(body.GetComponent<Collider>());

        // Крыша — куб повёрнутый
        var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.parent        = root.transform;
        roof.transform.localPosition = new Vector3(0f, 0.85f, 0f);
        roof.transform.localScale    = new Vector3(0.55f, 0.3f, 0.55f);
        roof.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
        roof.GetComponent<MeshRenderer>().material.color = color * 0.75f;
        Destroy(roof.GetComponent<Collider>());

        var b2          = root.AddComponent<Building>();
        b2.BuildingName = name;
        b2.GoldPerTurn  = gpt;
        b2.WoodPerTurn  = wpt;

        Debug.Log($"Построено: {name} | Золото: {ResourceManager.Instance.Gold} | Дерево: {ResourceManager.Instance.Wood}");
        ClearSelection();
    }

    void SelectTile(Vector2Int pos)
    {
        ClearSelection();
        selectedTile = pos;

        tileHighlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tileHighlight.name = "TileHighlight";
        tileHighlight.transform.position   = new Vector3(pos.x, 0.03f, pos.y);
        tileHighlight.transform.rotation   = Quaternion.Euler(90f, 0f, 0f);
        tileHighlight.transform.localScale = Vector3.one * 0.95f;
        tileHighlight.GetComponent<MeshRenderer>().material.color = Color.yellow;
        Destroy(tileHighlight.GetComponent<Collider>());
    }

    void ClearSelection()
    {
        if (tileHighlight != null) Destroy(tileHighlight);
        selectedTile = null;
    }
}
