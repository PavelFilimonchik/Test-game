using UnityEngine;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public int  TurnNumber   { get; private set; } = 1;
    public bool IsPlayerTurn { get; private set; } = true;

    void Awake() => Instance = this;

    void Start() => StartPlayerTurn();

    void Update()
    {
        if (IsPlayerTurn && Keyboard.current.spaceKey.wasPressedThisFrame)
            PlayerEndTurn();
    }

    void StartPlayerTurn()
    {
        IsPlayerTurn = true;
        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
            unit.ResetAP();
        ResourceManager.Instance.CollectFromBuildings();
        VictoryCondition.Check();
        UIManager.Instance?.UpdateTurn(TurnNumber, true);
        Debug.Log($"=== ХОД {TurnNumber} — ВАШ ХОД === (Пробел или кнопка — завершить)");
    }

    public void PlayerEndTurn()
    {
        if (!IsPlayerTurn) return;
        IsPlayerTurn = false;
        UIManager.Instance?.UpdateTurn(TurnNumber, false);
        Debug.Log($"Ход {TurnNumber}: ход противника...");
        StartCoroutine(EnemyAI.Instance.RunTurn());
    }

    public void OnEnemyTurnDone()
    {
        TurnNumber++;
        StartPlayerTurn();
    }
}
