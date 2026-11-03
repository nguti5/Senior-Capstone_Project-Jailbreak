using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotData
{
    public string name;
    public bool tutorialDone;
    public string resumeScene;
    public List<string> completed = new List<string>();
}

public static class SaveManager
{
    public const int SlotCount = 3;
    public static readonly SlotData[] Slots = new SlotData[SlotCount];
    public static bool Loaded;

    public static int CurrentSlot
    {
        get => PlayerPrefs.GetInt("CurrentSlot", 0);
        set => PlayerPrefs.SetInt("CurrentSlot", value);
    }

    public static bool HasSave(int slot) =>
        Slots[slot] != null && !string.IsNullOrEmpty(Slots[slot].name);

    static SlotData Current =>
        (CurrentSlot >= 0 && CurrentSlot < SlotCount) ? Slots[CurrentSlot] : null;

    public static bool IsComplete(string id)
    {
        SlotData s = Current;
        return s != null && s.completed != null && s.completed.Contains(id);
    }

    public static void MarkComplete(string id)
    {
        SlotData s = Current;
        if (s == null || string.IsNullOrEmpty(id)) return;
        if (s.completed == null) s.completed = new List<string>();
        if (s.completed.Contains(id)) return;

        s.completed.Add(id);
        Debug.Log("Checkpoint reached: " + id);
        SaveCurrent();

        if (LegacyCatalogManager.Instance != null)
            LegacyCatalogManager.Instance.SaveSlot(CurrentSlot, ok =>
            {
                if (!ok) Debug.LogWarning("Couldn't save checkpoint: " + id);
            });
    }

    public static void SetResumeScene(string sceneName)
    {
        Debug.Log($"SetResumeScene called: scene={sceneName}, slot={CurrentSlot}, slotData exists={Current != null}");
        SlotData s = Current;
        if (s == null || s.resumeScene == sceneName) return;

        s.resumeScene = sceneName;
        Debug.Log("Resume scene set to: " + sceneName);
        SaveCurrent();
    }
    
    public static string ResumeScene()
    {
        SlotData s = Current;
        return (s != null && !string.IsNullOrEmpty(s.resumeScene)) ? s.resumeScene : "Tutorial";
    }

    static void SaveCurrent()
    {
        if (LegacyCatalogManager.Instance == null) return;

        LegacyCatalogManager.Instance.SaveSlot(CurrentSlot, ok =>
        {
            if (!ok) Debug.LogWarning("Couldn't save progress.");
        });
    }
}