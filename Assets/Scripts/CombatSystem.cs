using UnityEngine;

public static class CombatSystem
{
    public static void Attack(Unit attacker, Unit target)
    {
        attacker.UseAP(1);
        if (!attacker.IsEnemy) attacker.RefreshHighlights();
        target.TakeDamage(attacker.Attack, attacker.UnitName);
    }

    public static bool InRange(Unit attacker, Unit target)
    {
        int ax = Mathf.RoundToInt(attacker.transform.position.x);
        int az = Mathf.RoundToInt(attacker.transform.position.z);
        int tx = Mathf.RoundToInt(target.transform.position.x);
        int tz = Mathf.RoundToInt(target.transform.position.z);
        return Mathf.Abs(ax - tx) + Mathf.Abs(az - tz) <= attacker.AttackRange;
    }
}
