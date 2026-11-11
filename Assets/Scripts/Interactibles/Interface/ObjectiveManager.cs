using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

[Serializable]
public class Objective
{
    public string id;                 // e.g. "get_key"
    [TextArea(1, 3)]
    public string description;        // e.g. "Find the key"
    public string completeOnItemId;   // optional: item in inventory that completes it, e.g. "tutorial_key"
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text objectiveText;

    [Header("Objectives")]
    public List<Objective> objectives;
    [Tooltip("Show future objectives greyed out. Off = they stay hidden until reached.")]
    public bool showUpcoming = false;

    [Header("Minimize")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Key toggleKey = Key.O;
    [SerializeField] private float padding = 24f;

    [Header("When everything is done")]
    public float finishDelay = 1.5f;
    public bool hidePanelWhenDone = true;
    public UnityEvent onAllComplete;

    int completed;   // how many objectives are checked off
    bool started;
    bool finished;
    readonly HashSet<string> owned = new HashSet<string>();
    public bool beginOnStart = false;
    private bool minimized;
    private static bool minimizedPreference;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    void Start()
    {
        if (beginOnStart) Begin();
    }

    // Call when the step-by-step tutorial ends
    public void Begin()
    {
        if (started) return;
        started = true;
        minimized = minimizedPreference;
        completed = 0;
        panel.SetActive(true);

        while (completed < objectives.Count && SaveManager.IsComplete(objectives[completed].id))
        {
            completed++;
        }

        if (completed >= objectives.Count)
        {
            finished = true;
            if (hidePanelWhenDone) panel.SetActive(false);
            return;
        }

        SkipOwned();
        Refresh();
        CheckFinished();
    }

    public void Update()
    {
        if (!started || finished || Keyboard.current == null) return;
        if (Time.timeScale == 0f) return;

        if (EventSystem.current != null)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.TryGetComponent(out TMP_InputField field) && field.isFocused)
                return;
        }

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
            SetMinimized(!minimized);
    }

    public void SetMinimized(bool value)
    {
        minimized = value;
        minimizedPreference = value;
        Refresh();
    }

    // Call from your inventory code when an item is confirmed added
    public void NotifyItemAcquired(string itemId)
    {
        owned.Add(itemId);

        if (IsActive() && objectives[completed].completeOnItemId == itemId)
            CompleteCurrent();
    }

    // Callable from UnityEvents, e.g. SceneDoor's On Before Load
    public void Complete(string objectiveId)
    {
        if (IsActive() && objectives[completed].id == objectiveId)
            CompleteCurrent();
    }

    public string CurrentObjectiveId => IsActive() ? objectives[completed].id : null;

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    bool IsActive() => started && !finished && completed < objectives.Count;

    bool IsOwned(Objective o) =>
        !string.IsNullOrEmpty(o.completeOnItemId) && owned.Contains(o.completeOnItemId);

    // if the player already has the needed item, check the objective off immediately
    void SkipOwned()
    {
        while (completed < objectives.Count && IsOwned(objectives[completed]))
        {
            SaveManager.MarkComplete(objectives[completed].id);
            completed++;
        }
    }

    void CompleteCurrent()
    {
        SaveManager.MarkComplete(objectives[completed].id);
        completed++;
        SkipOwned();
        Refresh();
        CheckFinished();
    }

    void CheckFinished()
    {
        if (completed >= objectives.Count && !finished)
        {
            finished = true;
            StartCoroutine(FinishAfterDelay());
        }
    }

    IEnumerator FinishAfterDelay()
    {
        yield return new WaitForSecondsRealtime(finishDelay);   // lets the player see the last box checked
        if (hidePanelWhenDone) panel.SetActive(false);
        onAllComplete?.Invoke();
    }

    void Refresh()
    {
        var sb = new StringBuilder();
        // color = #90d1eb
        sb.AppendLine("<size=75%><color=#90d1eb>OBJECTIVES  [" + toggleKey + "]</color></size>");

        if (minimized)
        {
            // just the current item
            string current = completed < objectives.Count
                ? objectives[completed].description
                : "All done!";
            sb.AppendLine("<b>" + current + "</b>");
        }
        else
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                string text = objectives[i].description;

                if (i < completed)
                    // color = #7FBF7F
                    sb.AppendLine("<color=#7FBF7F><s>[x] " + text + "</s></color>");
                else if (i == completed)
                    sb.AppendLine("<b>[ ] " + text + "</b>");
                else if (showUpcoming)
                    // color = #d4d4d4
                    sb.AppendLine("<color=#d4d4d4>[ ] " + text + "</color>");
            }
        }

        objectiveText.text = sb.ToString().TrimEnd();
        FitPanel();
    }

    void FitPanel()
    {
        if (panelRect == null) return;

        objectiveText.ForceMeshUpdate();
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, objectiveText.preferredHeight + padding);
    }

    public void RunAllCompleteActions()
    {
        if (finished)
            onAllComplete?.Invoke();
    }
}
