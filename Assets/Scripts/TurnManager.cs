using UnityEngine;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public int TurnNumber { get; private set; } = 1;
    public bool IsPlayerTurn { get; private set; } = true;

    void Awake() => Instance = this;

    void Start() => StartPlayerTurn();

    void Update()
    {
        if (IsPlayerTurn && Keyboard.current.spaceKey.wasPressedThisFrame)
            EndPlayerTurn();
    }

    void StartPlayerTurn()
    {
        IsPlayerTurn = true;
        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
            unit.ResetAP();
        Debug.Log($"=== ХОД {TurnNumber} — ВАШ ХОД === (Пробел — завершить)");
    }

    void EndPlayerTurn()
    {
        IsPlayerTurn = false;
        Debug.Log($"Ход {TurnNumber}: ход противника...");
        StartCoroutine(EnemyAI.Instance.RunTurn());
    }

    public void OnEnemyTurnDone()
    {
        TurnNumber++;
        StartPlayerTurn();
    }
}
