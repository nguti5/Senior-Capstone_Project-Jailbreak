using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ComputerInteraction : MonoBehaviour, IInteractible
{
    [Header("Computer UI")]
    [SerializeField] private GameObject computerLoginPanel;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Password")]
    [SerializeField] private string correctPassword = "Peenut";
    [SerializeField] private float closeDelay = 1.5f;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteractor;

    [Header("Objective")]
    [SerializeField] private string objectiveId = "unlock_computer";

    private bool unlocked;
    private bool busy;
    private CursorLockMode previousLock;
    private bool previousVisible;

    private void Start()
    {
        unlocked = !string.IsNullOrEmpty(objectiveId) && SaveManager.IsComplete(objectiveId);

        if (passwordInput != null)
            passwordInput.onSubmit.AddListener(_ => Submit());
    }

    public void Interact(GameObject interactor)
    {
        if (computerLoginPanel == null)
        {
            Debug.LogWarning("Computer Login Panel not assigned!");
            return;
        }

        if (busy || computerLoginPanel.activeSelf) return;

        if (unlocked)
        {
            if (ObjectiveManager.Instance != null)
                ObjectiveManager.Instance.RunAllCompleteActions();
            return;
        }

        Open();
    }

    private void Open()
    {
        previousLock = Cursor.lockState;
        previousVisible = Cursor.visible;

        computerLoginPanel.SetActive(true);
        SetFeedback("");

        if (passwordInput != null)
        {
            passwordInput.text = "";
            passwordInput.ActivateInputField();
        }

        // Disable player movement and interaction
        if (playerController != null) playerController.enabled = false;
        if (playerInteractor != null) playerInteractor.enabled = false;

        // Unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Grandma's computer login opened.");
    }

    public void Close() // add on close button
    {
        computerLoginPanel.SetActive(false);

        if (playerController != null) playerController.enabled = true;
        if (playerInteractor != null) playerInteractor.enabled = true;

        Cursor.lockState = previousLock;
        Cursor.visible = previousVisible;
    }

    public void Submit()   // add on submit button
    {
        Debug.Log("Submit clicked");

        if (passwordInput == null)
        {
            Debug.LogError("Password Input isn't assigned on ComputerInteraction!");
            return;
        }
        if (busy)
        {
            Debug.Log("Submit ignored: busy");
            return;
        }
        if (unlocked)
        {
            Debug.Log("Submit ignored: already unlocked (objective '" + objectiveId + "' is saved as complete)");
            return;
        }

        string attempt = passwordInput.text.Trim();
        Debug.Log($"Checking '{attempt}' against '{correctPassword}'");

        if (string.Equals(attempt, correctPassword, StringComparison.OrdinalIgnoreCase))
            StartCoroutine(Success());
        else
            StartCoroutine(Failure());
    }

    private IEnumerator Success()
    {
        busy = true;
        unlocked = true;
        SetFeedback("Access granted!");

        if (ObjectiveManager.Instance != null && !string.IsNullOrEmpty(objectiveId))
            ObjectiveManager.Instance.Complete(objectiveId);

        yield return new WaitForSecondsRealtime(1f);

        Close();
        busy = false;
    }

    private IEnumerator Failure()
    {
        busy = true;
        SetFeedback("Incorrect password. Maybe there's a clue nearby...");

        yield return new WaitForSecondsRealtime(closeDelay);

        Close();
        busy = false;
    }

    private void SetFeedback(string message)
    {
        if (feedbackText == null)
        {
            Debug.LogError("Feedback Text isn't assigned!");
            return;
        }

        feedbackText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        feedbackText.text = message;

        Debug.Log($"Feedback set on '{feedbackText.name}' (parent: '{feedbackText.transform.parent?.name}') " +
                $"text='{message}' | activeInHierarchy={feedbackText.gameObject.activeInHierarchy} | " +
                $"alpha={feedbackText.color.a} | size={feedbackText.rectTransform.rect.size} | closeDelay={closeDelay}");
    }
}
