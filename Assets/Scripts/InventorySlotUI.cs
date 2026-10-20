
using UnityEngine;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;

    public void SetItem(string itemId)
    {
        if (itemNameText == null)
        {
            Debug.LogError("Item Name Text has not been assigned!");
            return;
        }


        string displayName = itemId.Replace("_", " ");

        displayName = System.Globalization.CultureInfo
            .CurrentCulture.TextInfo.ToTitleCase(displayName);


        itemNameText.text = displayName;

        Debug.Log("Inventory slot displaying: " + displayName);
    }
}
