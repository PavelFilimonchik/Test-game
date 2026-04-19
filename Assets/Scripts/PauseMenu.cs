using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private GameObject pausePanel;
    private bool isPaused = false;

    void Start()
    {
        CreatePauseMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape нажат!");
            TogglePause();
        }
    }

    void CreatePauseMenu()
    {
        var canvasGO = new GameObject("PauseCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.7f);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasGO.transform);
        var panelImg = pausePanel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        var panelRect = pausePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.35f);
        panelRect.anchorMax = new Vector2(0.7f, 0.65f);
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(pausePanel.transform);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "ПАУЗА";
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;

        var resumeBtn = CreateButton(pausePanel.transform, "ResumeBtn", "Продолжить",
            new Vector2(0.2f, 0.4f), new Vector2(0.8f, 0.55f), new Color(0.2f, 0.6f, 0.2f));
        resumeBtn.onClick.AddListener(TogglePause);

        var restartBtn = CreateButton(pausePanel.transform, "RestartBtn", "Заново",
            new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.35f), new Color(0.6f, 0.5f, 0.2f));
        restartBtn.onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        var quitBtn = CreateButton(pausePanel.transform, "QuitBtn", "Выйти",
            new Vector2(0.2f, 0f), new Vector2(0.8f, 0.15f), new Color(0.6f, 0.2f, 0.2f));
        quitBtn.onClick.AddListener(() => {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });

        pausePanel.SetActive(false);
    }

    Button CreateButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent);

        // Image добавляется первым — он автоматически создаёт RectTransform
        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = color;

        var btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = anchorMin;
        btnRect.anchorMax = anchorMax;
        btnRect.sizeDelta = Vector2.zero;
        btnRect.anchoredPosition = Vector2.zero;

        var btn = btnGO.AddComponent<Button>();

        var textGO = new GameObject("Label");
        textGO.transform.SetParent(btnGO.transform);
        var btnText = textGO.AddComponent<TextMeshProUGUI>();
        btnText.text = text;
        btnText.fontSize = 28;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        return btn;
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "Пауза" : "Продолжили");
    }
}
