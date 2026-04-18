using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance;

    void Awake() => Instance = this;

    public IEnumerator RunTurn()
    {
        var enemies = FindObjectsByType<Unit>(FindObjectsSortMode.None)
                      .Where(u => u.IsEnemy).ToArray();

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            yield return new WaitForSeconds(0.6f);
            ProcessEnemy(enemy);
        }

        yield return new WaitForSeconds(0.4f);
        TurnManager.Instance.OnEnemyTurnDone();
    }

    void ProcessEnemy(Unit enemy)
    {
        Unit target = FindNearestPlayer(enemy);
        if (target == null) return;

        if (CombatSystem.InRange(enemy, target) && enemy.AP > 0)
        {
            CombatSystem.Attack(enemy, target);
            return;
        }

        if (enemy.AP > 0)
        {
            MoveToward(enemy, target);
            if (target != null && CombatSystem.InRange(enemy, target) && enemy.AP > 0)
                CombatSystem.Attack(enemy, target);
        }
    }

    Unit FindNearestPlayer(Unit enemy)
    {
        Unit nearest = null;
        int  minDist = int.MaxValue;

        foreach (var u in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (u.IsEnemy) continue;
            int dist = Mathf.Abs(Mathf.RoundToInt(enemy.transform.position.x) - Mathf.RoundToInt(u.transform.position.x))
                     + Mathf.Abs(Mathf.RoundToInt(enemy.transform.position.z) - Mathf.RoundToInt(u.transform.position.z));
            if (dist < minDist) { minDist = dist; nearest = u; }
        }
        return nearest;
    }

    void MoveToward(Unit enemy, Unit target)
    {
        Vector2Int myPos = new Vector2Int(
            Mathf.RoundToInt(enemy.transform.position.x),
            Mathf.RoundToInt(enemy.transform.position.z));
        Vector2Int targetPos = new Vector2Int(
            Mathf.RoundToInt(target.transform.position.x),
            Mathf.RoundToInt(target.transform.position.z));

        var occupied = new HashSet<Vector2Int>(
            FindObjectsByType<Unit>(FindObjectsSortMode.None)
                .Select(u => new Vector2Int(
                    Mathf.RoundToInt(u.transform.position.x),
                    Mathf.RoundToInt(u.transform.position.z))));

        var free = PathFinder.GetReachable(myPos, enemy.MoveRange)
                             .Where(p => !occupied.Contains(p)).ToList();
        if (free.Count == 0) return;

        Vector2Int best = free
            .OrderBy(p => Mathf.Abs(p.x - targetPos.x) + Mathf.Abs(p.y - targetPos.y))
            .First();

        enemy.transform.position = new Vector3(best.x, 0.5f, best.y);
        enemy.UseAP(1);
        Debug.Log($"{enemy.UnitName} → ({best.x},{best.y})");
    }
}
