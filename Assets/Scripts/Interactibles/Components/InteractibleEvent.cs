using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableGlow))]
public class InteractEvent : MonoBehaviour, IInteractible
{
    [SerializeField] private UnityEvent onInteract;

    public void Interact(GameObject interactor) => onInteract?.Invoke();
}
