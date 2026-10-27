using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NameEntry : MonoBehaviour
{
    public TMP_InputField nameInput;

    bool saving;

    public void ConfirmName()
    {
        if (saving) return;

        string newName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(newName)) return;   // don't save a blank name

        int slot = SaveManager.CurrentSlot;
        SaveManager.Slots[slot] = new SlotData { name = newName };

        if (LegacyCatalogManager.Instance == null)
        {
            Debug.LogWarning("LegacyCatalogManager not found. Start from the TitlePage scene.");
            return;
        }

        saving = true;
        LegacyCatalogManager.Instance.SaveSlot(slot, ok =>
        {
            saving = false;

            if (ok) SceneManager.LoadScene("Tutorial");
            else Debug.LogWarning("Couldn't save. Try again.");
        });
    }
}