
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryItemContainer;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;

    private LegacyCatalogManager catalogManager;

    private void Start()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel has not been assigned!");
            return;
        }

        inventoryPanel.SetActive(false);

        catalogManager = FindFirstObjectByType<LegacyCatalogManager>();

        if (catalogManager == null)
        {
            Debug.LogError("LegacyCatalogManager was not found!");
        }

        Debug.Log("Inventory UI initialized!");
    }

    private void Update()
    {
        if (inventoryPanel == null || Keyboard.current == null)
        {
            return;
        }


        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            bool isOpen = inventoryPanel.activeSelf;

            inventoryPanel.SetActive(!isOpen);



            if (!isOpen)
            {
                RefreshInventoryUI();

                if (playerController != null)
                {
                    playerController.enabled = false;
                }

                if (playerInteractor != null)
                {
                    playerInteractor.enabled = false;
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                if (playerController != null)
                {
                    playerController.enabled = true;
                }

                if (playerInteractor != null)
                {
                    playerInteractor.enabled = true;
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }



            Debug.Log(isOpen ? "Inventory closed!" : "Inventory opened!");
        }

    }

    private void RefreshInventoryUI()
    {
        if (catalogManager == null ||
            inventoryItemContainer == null ||
            inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory UI references are missing!");
            return;
        }

        if (!catalogManager.IsInventoryLoaded)
        {
            Debug.LogWarning("PlayFab inventory is still loading.");
            return;
        }

        foreach (Transform child in inventoryItemContainer)
        {
            Destroy(child.gameObject);
        }

        List<string> ownedItems = catalogManager.GetOwnedItemIds();

        foreach (string itemId in ownedItems)
        {
            GameObject slotObject = Instantiate(
                inventorySlotPrefab,
                inventoryItemContainer
            );

            InventorySlotUI slotUI =
                slotObject.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.SetItem(itemId);
            }
        }

        Debug.Log("Inventory UI refreshed: " + ownedItems.Count + " items.");
    }
}
