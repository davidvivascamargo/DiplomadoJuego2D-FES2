using UnityEngine;

/// <summary>
/// Controls the collectible key item.
/// </summary>
public class KeyItem : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("[KeyItem] PlayerInventory was not found on the player.");
            return;
        }

        if (inventory.AddItem(itemData))
        {
            Destroy(gameObject);
        }
    }
}