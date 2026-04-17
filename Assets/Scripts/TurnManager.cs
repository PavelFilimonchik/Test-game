using UnityEngine;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public int TurnNumber { get; private set; } = 1;
    public bool IsPlayerTurn { get; private set; } = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartPlayerTurn();
    }

    void Update()
    {
        // Пробел = завершить ход
        if (IsPlayerTurn && Keyboard.current.spaceKey.wasPressedThisFrame)
            EndPlayerTurn();
    }

    void StartPlayerTurn()
    {
        IsPlayerTurn = true;

        // Восстановить очки действия всем юнитам игрока
        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
            unit.ResetAP();

        Debug.Log($"=== ХОД {TurnNumber} — ВАШ ХОД === (Пробел — завершить)");
    }

    void EndPlayerTurn()
    {
        IsPlayerTurn = false;
        Debug.Log($"Ход {TurnNumber}: игрок завершил ход.");
        Invoke(nameof(EnemyTurn), 1f);
    }

    void EnemyTurn()
    {
        Debug.Log($"Ход {TurnNumber}: ход противника...");
        Invoke(nameof(EndEnemyTurn), 1.5f);
    }

    void EndEnemyTurn()
    {
        TurnNumber++;
        StartPlayerTurn();
    }
}
