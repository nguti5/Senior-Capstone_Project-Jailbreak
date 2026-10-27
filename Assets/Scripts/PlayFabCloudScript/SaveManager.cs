using System;
using UnityEngine;

[Serializable]
public class SlotData
{
    public string name;
    public bool tutorialDone;   // add more checkpoints as needed
    public int objectiveIndex;
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
}