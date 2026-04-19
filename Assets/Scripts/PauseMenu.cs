using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.Escape))
#endif
            TogglePause();
    }

    void CreatePauseMenu()
    {
        // Без EventSystem кнопки не реагируют на клики
        if (FindObjectOfType<EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
        }

        var canvasGO = new GameObject("PauseCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // pausePanel = полноэкранный тёмный оверлей, который и скрываем/показываем
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasGO.transform, false);
        var bgImg = pausePanel.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.7f);
        var bgRect = pausePanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        // Центральная панель с кнопками внутри оверлея
        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(pausePanel.transform, false);
        var contentImg = contentGO.AddComponent<Image>();
        contentImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        var contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.3f, 0.3f);
        contentRect.anchorMax = new Vector2(0.7f, 0.7f);
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;

        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(contentGO.transform, false);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "ПАУЗА";
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.raycastTarget = false;
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.72f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;

        var resumeBtn = CreateButton(contentGO.transform, "ResumeBtn", "Продолжить",
            new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.68f), new Color(0.2f, 0.6f, 0.2f));
        resumeBtn.onClick.AddListener(TogglePause);

        var restartBtn = CreateButton(contentGO.transform, "RestartBtn", "Заново",
            new Vector2(0.1f, 0.26f), new Vector2(0.9f, 0.46f), new Color(0.6f, 0.5f, 0.2f));
        restartBtn.onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        var quitBtn = CreateButton(contentGO.transform, "QuitBtn", "Выйти",
            new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.24f), new Color(0.6f, 0.2f, 0.2f));
        quitBtn.onClick.AddListener(() => {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        pausePanel.SetActive(false);
    }

    Button CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);

        // Image первым — создаёт RectTransform
        var img = btnGO.AddComponent<Image>();
        img.color = color;

        var rect = btnGO.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        var btn = btnGO.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.4f;
        colors.pressedColor = color * 0.6f;
        colors.selectedColor = color;
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        var textGO = new GameObject("Label");
        textGO.transform.SetParent(btnGO.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false; // текст не должен перехватывать клики
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
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
