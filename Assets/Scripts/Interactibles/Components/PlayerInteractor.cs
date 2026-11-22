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
        IInteractible newInteractible = FindInteractible(out InteractableGlow newGlow);

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
                string message = currentGlow != null ? currentGlow.PromptText : "INTERACT";

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

    private IInteractible FindInteractible(out InteractableGlow glow)
    {
        glow = null;
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            return null;

        Collider target = hit.collider;
        IInteractible interactible = target.GetComponentInParent<IInteractible>();

        if (interactible == null && target.isTrigger)
        {
            // Trigger volumes without an interaction can overlap the slime's trigger.
            // Choose the nearest interactible or solid surface so walls still block the ray.
            float nearestDistance = float.PositiveInfinity;
            foreach (RaycastHit candidate in Physics.RaycastAll(ray, interactionDistance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                if (candidate.distance >= nearestDistance)
                    continue;

                IInteractible candidateInteractible =
                    candidate.collider.GetComponentInParent<IInteractible>();
                if (candidate.collider.isTrigger && candidateInteractible == null)
                    continue;

                target = candidate.collider;
                interactible = candidateInteractible;
                nearestDistance = candidate.distance;
            }
        }

        if (interactible != null)
            glow = target.GetComponentInParent<InteractableGlow>();

        return interactible;
    }

    private void PlayerInteracted(InputAction.CallbackContext context)
    {
        FindInteractible(out _)?.Interact(gameObject);
    }
}
