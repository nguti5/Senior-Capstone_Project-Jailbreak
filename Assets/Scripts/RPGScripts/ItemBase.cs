using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Creature/New Item")]
public class ItemBase : ScriptableObject
{
    [Tooltip("Must match the Item ID in the PlayFab catalog exactly.")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private int healAmount = 20;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;
    public int HealAmount => healAmount;
}

public class BattleItemEntry
{
    public ItemBase Item;
    public int Count;

    public BattleItemEntry(ItemBase item, int count)
    {
        Item = item;
        Count = count;
    }
}
