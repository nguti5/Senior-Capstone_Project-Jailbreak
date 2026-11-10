using UnityEngine;
using UnityEngine.Events;

public class InteractEvent : MonoBehaviour, IInteractible
{
    [SerializeField] private UnityEvent onInteract;

    public void Interact(GameObject interactor) => onInteract?.Invoke();
}
