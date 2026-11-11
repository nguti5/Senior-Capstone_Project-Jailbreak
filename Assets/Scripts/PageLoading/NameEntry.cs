using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NameEntry : MonoBehaviour
{
    public TMP_InputField nameInput;

    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Color successColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color errorColor = new Color(0.9f, 0.2f, 0.2f);

    [Header("Rules")]
    [SerializeField] private int maxLength = 20;

    bool saving;

    private void Start()
    {
        ClearFeedback();

        if (nameInput != null)
            nameInput.onValueChanged.AddListener(_ => { if (!saving) ClearFeedback(); });
    }

    public void ConfirmName()
    {
        if (saving) return;

        string newName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            ShowMessage("Please enter a name.", errorColor);
            return;
        }

        if (newName.Length > maxLength)
        {
            ShowMessage($"Name is too long (max {maxLength} characters).", errorColor);
            return;
        }

        if (LegacyCatalogManager.Instance == null)
        {
            Debug.LogWarning("LegacyCatalogManager not found. Start from the TitlePage scene.");
            return;
        }

        int slot = SaveManager.CurrentSlot;
        SlotData previous = SaveManager.Slots[slot];
        SaveManager.Slots[slot] = new SlotData { name = newName };

        saving = true;
        nameInput.interactable = false;
        ShowMessage("Saving...", Color.white);

        LegacyCatalogManager.Instance.SaveSlot(slot, ok =>
        {
            if (ok)
            {
                StartCoroutine(SuccessThenLoad());
            }
            else
            {
                SaveManager.Slots[slot] = previous;
                saving = false;
                nameInput.interactable = true;
                ShowMessage("Couldn't save. Please try again", errorColor);
            }
        });
    }

    private IEnumerator SuccessThenLoad()
    {
        ShowMessage("Saved!", successColor);
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene("Tutorial");
    }

    private void ShowMessage(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.gameObject.SetActive(true);
        feedbackText.color = color;
        feedbackText.text = message;
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void Back()
    {
        SceneManager.LoadScene("LoadGame");
    }
}