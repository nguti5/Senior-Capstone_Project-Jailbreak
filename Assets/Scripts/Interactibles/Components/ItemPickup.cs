using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InteractableGlow))]
public class ItemPickup : MonoBehaviour, IInteractible
{
    [SerializeField] private string itemId;

    private bool isClaiming;

    private IEnumerator Start()
    {
        // once login and inventory are ready, tidy up if this slot already has the item
        while (LegacyCatalogManager.Instance == null) yield return null;
        while (!LegacyCatalogManager.Instance.IsInventoryLoaded) yield return null;

        if (LegacyCatalogManager.Instance.HasItem(itemId))
            Collected();
    }

    public void Interact(GameObject interactor) => Claim();

    public void Claim()   // also callable from a UnityEvent, e.g. the cat's dialogue
    {
        if (isClaiming) return;

        if (LegacyCatalogManager.Instance == null)
        {
            Debug.LogError("LegacyCatalogManager not found. Start from the TitlePage scene.");
            return;
        }

        isClaiming = true;
        LegacyCatalogManager.Instance.ClaimItem(itemId, OnClaimed);
    }

    private void OnClaimed(bool success)
    {
        isClaiming = false;

        if (success) Collected();
        else Debug.LogWarning("Couldn't collect " + itemId + ". Try again.");
    }

    private void Collected()
    {
        Debug.Log(itemId + " collected.");

        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.NotifyItemAcquired(itemId);

        gameObject.SetActive(false);
    }
}