using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private InputActionReference interactionReference;

    private void OnEnable()
    {
        interactionReference.action.Enable();
        interactionReference.action.started += PlayerInteracted;
    }

    private void OnDisable()
    {
        interactionReference.action.started -= PlayerInteracted;
        interactionReference.action.Disable();
    }

    private void PlayerInteracted(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance)) return;

        if (hitInfo.collider.TryGetComponent(out IInteractible interactible))
        {
            interactible.Interact();
        }
    }
}