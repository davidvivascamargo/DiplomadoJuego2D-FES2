using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the items collected by the player.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int maxSlots = 100;

    private readonly List<ItemData> _items = new();

    /// <summary>
    /// Returns the items currently stored in the inventory.
    /// </summary>
    public IReadOnlyList<ItemData> Items => _items;

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    public bool AddItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        if (!item.CanStore)
        {
            return false;
        }

        if (_items.Count >= maxSlots)
        {
            return false;
        }

        _items.Add(item);
        return true;
    }

    /// <summary>
    /// Checks whether the inventory contains the specified item.
    /// </summary>
    public bool HasItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        return _items.Contains(item);
    }

    /// <summary>
    /// Removes an item from the inventory.
    /// </summary>
    public bool RemoveItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        return _items.Remove(item);
    }
}