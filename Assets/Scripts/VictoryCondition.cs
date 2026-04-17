using UnityEngine;

public static class VictoryCondition
{
    public static void Check()
    {
        bool hasPlayer = false;
        bool hasEnemy  = false;

        foreach (var u in Object.FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (u.IsEnemy) hasEnemy  = true;
            else           hasPlayer = true;
        }

        if (!hasEnemy)
        {
            Debug.Log("=== ПОБЕДА! Все враги уничтожены! ===");
            UIManager.Instance?.ShowResult(true);
        }
        else if (!hasPlayer)
        {
            Debug.Log("=== ПОРАЖЕНИЕ! Все ваши юниты уничтожены! ===");
            UIManager.Instance?.ShowResult(false);
        }
    }
}
