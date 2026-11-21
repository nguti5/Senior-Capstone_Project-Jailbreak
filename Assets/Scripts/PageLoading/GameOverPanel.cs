using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteractor;

    [Header("What the button does")]
    [SerializeField] private UnityEvent onContinue;

    private CursorLockMode previousLock;
    private bool previousVisible;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show()   // call from a BattleEncounter's On Player Lost
    {
        if (panel == null || panel.activeSelf) return;

        previousLock = Cursor.lockState;
        previousVisible = Cursor.visible;

        panel.SetActive(true);

        if (playerController != null) playerController.enabled = false;
        if (playerInteractor != null) playerInteractor.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continue()   // the panel button's OnClick
    {
        panel.SetActive(false);

        if (playerController != null) playerController.enabled = true;
        if (playerInteractor != null) playerInteractor.enabled = true;

        Cursor.lockState = previousLock;
        Cursor.visible = previousVisible;

        onContinue?.Invoke();
    }
}