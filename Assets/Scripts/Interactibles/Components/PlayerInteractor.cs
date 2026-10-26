
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private InputActionReference interactionReference;
    [SerializeField] private InteractionPromptUI interactionPrompt;

    private InteractableGlow currentGlow;
    private IInteractible currentInteractible;

    private void OnEnable()
    {
        interactionReference.action.Enable();
        interactionReference.action.started += PlayerInteracted;
    }

    private void OnDisable()
    {
        interactionReference.action.started -= PlayerInteracted;
        interactionReference.action.Disable();

        ClearHover();
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        InteractableGlow newGlow = null;
        IInteractible newInteractible = null;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance))
        {
            newInteractible =
                hitInfo.collider.GetComponentInParent<IInteractible>();

            if (newInteractible != null)
            {
                newGlow =
                    hitInfo.collider.GetComponentInParent<InteractableGlow>();
            }
        }

        if (newGlow != currentGlow ||
            !ReferenceEquals(newInteractible, currentInteractible))
        {
            ClearHover();

            currentGlow = newGlow;
            currentInteractible = newInteractible;

            if (currentGlow != null)
                currentGlow.SetGlow(true);

            if (currentInteractible != null && interactionPrompt != null)
            {
                string message = "INTERACT";

                if (currentGlow != null)
                {
                    string objectName = currentGlow.gameObject.name;

                    if (objectName == "TutorialKey")
                        message = "PICK UP KEYCARD";
                    else if (objectName == "File App (Door)")
                        message = "OPEN DOOR";
                }

                interactionPrompt.ShowPrompt(message);
            }
        }
    }

    private void ClearHover()
    {
        if (currentGlow != null)
        {
            currentGlow.SetGlow(false);
            currentGlow = null;
        }

        currentInteractible = null;

        if (interactionPrompt != null)
            interactionPrompt.HidePrompt();
    }

    private void PlayerInteracted(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance))
            return;

        if (hitInfo.collider.TryGetComponent(out IInteractible interactible))
        {
            interactible.Interact(gameObject);
        }
    }
}
