using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    void Start()
    {
        SpawnUnit("Пехота",  2, 5, new Color(0.2f, 0.5f, 1.0f),  hp: 30, atk: 10, move: 3);
        SpawnUnit("Конница", 5, 7, new Color(1.0f, 0.5f, 0.0f),  hp: 20, atk: 15, move: 5);
        SpawnUnit("Лучник",  8, 3, new Color(0.8f, 0.2f, 0.8f),  hp: 15, atk: 12, move: 2);
    }

    void SpawnUnit(string unitName, int x, int y, Color color,
                   int hp, int atk, int move)
    {
        GameObject obj = new GameObject(unitName);
        obj.transform.position = new Vector3(x, y, -1f);

        // Спрайт — белый кружок
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = MakeCircleSprite();
        sr.color = color;
        sr.transform.localScale = Vector3.one * 0.8f;

        // Коллайдер для клика мышью
        obj.AddComponent<CircleCollider2D>();

        Unit unit = obj.AddComponent<Unit>();
        unit.UnitName = unitName;
        unit.HP      = hp;
        unit.MaxHP   = hp;
        unit.Attack  = atk;
        unit.MoveRange = move;
    }

    Sprite MakeCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        float r = size / 2f;

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(r, r));
                tex.SetPixel(x, y, dist < r ? Color.white : Color.clear);
            }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
