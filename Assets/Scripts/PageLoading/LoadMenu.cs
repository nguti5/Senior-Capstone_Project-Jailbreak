using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadMenu : MonoBehaviour
{
    public TMP_Text[] slotLabels;

    IEnumerator Start()
    {
        foreach (var label in slotLabels) label.text = "Loading...";
        while (!SaveManager.Loaded) yield return null;
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < slotLabels.Length; i++)
            slotLabels[i].text = SaveManager.HasSave(i) ? SaveManager.Slots[i].name : "Empty";
    }

    public void Load(int slot)
    {
        if (!SaveManager.Loaded)
        {
            Debug.LogWarning("Save data hasn't loaded. Start from title page.");
            return;
        }

        SaveManager.CurrentSlot = slot;
        string next = SaveManager.HasSave(slot) ? "Tutorial" : "RenameField";
        SceneManager.LoadScene(next);
    }

    public void DeleteSlot(int slot)
    {
        if (LegacyCatalogManager.Instance == null) return;
        LegacyCatalogManager.Instance.DeleteSlot(slot, ok => { if (ok) Refresh(); });
    }

    public void Back()
    {
        SceneManager.LoadScene("TitlePage");
    }
}