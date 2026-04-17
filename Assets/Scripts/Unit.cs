using UnityEngine;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour
{
    public string UnitName;
    public int HP;
    public int MaxHP;
    public int Attack;
    public int MoveRange;

    private SpriteRenderer sr;
    private Color baseColor;
    private static Unit selected;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
                Select();
            else if (selected == this)
                Deselect();
        }
    }

    void Select()
    {
        if (selected != null && selected != this)
            selected.Deselect();

        selected = this;
        sr.color = Color.yellow;
        Debug.Log($"Выбран: {UnitName} | HP: {HP}/{MaxHP} | Атака: {Attack}");
    }

    void Deselect()
    {
        selected = null;
        sr.color = baseColor;
    }
}
