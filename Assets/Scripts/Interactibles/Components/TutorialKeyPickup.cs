
using UnityEngine;
using System.Collections;

public class TutorialKeyPickup : MonoBehaviour, IInteractible
{
    private const string KeyItemId = "tutorial_key";
    private bool isClaiming = false;


    private IEnumerator Start()
    {
        LegacyCatalogManager catalogManager = null;

        while (catalogManager == null)
        {
            catalogManager =
                FindFirstObjectByType<LegacyCatalogManager>();

            if (catalogManager == null)
            {
                yield return null;
            }
        }

        while (!catalogManager.IsInventoryLoaded)
        {
            yield return null;
        }

        if (catalogManager.HasItem("tutorial_key"))
        {
            Debug.Log("Tutorial key already owned. Hiding pickup.");
            NotifyObjectives();
            gameObject.SetActive(false);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (isClaiming)
        {
            return;
        }

        Debug.Log("Tutorial key interaction detected!");

        LegacyCatalogManager catalogManager =
            FindFirstObjectByType<LegacyCatalogManager>();

        if (catalogManager == null)
        {
            Debug.LogError("LegacyCatalogManager was not found in the scene.");
            return;
        }

        isClaiming = true;
        catalogManager.ClaimTutorialKey(OnClaimCompleted);
    }

    private void OnClaimCompleted(bool success)
    {
        isClaiming = false;

        if (success)
        {
            Debug.Log("Tutorial key collected successfully!");
            NotifyObjectives();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Tutorial key collection failed. Try again.");
        }
    }

    private void NotifyObjectives(){
        if (ObjectiveManager.Instance != null){
            ObjectiveManager.Instance.NotifyItemAcquired(KeyItemId);
        }
    }
}
