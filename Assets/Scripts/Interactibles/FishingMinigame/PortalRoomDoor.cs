
using UnityEngine;

public class PortalRoomDoor : MonoBehaviour, IInteractible
{
    [Header("Door Settings")]
    [SerializeField] private string requiredKeyId = "PortalRoom_key";

    private bool isUnlocked = false;

    public void Interact(GameObject interactor)
    {
        LegacyCatalogManager catalog =
            LegacyCatalogManager.Instance;

        if (catalog == null || !catalog.IsInventoryLoaded)
        {
            Debug.LogWarning("Inventory is not ready!");
            return;
        }

        if (catalog.HasItem(requiredKeyId))
        {
            isUnlocked = true;
            Debug.Log("Mailroom door unlocked!");
        }
        else
        {
            Debug.Log("Door locked! Portal Room Key required.");
        }
    }
}
