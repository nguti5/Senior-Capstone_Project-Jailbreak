using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableGlow))]
public class MailPileInteraction : MonoBehaviour, IInteractible
{
    [Serializable]
    public class EmailEntry
    {
        public Sprite image;
        public bool isScam;   // tick this on exactly one email
    }

    [Header("UI")]
    [SerializeField] private GameObject mailPanel;
    [SerializeField] private Image emailImage;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Color successColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color errorColor = new Color(0.9f, 0.2f, 0.2f);

    [Header("Emails")]
    [SerializeField] private EmailEntry[] emails;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteractor;

    [Header("Objective")]
    [SerializeField] private string objectiveId = "l1_destroy_scam";

    [SerializeField] private UnityEvent onScamDestroyed;

    private int current;
    private bool solved;
    private bool busy;
    private CursorLockMode previousLock;
    private bool previousVisible;

    private void Start()
    {
        if (mailPanel != null) mailPanel.SetActive(false);
        solved = !string.IsNullOrEmpty(objectiveId) && SaveManager.IsComplete(objectiveId);
    }

    private void Update()
    {
        // left and right arrow keys flip through the emails while the panel is open
        if (mailPanel == null || !mailPanel.activeSelf || Keyboard.current == null) return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) Next();
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) Previous();
    }

    public void Interact(GameObject interactor)
    {
        if (mailPanel == null || emails == null || emails.Length == 0)
        {
            Debug.LogWarning("Mail panel or emails aren't set up!");
            return;
        }

        if (solved || busy || mailPanel.activeSelf) return;

        Open();
    }

    private void Open()
    {
        previousLock = Cursor.lockState;
        previousVisible = Cursor.visible;

        mailPanel.SetActive(true);
        Show(0);

        if (playerController != null) playerController.enabled = false;
        if (playerInteractor != null) playerInteractor.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()   // hook to a Close button
    {
        mailPanel.SetActive(false);

        if (playerController != null) playerController.enabled = true;
        if (playerInteractor != null) playerInteractor.enabled = true;

        Cursor.lockState = previousLock;
        Cursor.visible = previousVisible;
    }

    public void Next() => Show((current + 1) % emails.Length);
    public void Previous() => Show((current - 1 + emails.Length) % emails.Length);

    private void Show(int index)
    {
        current = index;
        emailImage.sprite = emails[current].image;
        if (counterText != null) counterText.text = (current + 1) + " / " + emails.Length;
        SetFeedback("", Color.white);
    }

    public void DestroyCurrent()   // hook to the Destroy button
    {
        if (busy || solved) return;

        if (emails[current].isScam) StartCoroutine(Success());
        else StartCoroutine(WrongPick());
    }

    private IEnumerator Success()
    {
        busy = true;
        solved = true;
        SetFeedback("Scam destroyed! Nice catch.", successColor);

        if (ObjectiveManager.Instance != null && !string.IsNullOrEmpty(objectiveId))
            ObjectiveManager.Instance.Complete(objectiveId);

        yield return new WaitForSecondsRealtime(1.5f);

        Close();
        busy = false;
        onScamDestroyed?.Invoke();
    }

    private IEnumerator WrongPick()
    {
        busy = true;
        SetFeedback("That one looks real. Look more closely...", errorColor);

        yield return new WaitForSecondsRealtime(2f);

        SetFeedback("", Color.white);
        busy = false;
    }

    private void SetFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        feedbackText.color = color;
        feedbackText.text = message;
    }
}