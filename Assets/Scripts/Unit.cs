using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public string UnitName;
    public int HP, MaxHP, Attack, MoveRange, AP, MaxAP = 2;

    private SpriteRenderer sr;
    private Color baseColor;
    public static Unit Selected;

    private List<Vector2Int> reachable   = new List<Vector2Int>();
    private List<GameObject> highlights  = new List<GameObject>();

    void Start()
    {
        sr        = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        AP        = MaxAP;
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit   = Physics2D.Raycast(mouseWorld, Vector2.zero);

        // Клик по этому юниту — выбрать
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Select();
            return;
        }

        if (Selected != this) return;

        // Клик по клетке — попытаться переместиться
        Vector2Int gridPos = Vector2Int.RoundToInt(mouseWorld);
        if (reachable.Contains(gridPos) && AP > 0)
            MoveTo(gridPos);
        else
            Deselect();
    }

    void Select()
    {
        if (Selected != null && Selected != this)
            Selected.Deselect();

        Selected  = this;
        sr.color  = Color.yellow;
        RefreshHighlights();
        Debug.Log($"Выбран: {UnitName} | HP: {HP}/{MaxHP} | AP: {AP}/{MaxAP}");
    }

    public void Deselect()
    {
        ClearHighlights();
        reachable.Clear();
        Selected = null;
        sr.color = baseColor;
    }

    void MoveTo(Vector2Int pos)
    {
        UseAP(1);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        RefreshHighlights();
        Debug.Log($"{UnitName} → ({pos.x},{pos.y}) | AP: {AP}/{MaxAP}");
    }

    void RefreshHighlights()
    {
        ClearHighlights();
        if (AP <= 0) return;

        Vector2Int myPos = Vector2Int.RoundToInt(transform.position);
        reachable = PathFinder.GetReachable(myPos, MoveRange);

        Sprite sq = MakeSquareSprite();
        foreach (var p in reachable)
        {
            var h   = new GameObject("Highlight");
            h.transform.position = new Vector3(p.x, p.y, -0.5f);
            var hsr = h.AddComponent<SpriteRenderer>();
            hsr.sprite = sq;
            hsr.color  = new Color(0f, 1f, 1f, 0.35f);
            highlights.Add(h);
        }
    }

    void ClearHighlights()
    {
        foreach (var h in highlights)
            if (h != null) Destroy(h);
        highlights.Clear();
    }

    Sprite MakeSquareSprite()
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, Color.white);
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    public void ResetAP()
    {
        AP = MaxAP;
        if (Selected == this) RefreshHighlights();
    }

    public bool UseAP(int amount = 1)
    {
        if (AP < amount) return false;
        AP -= amount;
        return true;
    }
}
