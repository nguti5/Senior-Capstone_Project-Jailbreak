
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingPortalInteraction : MonoBehaviour, IInteractible
{
    [Header("Fishing UI")]
    [SerializeField] private GameObject fishingGamePanel;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteractor;

    private bool gameOpen;

    private void Start()
    {
        if (fishingGamePanel != null)
            fishingGamePanel.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        if (gameOpen)
            return;

        OpenFishingGame();
    }

    private void OpenFishingGame()
    {
        if (fishingGamePanel == null)
        {
            Debug.LogError("Fishing Game Panel is not assigned!");
            return;
        }

        gameOpen = true;

        if (playerController != null)
            playerController.enabled = false;

        if (playerInteractor != null)
            playerInteractor.enabled = false;

        fishingGamePanel.SetActive(true);

        PlaceholderGameUI.SetHUDVisible(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Fishing minigame opened!");
    }

    private void Update()
    {
        if (!gameOpen || Keyboard.current == null)
            return;

        if (Keyboard.current.xKey.wasPressedThisFrame)
            CloseFishingGame();
    }

    public void CloseFishingGame()
    {
        if (!gameOpen)
            return;

        gameOpen = false;

        fishingGamePanel.SetActive(false);

        PlaceholderGameUI.SetHUDVisible(true);

        // Restore normal gameplay.
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = true;

        if (playerInteractor != null)
            playerInteractor.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Fishing minigame closed. Player controls restored!");
    }
}
