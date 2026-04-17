using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    private Vector2Int? selectedTile = null;
    private GameObject tileHighlight;

    void Update()
    {
        HandleTileClick();
        HandleBuildKey();
    }

    void HandleTileClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (Unit.Selected != null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit   = Physics2D.Raycast(mouseWorld, Vector2.zero);

        if (hit.collider == null || hit.collider.GetComponent<Unit>() == null)
        {
            Vector2Int gridPos = Vector2Int.RoundToInt(mouseWorld);
            if (MapGenerator.TileObjects.ContainsKey(gridPos))
            {
                SelectTile(gridPos);
                Debug.Log($"Клетка ({gridPos.x},{gridPos.y}) | [1] Ферма (5 зол → +3 дер/ход) | [2] Шахта (10 зол → +5 зол/ход)");
            }
            else
            {
                ClearSelection();
            }
        }
    }

    void HandleBuildKey()
    {
        if (selectedTile == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TryBuild(selectedTile.Value, "Ферма",  goldCost: 5,  woodCost: 0, gpt: 0, wpt: 3, new Color(0.3f, 0.85f, 0.2f));

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TryBuild(selectedTile.Value, "Шахта", goldCost: 10, woodCost: 0, gpt: 5, wpt: 0, new Color(0.9f, 0.7f, 0.1f));
    }

    void TryBuild(Vector2Int pos, string name, int goldCost, int woodCost,
                  int gpt, int wpt, Color color)
    {
        foreach (var b in FindObjectsByType<Building>(FindObjectsSortMode.None))
        {
            if (Vector2Int.RoundToInt(b.transform.position) == pos)
            {
                Debug.Log("Здесь уже есть постройка!");
                return;
            }
        }

        foreach (var u in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (Vector2Int.RoundToInt(u.transform.position) == pos)
            {
                Debug.Log("Клетка занята юнитом!");
                return;
            }
        }

        if (!ResourceManager.Instance.Spend(goldCost, woodCost)) return;

        GameObject obj = new GameObject(name);
        obj.transform.position   = new Vector3(pos.x, pos.y, -0.8f);
        obj.transform.localScale = Vector3.one * 0.55f;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSquare();
        sr.color  = color;

        Building building          = obj.AddComponent<Building>();
        building.BuildingName      = name;
        building.GoldPerTurn       = gpt;
        building.WoodPerTurn       = wpt;

        Debug.Log($"Построено: {name} | Золото: {ResourceManager.Instance.Gold} | Дерево: {ResourceManager.Instance.Wood}");
        ClearSelection();
    }

    void SelectTile(Vector2Int pos)
    {
        ClearSelection();
        selectedTile = pos;

        tileHighlight = new GameObject("TileHighlight");
        tileHighlight.transform.position = new Vector3(pos.x, pos.y, -0.4f);
        var sr    = tileHighlight.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSquare();
        sr.color  = new Color(1f, 1f, 0f, 0.45f);
    }

    void ClearSelection()
    {
        if (tileHighlight != null) Destroy(tileHighlight);
        selectedTile = null;
    }

    Sprite MakeSquare()
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, Color.white);
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
