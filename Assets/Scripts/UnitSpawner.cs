using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    void Start()
    {
        // Игрок
        Spawn("Пехота",   2, 5, new Color(0.2f, 0.5f, 1.0f), hp:30, atk:10, move:3, atkRange:1, enemy:false);
        Spawn("Конница",  5, 7, new Color(1.0f, 0.5f, 0.0f), hp:20, atk:15, move:5, atkRange:1, enemy:false);
        Spawn("Лучник",   8, 3, new Color(0.8f, 0.2f, 0.8f), hp:15, atk:12, move:2, atkRange:2, enemy:false);

        // Враги
        Spawn("Враг-меч", 7, 8, new Color(0.9f, 0.1f, 0.1f), hp:25, atk: 8, move:3, atkRange:1, enemy:true);
        Spawn("Враг-лук", 3, 2, new Color(0.7f, 0.0f, 0.0f), hp:15, atk:10, move:2, atkRange:2, enemy:true);
    }

    void Spawn(string unitName, int x, int z, Color color,
               int hp, int atk, int move, int atkRange, bool enemy)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        obj.name = unitName;
        obj.transform.position   = new Vector3(x, 0.5f, z);
        obj.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
        obj.GetComponent<MeshRenderer>().material.color = color;

        var u         = obj.AddComponent<Unit>();
        u.UnitName    = unitName;
        u.HP          = hp;
        u.MaxHP       = hp;
        u.Attack      = atk;
        u.MoveRange   = move;
        u.AttackRange = atkRange;
        u.IsEnemy     = enemy;
    }
}
