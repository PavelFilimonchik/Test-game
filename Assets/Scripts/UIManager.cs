using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

[DefaultExecutionOrder(-5)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private TextMeshProUGUI goldText;
    private TextMeshProUGUI woodText;
    private TextMeshProUGUI turnText;
    private GameObject      resultPanel;
    private TextMeshProUGUI resultText;

    void Awake()
    {
        Instance = this;
        BuildUI();
    }

    void BuildUI()
    {
        // EventSystem — без него кнопки не работают
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<InputSystemUIInputModule>();

        // Canvas
        var canvasGO = new GameObject("GameCanvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // HUD: тёмная полоска сверху
        var hud     = MakeRect(canvasGO.transform, "HUD");
        hud.anchorMin        = new Vector2(0, 1);
        hud.anchorMax        = new Vector2(1, 1);
        hud.pivot            = new Vector2(0.5f, 1f);
        hud.anchoredPosition = Vector2.zero;
        hud.sizeDelta        = new Vector2(0, 70);
        hud.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f, 0.88f);

        goldText = MakeText(hud, "Gold", new Vector2(0f,    0f), new Vector2(0.25f, 1f), "Золото: 20");
        woodText = MakeText(hud, "Wood", new Vector2(0.25f, 0f), new Vector2(0.50f, 1f), "Дерево: 10");
        turnText = MakeText(hud, "Turn", new Vector2(0.50f, 0f), new Vector2(0.78f, 1f), "Ход 1 — Ваш");
        turnText.alignment = TextAlignmentOptions.Center;

        // Кнопка "Завершить ход"
        var btnRT       = MakeRect(hud, "EndTurnBtn");
        btnRT.anchorMin = new Vector2(0.81f, 0.1f);
        btnRT.anchorMax = new Vector2(0.99f, 0.9f);
        btnRT.sizeDelta = Vector2.zero;
        btnRT.gameObject.AddComponent<Image>().color = new Color(0.15f, 0.55f, 0.15f);
        var btn = btnRT.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(OnEndTurnClicked);
        var btnLabel = MakeText(btnRT, "Label", Vector2.zero, Vector2.one, "Завершить ход");
        btnLabel.alignment = TextAlignmentOptions.Center;
        btnLabel.fontSize  = 26;

        // Панель победы/поражения (по центру, скрыта)
        var panelRT       = MakeRect(canvasGO.transform, "ResultPanel");
        panelRT.anchorMin = new Vector2(0.3f, 0.4f);
        panelRT.anchorMax = new Vector2(0.7f, 0.6f);
        panelRT.sizeDelta = Vector2.zero;
        panelRT.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        resultPanel = panelRT.gameObject;

        resultText           = MakeText(panelRT, "ResultText", Vector2.zero, Vector2.one, "");
        resultText.fontSize  = 62;
        resultText.fontStyle = FontStyles.Bold;
        resultText.alignment = TextAlignmentOptions.Center;
        resultPanel.SetActive(false);
    }

    void OnEndTurnClicked()
    {
        if (TurnManager.Instance != null && TurnManager.Instance.IsPlayerTurn)
            TurnManager.Instance.PlayerEndTurn();
    }

    public void UpdateResources(int gold, int wood)
    {
        if (goldText) goldText.text = $"Золото: {gold}";
        if (woodText) woodText.text = $"Дерево: {wood}";
    }

    public void UpdateTurn(int turn, bool isPlayer)
    {
        if (turnText) turnText.text = isPlayer ? $"Ход {turn} — Ваш" : $"Ход {turn} — Враг";
    }

    public void ShowResult(bool playerWon)
    {
        resultPanel.SetActive(true);
        resultText.text  = playerWon ? "ПОБЕДА!" : "ПОРАЖЕНИЕ";
        resultText.color = playerWon
            ? new Color(1f, 0.85f, 0f)
            : new Color(0.9f, 0.15f, 0.15f);
        Time.timeScale = 0f;
    }

    // ─── Helpers ─────────────────────────────────────────────

    RectTransform MakeRect(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }

    TextMeshProUGUI MakeText(RectTransform parent, string name,
                             Vector2 anchorMin, Vector2 anchorMax, string text)
    {
        var rt       = MakeRect(parent, name);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = Vector2.zero;
        var tmp      = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = 28;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.margin    = new Vector4(10, 0, 10, 0);
        return tmp;
    }
}
