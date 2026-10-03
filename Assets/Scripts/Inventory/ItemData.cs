using UnityEngine;

/// <summary>
/// Defines the data and properties of an item that can be stored in the inventory.
/// </summary>
[CreateAssetMenu(
    fileName = "ItemData",
    menuName = "Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("Item Information")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [TextArea]
    [SerializeField] private string description;

    [Header("Storage")]
    [SerializeField] private bool canStore = true;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public string Description => description;
    public bool CanStore => canStore;
}