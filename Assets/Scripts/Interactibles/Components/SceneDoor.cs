using UnityEngine;
using UnityEngine.Events;
 
/// <summary>
/// A door that sends the player to another scene when interacted with.
/// Configure everything in the Inspector, no code changes per door.
/// </summary>
public class SceneDoor : MonoBehaviour, IInteractible
{
    [Header("Destination")]
    [SerializeField] private string targetSceneName;
 
    [Tooltip("ID of the SpawnPoint in the target scene. Leave empty to use the scene's default placement.")]
    [SerializeField] private string targetSpawnId;
 
    [Header("State")]
    [SerializeField] private bool isLocked;
 
    [Header("Events (hook up sounds, fades, UI messages, etc.)")]
    [SerializeField] private UnityEvent onBeforeLoad;
    [SerializeField] private UnityEvent onLockedAttempt;

    [SerializeField] private string unlockedByCheckpoint;
 
    public void Interact(GameObject interactor)
    {
        Debug.Log($"Door '{name}': locked={isLocked}, slot={SaveManager.CurrentSlot}, " +
          $"checkpoint '{unlockedByCheckpoint}' complete={SaveManager.IsComplete(unlockedByCheckpoint)}");

        if (isLocked && !string.IsNullOrEmpty(unlockedByCheckpoint)
        && SaveManager.IsComplete(unlockedByCheckpoint))
        isLocked = false;

        if (isLocked)
        {
            onLockedAttempt?.Invoke();
            return;
        }
 
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning($"{name}: no target scene set.", this);
            return;
        }
 
        onBeforeLoad?.Invoke();
        SceneTransition.Load(targetSceneName, targetSpawnId);
    }
 
    // Callable from other scripts or UnityEvents (e.g. a key pickup)
    public void SetLocked(bool locked) => isLocked = locked;
}
