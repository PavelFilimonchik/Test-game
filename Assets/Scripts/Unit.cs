using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public string UnitName;
    public int HP, MaxHP, Attack, MoveRange, AttackRange = 1, AP, MaxAP = 2;
    public bool IsEnemy;

    private MeshRenderer mr;
    private Color        baseColor;
    public static Unit   Selected;

    private List<Vector2Int> reachable  = new List<Vector2Int>();
    private List<GameObject> highlights = new List<GameObject>();

    void Start()
    {
        mr        = GetComponent<MeshRenderer>();
        baseColor = mr.material.color;
        AP        = MaxAP;
    }

    void Update()
    {
        if (IsEnemy) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray        ray    = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        bool       didHit = Physics.Raycast(ray, out hit);

        // Клик по этому юниту
        if (didHit && hit.collider.gameObject == gameObject)
        {
            Select();
            return;
        }

        if (Selected != this) return;

        // Клик по другому юниту
        if (didHit)
        {
            Unit other = hit.collider.GetComponent<Unit>();
            if (other != null)
            {
                if (other.IsEnemy && CombatSystem.InRange(this, other) && AP > 0)
                    CombatSystem.Attack(this, other);
                else if (!other.IsEnemy)
                    other.Select();
                else
                    Debug.Log($"Слишком далеко! (дальность: {AttackRange})");
                return;
            }
        }

        // Клик по земле — переместиться
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float enter;
        if (ground.Raycast(ray, out enter))
        {
            Vector3    worldPos = ray.GetPoint(enter);
            Vector2Int gridPos  = new Vector2Int(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.z)
            );
            if (reachable.Contains(gridPos) && AP > 0)
                MoveTo(gridPos);
            else
                Deselect();
        }
    }

    public void Select()
    {
        if (Selected != null && Selected != this) Selected.Deselect();
        Selected = this;
        mr.material.color = Color.yellow;
        RefreshHighlights();
        Debug.Log($"Выбран: {UnitName} | HP:{HP}/{MaxHP} | AP:{AP}/{MaxAP}");
    }

    public void Deselect()
    {
        ClearHighlights();
        reachable.Clear();
        Selected = null;
        mr.material.color = baseColor;
    }

    void MoveTo(Vector2Int pos)
    {
        UseAP(1);
        transform.position = new Vector3(pos.x, 0.5f, pos.y);
        RefreshHighlights();
        Debug.Log($"{UnitName} → ({pos.x},{pos.y}) | AP:{AP}/{MaxAP}");
    }

    public void RefreshHighlights()
    {
        ClearHighlights();
        if (AP <= 0) return;

        Vector2Int myPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );

        reachable = PathFinder.GetReachable(myPos, MoveRange);
        foreach (var p in reachable)
            SpawnHighlight(p, new Color(0f, 0.8f, 1f));

        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (!unit.IsEnemy) continue;
            if (CombatSystem.InRange(this, unit))
            {
                Vector2Int ep = new Vector2Int(
                    Mathf.RoundToInt(unit.transform.position.x),
                    Mathf.RoundToInt(unit.transform.position.z)
                );
                SpawnHighlight(ep, new Color(1f, 0.15f, 0.15f));
            }
        }
    }

    void SpawnHighlight(Vector2Int pos, Color color)
    {
        var h = GameObject.CreatePrimitive(PrimitiveType.Quad);
        h.name = "Highlight";
        h.transform.position   = new Vector3(pos.x, 0.02f, pos.y);
        h.transform.rotation   = Quaternion.Euler(90f, 0f, 0f);
        h.transform.localScale = Vector3.one * 0.9f;
        h.GetComponent<MeshRenderer>().material.color = color;
        Destroy(h.GetComponent<Collider>());
        highlights.Add(h);
    }

    void ClearHighlights()
    {
        foreach (var h in highlights)
            if (h != null) Destroy(h);
        highlights.Clear();
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
