using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Authored guidance only; never generates answers or completes objectives.</summary>
public sealed class AssistantGuidance
{
    [Serializable] public class Entry
    {
        public string scene;
        public string objectiveId;
        public string threatId;
        public string[] hints;
        public string[] clues;
    }
    [Serializable] public class Content
    {
        public float offerAfterSeconds = 60f;
        public Entry[] entries;
    }

    private readonly Content content;
    private readonly Dictionary<string, int> positions = new Dictionary<string, int>();
    private string progressKey;
    private float waiting;
    private bool offered;
    public string ThreatId { get; private set; }

    public AssistantGuidance()
    {
        var asset = Resources.Load<TextAsset>("AssistantGuidance");
        try
        {
            content = asset == null ? null : JsonUtility.FromJson<Content>(asset.text);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Assistant guidance could not be loaded: " + ex.Message);
        }
        if (content == null || content.entries == null)
            content = new Content { entries = new Entry[0] };
    }

    public void SetThreat(string id) => ThreatId = id;

    public bool Tick(string scene, string objectiveId, float elapsed)
    {
        string key = scene + "/" + objectiveId;
        if (key != progressKey)
        {
            progressKey = key;
            waiting = 0f;
            offered = false;
        }
        if (string.IsNullOrEmpty(objectiveId) || offered) return false;
        waiting += elapsed;
        if (waiting < Mathf.Max(1f, content.offerAfterSeconds)) return false;
        offered = true;
        return true;
    }

    private Entry Find(string scene, string objectiveId, bool clue)
    {
        Entry fallback = null;
        foreach (var entry in content.entries)
        {
            if (entry == null) continue;
            if (clue)
            {
                if (!string.IsNullOrEmpty(ThreatId) && entry.threatId == ThreatId)
                    return entry;
                continue;
            }
            if (entry.scene != scene) continue;
            if (!string.IsNullOrEmpty(objectiveId) && entry.objectiveId == objectiveId)
                return entry;
            if (string.IsNullOrEmpty(entry.objectiveId)) fallback = entry;
        }
        return fallback;
    }

    public string Request(string scene, string objectiveId, bool clue)
    {
        Entry entry = Find(scene, objectiveId, clue);
        string[] messages = entry == null ? null : (clue ? entry.clues : entry.hints);
        if (messages == null || messages.Length == 0)
            return clue
                ? "No threat has been identified here yet. Investigate suspicious objects and compare the evidence with the cybersecurity documentation."
                : "No specific hint is available here yet. Review your current objective, inspect nearby objects, and try E when an interaction prompt appears.";
        string key = scene + "/" + (clue ? ThreatId : objectiveId) + "/" + clue;
        positions.TryGetValue(key, out int index);
        // Remain on the strongest hint instead of wrapping back to the first.
        index = Mathf.Min(index, messages.Length - 1);
        positions[key] = index + 1;
        waiting = 0f;
        offered = false;
        return (clue ? "CLUE" : "HINT") + " " + (index + 1) + "/" + messages.Length +
            "\n" + messages[index];
    }
}
