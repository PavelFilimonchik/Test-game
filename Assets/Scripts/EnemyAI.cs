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

        // Атаковать если уже в зоне досягаемости
        if (CombatSystem.InRange(enemy, target) && enemy.AP > 0)
        {
            CombatSystem.Attack(enemy, target);
            return;
        }

        // Двигаться к ближайшему игроку
        if (enemy.AP > 0)
        {
            MoveToward(enemy, target);

            // Атаковать после движения если теперь в зоне
            if (target != null && CombatSystem.InRange(enemy, target) && enemy.AP > 0)
                CombatSystem.Attack(enemy, target);
        }
    }

    Unit FindNearestPlayer(Unit enemy)
    {
        Unit nearest = null;
        int  minDist = int.MaxValue;

        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (unit.IsEnemy) continue;
            Vector2Int a = Vector2Int.RoundToInt(enemy.transform.position);
            Vector2Int b = Vector2Int.RoundToInt(unit.transform.position);
            int dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            if (dist < minDist) { minDist = dist; nearest = unit; }
        }

        return nearest;
    }

    void MoveToward(Unit enemy, Unit target)
    {
        Vector2Int myPos     = Vector2Int.RoundToInt(enemy.transform.position);
        Vector2Int targetPos = Vector2Int.RoundToInt(target.transform.position);

        // Занятые клетки — нельзя вставать на них
        var occupied = new HashSet<Vector2Int>(
            FindObjectsByType<Unit>(FindObjectsSortMode.None)
                .Select(u => Vector2Int.RoundToInt(u.transform.position))
        );

        var reachable = PathFinder.GetReachable(myPos, enemy.MoveRange);
        var free      = reachable.Where(p => !occupied.Contains(p)).ToList();

        if (free.Count == 0) return;

        Vector2Int best = free
            .OrderBy(p => Mathf.Abs(p.x - targetPos.x) + Mathf.Abs(p.y - targetPos.y))
            .First();

        enemy.transform.position = new Vector3(best.x, best.y, enemy.transform.position.z);
        enemy.UseAP(1);
        Debug.Log($"{enemy.UnitName} движется → ({best.x},{best.y})");
    }
}
