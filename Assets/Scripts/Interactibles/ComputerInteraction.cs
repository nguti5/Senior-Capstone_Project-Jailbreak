
using UnityEngine;

public class ComputerInteraction : MonoBehaviour, IInteractible
{
    [Header("Computer UI")]
    [SerializeField] private GameObject computerLoginPanel;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteractor;

    public void Interact(GameObject interactor)
    {
        if (computerLoginPanel == null)
        {
            Debug.LogWarning("Computer Login Panel not assigned!");
            return;
        }

        // Open the login screen
        computerLoginPanel.SetActive(true);

        // Disable player movement and interaction
        if (playerController != null)
            playerController.enabled = false;

        if (playerInteractor != null)
            playerInteractor.enabled = false;

        // Unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Grandma's computer login opened.");
    }
}
