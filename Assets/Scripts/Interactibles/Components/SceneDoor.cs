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
 
    public void Interact(GameObject interactor)
    {
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
