using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public string UnitName;
    public int HP, MaxHP, Attack, MoveRange, AttackRange = 1, AP, MaxAP = 2;
    public bool IsEnemy;

    private SpriteRenderer sr;
    private Color baseColor;
    public static Unit Selected;

    private List<Vector2Int> reachable  = new List<Vector2Int>();
    private List<GameObject> highlights = new List<GameObject>();

    void Start()
    {
        sr        = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        AP        = MaxAP;
    }

    void Update()
    {
        if (IsEnemy) return;
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

        // Клик по другому юниту
        if (hit.collider != null)
        {
            Unit other = hit.collider.GetComponent<Unit>();
            if (other != null)
            {
                if (other.IsEnemy && CombatSystem.InRange(this, other) && AP > 0)
                    CombatSystem.Attack(this, other);
                else if (!other.IsEnemy)
                    other.Select();
                else
                    Debug.Log($"Слишком далеко для атаки! (дальность: {AttackRange})");
                return;
            }
        }

        // Клик по клетке — переместиться
        Vector2Int gridPos = Vector2Int.RoundToInt(mouseWorld);
        if (reachable.Contains(gridPos) && AP > 0)
            MoveTo(gridPos);
        else
            Deselect();
    }

    public void Select()
    {
        if (Selected != null && Selected != this)
            Selected.Deselect();

        Selected = this;
        sr.color = Color.yellow;
        RefreshHighlights();
        Debug.Log($"Выбран: {UnitName} | HP: {HP}/{MaxHP} | Атака: {Attack} | AP: {AP}/{MaxAP}");
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

    public void RefreshHighlights()
    {
        ClearHighlights();
        if (AP <= 0) return;

        Vector2Int myPos = Vector2Int.RoundToInt(transform.position);
        Sprite sq = MakeSquareSprite();

        // Голубые клетки — зона движения
        reachable = PathFinder.GetReachable(myPos, MoveRange);
        foreach (var p in reachable)
            CreateHighlight(p, new Color(0f, 1f, 1f, 0.35f), sq, -0.5f);

        // Красные клетки — враги в зоне атаки
        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (!unit.IsEnemy) continue;
            if (CombatSystem.InRange(this, unit))
            {
                Vector2Int ep = Vector2Int.RoundToInt(unit.transform.position);
                CreateHighlight(ep, new Color(1f, 0f, 0f, 0.5f), sq, -0.6f);
            }
        }
    }

    void CreateHighlight(Vector2Int pos, Color color, Sprite sprite, float z)
    {
        var h = new GameObject("Highlight");
        h.transform.position = new Vector3(pos.x, pos.y, z);
        var hsr    = h.AddComponent<SpriteRenderer>();
        hsr.sprite = sprite;
        hsr.color  = color;
        highlights.Add(h);
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

    public void TakeDamage(int damage, string attackerName)
    {
        HP -= damage;
        HP  = Mathf.Max(HP, 0);
        Debug.Log($"{attackerName} атакует {UnitName}: -{damage} HP. Осталось: {HP}/{MaxHP}");

        if (HP <= 0)
        {
            Debug.Log($"{UnitName} уничтожен!");
            if (Selected == this) Deselect();
            Destroy(gameObject);
        }
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
