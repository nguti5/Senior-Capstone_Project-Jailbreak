using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Creates a placeholder gameplay HUD, pause menu and JSON-driven threat documentation.</summary>
public sealed class PlaceholderGameUI : MonoBehaviour
{
    [Serializable]
    private class ThreatEntry
    {
        public string id;
        public string name;
        public string definition;
        public string howItWorks;
    }

    [Serializable]
    private class ThreatCollection
    {
        public ThreatEntry[] threats;
    }

    private static PlaceholderGameUI instance;
    private static readonly Color PanelColor = new Color(0.10f, 0.12f, 0.16f, 0.94f);
    private static readonly Color ButtonColor = new Color(0.25f, 0.30f, 0.36f, 1f);
    private static readonly Color SelectedColor = new Color(0.20f, 0.40f, 0.65f, 1f);

    private Canvas canvas;
    private GameObject hud;
    private GameObject pausePanel;
    private GameObject threatPanel;
    private GameObject documentationPanel;
    private TMP_Text objectiveLabel;
    private TMP_Text noticeLabel;
    private TMP_Text statusLabel;
    private TMP_Text documentationTitle;
    private TMP_Text documentationBody;
    private Transform documentationListContent;
    private Button selectedThreatButton;
    private ThreatCollection documentation;

    private bool paused;
    private bool threatOpen;
    private bool documentationOpen;
    private float savedTimeScale = 1f;
    private CursorLockMode savedCursorLock;
    private bool savedCursorVisible;
    private PlayerController player;
    private bool savedPlayerEnabled;
    private bool cursorSnapshotTaken;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        GameObject host = new GameObject("Placeholder Game UI");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<PlaceholderGameUI>();
        instance.BuildForScene(SceneManager.GetActiveScene());
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ResetInteractionState();
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private static bool IsExistingMenu(string sceneName)
    {
        switch (sceneName)
        {
            case "TitlePage":
            case "Options":
            case "LoadGame":
            case "RenameField":
            case "BattleLoading":
                return true;
            default:
                return false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => BuildForScene(scene);

    private void BuildForScene(Scene scene)
    {
        ResetInteractionState();
        if (canvas != null) Destroy(canvas.gameObject);
        canvas = null;
        hud = null;
        pausePanel = null;
        threatPanel = null;
        documentationPanel = null;
        objectiveLabel = null;
        noticeLabel = null;
        statusLabel = null;
        documentationTitle = null;
        documentationBody = null;
        documentationListContent = null;
        selectedThreatButton = null;
        player = null;
        cursorSnapshotTaken = false;

        if (IsExistingMenu(scene.name)) return;

        EnsureEventSystem();
        GameObject root = new GameObject("Gameplay UI Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        hud = MakePanel("Gameplay HUD", root.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(560f, 196f));
        MakeLabel("PROJECT JAILBREAK  |  PLACEHOLDER HUD", hud.transform, new Vector2(14f, -12f), new Vector2(530f, 32f), 22f);
        statusLabel = MakeLabel("STATUS: ACTIVE", hud.transform, new Vector2(14f, -52f), new Vector2(530f, 28f), 17f);
        objectiveLabel = MakeLabel("OBJECTIVE: Explore the scene (placeholder)", hud.transform, new Vector2(14f, -87f), new Vector2(530f, 28f), 17f);
        noticeLabel = MakeLabel("WASD: Move  |  ESC: Pause  |  TAB: Threats", hud.transform, new Vector2(14f, -124f), new Vector2(530f, 52f), 16f);

        threatPanel = MakePanel("Threat Information", root.transform, Vector2.one, Vector2.one, new Vector2(-20f, -20f), new Vector2(440f, 278f));
        MakeLabel("THREAT INFORMATION", threatPanel.transform, new Vector2(14f, -12f), new Vector2(410f, 34f), 22f);
        MakeLabel("Threat: Not detected (placeholder)\nSeverity: Unknown\nDetails: Pending gameplay integration\nSuggested action: Investigate", threatPanel.transform, new Vector2(14f, -54f), new Vector2(410f, 150f), 18f);
        MakeButton("Close", threatPanel.transform, new Vector2(14f, -222f), new Vector2(175f, 40f), ToggleThreats);
        threatPanel.SetActive(false);

        BuildDocumentationPanel(root.transform);

        pausePanel = MakePanel("Pause Menu", root.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(440f, 382f));
        MakeLabel("PAUSED", pausePanel.transform, new Vector2(25f, -22f), new Vector2(390f, 45f), 30f);
        MakeButton("Resume", pausePanel.transform, new Vector2(45f, -91f), new Vector2(350f, 48f), () => SetPaused(false));
        MakeButton("Threat Information", pausePanel.transform, new Vector2(45f, -151f), new Vector2(350f, 48f), () =>
        {
            SetPaused(false);
            ToggleThreats();
        });
        MakeButton("Documentation", pausePanel.transform, new Vector2(45f, -211f), new Vector2(350f, 48f), OpenDocumentation);
        MakeButton("Main Menu", pausePanel.transform, new Vector2(45f, -271f), new Vector2(350f, 48f), () =>
        {
            SetPaused(false);
            SceneManager.LoadScene("TitlePage");
        });
        pausePanel.SetActive(false);
    }

    private void BuildDocumentationPanel(Transform parent)
    {
        documentation = LoadDocumentation();
        documentationPanel = MakePanel("Documentation", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1150f, 750f));
        MakeLabel("CYBERSECURITY DOCUMENTATION", documentationPanel.transform, new Vector2(30f, -24f), new Vector2(1060f, 44f), 30f);
        MakeLabel("THREATS", documentationPanel.transform, new Vector2(30f, -91f), new Vector2(300f, 32f), 22f);
        MakeLabel("DEFINITION & HOW IT WORKS", documentationPanel.transform, new Vector2(380f, -91f), new Vector2(700f, 32f), 22f);

        GameObject listViewport = MakePanel("Threat List Viewport", documentationPanel.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -137f), new Vector2(315f, 500f));
        listViewport.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.11f, 0.8f);
        listViewport.AddComponent<RectMask2D>();

        GameObject listContent = new GameObject("Threat List Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        listContent.transform.SetParent(listViewport.transform, false);
        RectTransform contentRect = listContent.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        VerticalLayoutGroup layout = listContent.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        ContentSizeFitter fitter = listContent.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        documentationListContent = listContent.transform;

        ScrollRect scroll = listViewport.AddComponent<ScrollRect>();
        scroll.content = contentRect;
        scroll.viewport = listViewport.GetComponent<RectTransform>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 30f;

        GameObject detailBackground = MakePanel("Documentation Details", documentationPanel.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(380f, -137f), new Vector2(740f, 500f));
        detailBackground.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.11f, 0.8f);
        documentationTitle = MakeLabel("Select a threat", detailBackground.transform, new Vector2(20f, -20f), new Vector2(700f, 50f), 28f);
        GameObject detailViewport = MakePanel("Detail Scroll Viewport", detailBackground.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -86f), new Vector2(700f, 394f));
        detailViewport.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        detailViewport.AddComponent<RectMask2D>();
        documentationBody = MakeLabel("Select a threat to read its documentation.", detailViewport.transform, Vector2.zero, new Vector2(700f, 394f), 20f);
        documentationBody.rectTransform.anchorMin = new Vector2(0f, 1f);
        documentationBody.rectTransform.anchorMax = new Vector2(1f, 1f);
        documentationBody.rectTransform.sizeDelta = new Vector2(0f, 394f);
        documentationBody.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        ScrollRect detailScroll = detailViewport.AddComponent<ScrollRect>();
        detailScroll.content = documentationBody.rectTransform;
        detailScroll.viewport = detailViewport.GetComponent<RectTransform>();
        detailScroll.horizontal = false;
        detailScroll.vertical = true;
        detailScroll.movementType = ScrollRect.MovementType.Clamped;
        detailScroll.scrollSensitivity = 30f;

        MakeButton("Back to Pause Menu", documentationPanel.transform, new Vector2(840f, -665f), new Vector2(280f, 52f), CloseDocumentation);
        PopulateThreatList();
        documentationPanel.SetActive(false);
    }

    private static ThreatCollection LoadDocumentation()
    {
        TextAsset asset = Resources.Load<TextAsset>("CybersecurityThreats");
        if (asset == null)
        {
            Debug.LogWarning("Documentation JSON missing: Assets/Resources/CybersecurityThreats.json");
            return new ThreatCollection { threats = new ThreatEntry[0] };
        }
        try
        {
            ThreatCollection result = JsonUtility.FromJson<ThreatCollection>(asset.text);
            if (result != null && result.threats != null) return result;
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not parse CybersecurityThreats.json: " + ex.Message);
        }
        return new ThreatCollection { threats = new ThreatEntry[0] };
    }

    private void PopulateThreatList()
    {
        if (documentation.threats.Length == 0)
        {
            documentationTitle.text = "No documentation available";
            documentationBody.text = "Add valid threat entries to Assets/Resources/CybersecurityThreats.json.";
            return;
        }

        bool selectedFirst = false;
        foreach (ThreatEntry entry in documentation.threats)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.name)) continue;
            ThreatEntry capturedEntry = entry;
            GameObject item = new GameObject("Threat - " + entry.name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            item.transform.SetParent(documentationListContent, false);
            item.GetComponent<Image>().color = ButtonColor;
            LayoutElement element = item.GetComponent<LayoutElement>();
            element.preferredHeight = 56f;
            Button button = item.GetComponent<Button>();
            MakeLabel(entry.name, item.transform, Vector2.zero, new Vector2(300f, 56f), 20f);
            TMP_Text caption = item.GetComponentInChildren<TMP_Text>();
            caption.rectTransform.anchorMin = Vector2.zero;
            caption.rectTransform.anchorMax = Vector2.one;
            caption.rectTransform.offsetMin = new Vector2(12f, 0f);
            caption.rectTransform.offsetMax = new Vector2(-12f, 0f);
            caption.alignment = TextAlignmentOptions.MidlineLeft;
            button.onClick.AddListener(() => SelectThreat(capturedEntry, button));
            if (!selectedFirst)
            {
                SelectThreat(entry, button);
                selectedFirst = true;
            }
        }
    }

    private void SelectThreat(ThreatEntry entry, Button button)
    {
        if (selectedThreatButton != null) selectedThreatButton.GetComponent<Image>().color = ButtonColor;
        selectedThreatButton = button;
        selectedThreatButton.GetComponent<Image>().color = SelectedColor;
        documentationTitle.text = entry.name;
        documentationBody.text = "DEFINITION\n" + SafeText(entry.definition) + "\n\nHOW IT WORKS\n" + SafeText(entry.howItWorks);
        documentationBody.rectTransform.anchoredPosition = Vector2.zero;
    }

    private static string SafeText(string value) => string.IsNullOrWhiteSpace(value) ? "Not yet documented." : value;

    private void OpenDocumentation()
    {
        if (!paused || documentationPanel == null) return;
        documentationOpen = true;
        pausePanel.SetActive(false);
        documentationPanel.SetActive(true);
    }

    private void CloseDocumentation()
    {
        if (!documentationOpen) return;
        documentationOpen = false;
        documentationPanel.SetActive(false);
        if (paused && pausePanel != null) pausePanel.SetActive(true);
    }

    private void Update()
    {
        if (canvas == null || Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (documentationOpen) CloseDocumentation();
            else if (threatOpen && !paused) ToggleThreats();
            else SetPaused(!paused);
        }
        if (Keyboard.current.tabKey.wasPressedThisFrame && !paused)
            ToggleThreats();
        if (statusLabel != null)
            statusLabel.text = "STATUS: ACTIVE  |  SCENE: " + SceneManager.GetActiveScene().name;
    }

    private void ToggleThreats()
    {
        if (paused || threatPanel == null) return;
        if (!cursorSnapshotTaken) SnapshotCursor();
        threatOpen = !threatOpen;
        threatPanel.SetActive(threatOpen);
        SetCursorForUI(threatOpen);
        SetPlayerInputEnabled(!threatOpen);
        if (!threatOpen) ReleaseCursorSnapshot();
    }

    private void SetPaused(bool value)
    {
        if (pausePanel == null || paused == value) return;
        if (value)
        {
            if (!cursorSnapshotTaken) SnapshotCursor();
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            paused = true;
            SetPlayerInputEnabled(false);
            SetCursorForUI(true);
        }
        else
        {
            documentationOpen = false;
            if (documentationPanel != null) documentationPanel.SetActive(false);
            RestorePauseState();
            if (!threatOpen) ReleaseCursorSnapshot();
        }
        pausePanel.SetActive(paused && !documentationOpen);
        hud.SetActive(!paused);
        threatPanel.SetActive(!paused && threatOpen);
    }

    private void SnapshotCursor()
    {
        savedCursorLock = Cursor.lockState;
        savedCursorVisible = Cursor.visible;
        player = FindFirstObjectByType<PlayerController>();
        savedPlayerEnabled = player != null && player.enabled;
        cursorSnapshotTaken = true;
    }

    private void ReleaseCursorSnapshot()
    {
        cursorSnapshotTaken = false;
        player = null;
    }

    private void RestorePauseState()
    {
        if (!paused) return;
        Time.timeScale = savedTimeScale;
        paused = false;
        SetPlayerInputEnabled(!threatOpen);
        SetCursorForUI(threatOpen);
    }

    private void ResetInteractionState()
    {
        if (paused) RestorePauseState();
        if (cursorSnapshotTaken)
        {
            if (player != null) player.enabled = savedPlayerEnabled;
            Cursor.lockState = savedCursorLock;
            Cursor.visible = savedCursorVisible;
        }
        paused = false;
        threatOpen = false;
        documentationOpen = false;
        cursorSnapshotTaken = false;
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        if (player != null) player.enabled = enabled && savedPlayerEnabled;
    }

    private void SetCursorForUI(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : (cursorSnapshotTaken ? savedCursorLock : Cursor.lockState);
        Cursor.visible = visible || (cursorSnapshotTaken && savedCursorVisible);
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        GameObject obj = new GameObject("UI EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        DontDestroyOnLoad(obj);
    }

    private static GameObject MakePanel(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        obj.GetComponent<Image>().color = PanelColor;
        return obj;
    }

    private static TMP_Text MakeLabel(string value, Transform parent, Vector2 position, Vector2 size, float fontSize)
    {
        GameObject obj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TextMeshProUGUI label = obj.GetComponent<TextMeshProUGUI>();
        label.text = value;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;
        return label;
    }

    private static void MakeButton(string caption, Transform parent, Vector2 position, Vector2 size, Action callback)
    {
        GameObject obj = new GameObject(caption, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        obj.GetComponent<Image>().color = ButtonColor;
        obj.GetComponent<Button>().onClick.AddListener(() => callback());
        TMP_Text text = MakeLabel(caption, obj.transform, Vector2.zero, size, 20f);
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        text.alignment = TextAlignmentOptions.Center;
    }

    public static void SetObjective(string objective)
    {
        if (instance != null && instance.objectiveLabel != null)
            instance.objectiveLabel.text = "OBJECTIVE: " + objective;
    }

    public static void SetNotice(string notice)
    {
        if (instance != null && instance.noticeLabel != null)
            instance.noticeLabel.text = notice;
    }
}
