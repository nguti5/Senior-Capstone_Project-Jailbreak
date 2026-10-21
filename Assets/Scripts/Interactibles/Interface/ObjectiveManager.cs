using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class Objective
{
    public string id;                 // e.g. "get_key"
    [TextArea(1, 3)]
    public string description;        // e.g. "Find the key"
    public string completeOnItemId;   // optional: item that completes it, e.g. "key"
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public GameObject panel;
    public TMP_Text objectiveText;
    public List<Objective> objectives;

    int current = -1;
    bool advancing;
    readonly HashSet<string> owned = new HashSet<string>();

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // Call this when the step-by-step tutorial ends
    public void Begin()
    {
        current = 0;
        Show();
    }

    // Call from your inventory code when an item is confirmed added
    public void NotifyItemAcquired(string itemId)
    {
        owned.Add(itemId);

        if (CurrentIsValid() && objectives[current].completeOnItemId == itemId)
            StartAdvance();
    }

    // Callable from UnityEvents, e.g. SceneDoor's On Before Load
    public void Complete(string objectiveId)
    {
        if (CurrentIsValid() && objectives[current].id == objectiveId)
            StartAdvance();
    }

    bool CurrentIsValid() => current >= 0 && current < objectives.Count && !advancing;

    void Show()
    {
        if (current >= objectives.Count)
        {
            panel.SetActive(false); // all done
            return;
        }

        panel.SetActive(true);
        objectiveText.text = objectives[current].description;

        // if the player already has the item, finish it right away
        string itemId = objectives[current].completeOnItemId;
        if (!string.IsNullOrEmpty(itemId) && owned.Contains(itemId))
            StartAdvance();
    }

    void StartAdvance()
    {
        if (!advancing) StartCoroutine(Advance());
    }

    IEnumerator Advance()
    {
        advancing = true;
        objectiveText.text = "<s>" + objectives[current].description + "</s>"; // strikethrough
        yield return new WaitForSecondsRealtime(1.5f);
        advancing = false;
        current++;
        Show();
    }
}