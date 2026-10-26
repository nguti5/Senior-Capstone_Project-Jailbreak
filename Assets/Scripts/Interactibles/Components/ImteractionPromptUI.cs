
using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private GameObject interactionBackground;
    [SerializeField] private float blinkSpeed = 0.5f;

    private string currentMessage = "";
    private float blinkTimer;
    private bool cursorVisible = true;
    private bool promptVisible;

    private void Awake()
    {
        if (promptText == null)
            promptText = GetComponent<TMP_Text>();

        HidePrompt();
    }

    private void Update()
    {
        if (!promptVisible || promptText == null)
            return;

        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkSpeed)
        {
            blinkTimer = 0f;
            cursorVisible = !cursorVisible;
            UpdateText();
        }
    }

    public void ShowPrompt(string message)
    {
        if (promptText == null)
            return;

        currentMessage = "[E] " + message;
        promptVisible = true;
        cursorVisible = true;
        blinkTimer = 0f;

        promptText.enabled = true;
        UpdateText();

        if (interactionBackground != null)
            interactionBackground.SetActive(true);
    }

    public void HidePrompt()
    {
        promptVisible = false;

        if (promptText != null)
            promptText.enabled = false;

        if (interactionBackground != null)
            interactionBackground.SetActive(false);
    }

    private void UpdateText()
    {
        promptText.text = currentMessage +
            (cursorVisible ? " _" : "  ");
    }

    private void OnDisable()
    {
        HidePrompt();
    }
}
