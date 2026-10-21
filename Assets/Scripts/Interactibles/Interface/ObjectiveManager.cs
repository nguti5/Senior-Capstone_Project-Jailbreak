using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("When everything is done")]
    public float finishDelay = 1.5f;
    public bool hidePanelWhenDone = true;
    public UnityEvent onAllComplete;

    int completed;   // how many objectives are checked off
    bool started;
    bool finished;
    readonly HashSet<string> owned = new HashSet<string>();

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // Call when the step-by-step tutorial ends
    public void Begin()
    {
        if (started) return;
        started = true;
        completed = 0;
        panel.SetActive(true);

        SkipOwned();
        Refresh();
        CheckFinished();
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

    bool IsActive() => started && !finished && completed < objectives.Count;

    bool IsOwned(Objective o) =>
        !string.IsNullOrEmpty(o.completeOnItemId) && owned.Contains(o.completeOnItemId);

    // if the player already has the needed item, check the objective off immediately
    void SkipOwned()
    {
        while (completed < objectives.Count && IsOwned(objectives[completed]))
            completed++;
    }

    void CompleteCurrent()
    {
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

        for (int i = 0; i < objectives.Count; i++)
        {
            string text = objectives[i].description;

            if (i < completed)
                sb.AppendLine("<color=#7FBF7F>[x]<s> " + text + "</s></color><br><size=10><br></size>");   // done
            else if (i == completed)
                sb.AppendLine("<b>[ ] " + text + "</b><br><size=10><br></size>");                          // current
            else if (showUpcoming)
                // add opacity change color to: 75% visible - #888888BF, 50% visible - #88888880, 30% visible - #8888884D
                sb.AppendLine("<color=#888888>[ ] " + text + "</color><br><size=10><br></size>");        // upcoming
        }

        objectiveText.text = sb.ToString().TrimEnd();
    }
}