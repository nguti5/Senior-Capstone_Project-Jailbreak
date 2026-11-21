
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryItemContainer;
    [SerializeField] private GameObject inventorySlotPrefab;

    [Header("Player Controls")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;

    private LegacyCatalogManager catalogManager;
    private bool inventoryOpen = false;

    private void Awake()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Start()
    {
        catalogManager = LegacyCatalogManager.Instance;

        if (catalogManager == null)
            catalogManager = FindFirstObjectByType<LegacyCatalogManager>();

        if (catalogManager == null)
            Debug.LogError("LegacyCatalogManager was not found!");

        if (inventoryPanel == null)
            Debug.LogError("Inventory Panel has not been assigned!");

        Debug.Log("Inventory UI initialized!");
    }

    private void Update()
    {
        if (inventoryPanel == null || Keyboard.current == null)
            return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (inventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    private void OpenInventory()
    {
        inventoryOpen = true;
        inventoryPanel.SetActive(true);

        RefreshInventoryUI();

        if (playerController != null)
            playerController.enabled = false;

        if (playerInteractor != null)
            playerInteractor.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Inventory opened!");
    }

    private void CloseInventory()
    {
        inventoryOpen = false;
        inventoryPanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        if (playerInteractor != null)
            playerInteractor.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Inventory closed!");
    }

    private void RefreshInventoryUI()
    {
        if (catalogManager == null)
            catalogManager = LegacyCatalogManager.Instance;

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
            Destroy(child.gameObject);

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
                slotUI.SetItem(itemId);
        }

        Debug.Log("Inventory UI refreshed: " + ownedItems.Count + " items.");
    }
}
