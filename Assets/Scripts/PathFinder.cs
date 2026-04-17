using UnityEngine;
using System.Collections.Generic;

public static class PathFinder
{
    static readonly Vector2Int[] Dirs =
    {
        Vector2Int.up, Vector2Int.down,
        Vector2Int.left, Vector2Int.right
    };

    // Возвращает все клетки, достижимые за <range> шагов
    public static List<Vector2Int> GetReachable(Vector2Int start, int range)
    {
        var visited = new HashSet<Vector2Int> { start };
        var queue   = new Queue<(Vector2Int pos, int dist)>();
        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            var (pos, dist) = queue.Dequeue();
            if (dist >= range) continue;

            foreach (var dir in Dirs)
            {
                Vector2Int next = pos + dir;
                if (!visited.Contains(next) && MapGenerator.TileObjects.ContainsKey(next))
                {
                    visited.Add(next);
                    queue.Enqueue((next, dist + 1));
                }
            }
        }

        visited.Remove(start);
        return new List<Vector2Int>(visited);
    }
}
