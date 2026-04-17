using UnityEngine;

public static class CombatSystem
{
    public static void Attack(Unit attacker, Unit target)
    {
        int damage = attacker.Attack;
        attacker.UseAP(1);
        if (!attacker.IsEnemy) attacker.RefreshHighlights();
        target.TakeDamage(damage, attacker.UnitName);
    }

    public static bool InRange(Unit attacker, Unit target)
    {
        Vector2Int a = Vector2Int.RoundToInt(attacker.transform.position);
        Vector2Int t = Vector2Int.RoundToInt(target.transform.position);
        int dist = Mathf.Abs(a.x - t.x) + Mathf.Abs(a.y - t.y);
        return dist <= attacker.AttackRange;
    }
}
