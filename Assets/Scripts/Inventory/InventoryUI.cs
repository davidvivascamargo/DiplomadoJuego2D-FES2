
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Controls the inventory interface, generates inventory slots,
/// displays collected items, and manages inventory visibility.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InputActionReference inventoryAction;

    [Header("Slots")]
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount = 100;

    private PlayerInventory _playerInventory;
    private PauseManager _pauseManager;

    private void Awake()
    {
        _playerInventory = FindFirstObjectByType<PlayerInventory>();
        _pauseManager = FindFirstObjectByType<PauseManager>();

        CreateSlots();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (inventoryAction == null)
        {
            Debug.LogWarning("[InventoryUI] Inventory action is not assigned.");
            return;
        }

        inventoryAction.action.performed += OnInventoryPerformed;
        inventoryAction.action.Enable();
    }

    private void OnDisable()
    {
        if (inventoryAction == null)
        {
            return;
        }

        inventoryAction.action.performed -= OnInventoryPerformed;
        inventoryAction.action.Disable();
    }

    /// <summary>
    /// Generates inventory slots from the configured prefab.
    /// </summary>
    private void CreateSlots()
    {
        if (slotGrid == null || slotPrefab == null)
        {
            Debug.LogWarning("[InventoryUI] SlotGrid or SlotPrefab is missing.");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, slotGrid);
        }

        Debug.Log($"[InventoryUI] Created {slotCount} inventory slots.");
    }

    /// <summary>
    /// Refreshes all inventory slots using the player's stored items.
    /// </summary>
    private void RefreshInventory()
    {
        if (_playerInventory == null)
        {
            Debug.LogWarning("[InventoryUI] PlayerInventory was not found.");
            return;
        }

        if (slotGrid == null)
        {
            return;
        }

        // Clear all slot icons before displaying current items.
        foreach (Transform slot in slotGrid)
        {
            Transform iconTransform = slot.Find("ItemIcon");

            if (iconTransform == null)
            {
                continue;
            }

            Image itemIcon = iconTransform.GetComponent<Image>();

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }

        // Display collected items in their corresponding slots.
        int slotIndex = 0;

        foreach (ItemData item in _playerInventory.Items)
        {
            if (slotIndex >= slotGrid.childCount)
            {
                break;
            }

            Transform slot = slotGrid.GetChild(slotIndex);
            Transform iconTransform = slot.Find("ItemIcon");

            if (iconTransform != null)
            {
                Image itemIcon = iconTransform.GetComponent<Image>();

                if (itemIcon != null)
                {
                    itemIcon.sprite = item.Icon;
                    itemIcon.enabled = item.Icon != null;
                }
            }

            slotIndex++;
        }
    }

    /// <summary>
    /// Handles the inventory input action.
    /// </summary>
    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        ToggleInventory();
    }

    /// <summary>
    /// Opens the inventory and pauses the game.
    /// </summary>
    public void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogWarning("[InventoryUI] InventoryPanel is not assigned.");
            return;
        }

        RefreshInventory();

        inventoryPanel.SetActive(true);

        if (_pauseManager != null)
        {
            _pauseManager.PauseGame();
        }

        Debug.Log("[InventoryUI] Inventory opened.");
    }

    /// <summary>
    /// Closes the inventory and resumes the game.
    /// </summary>
    public void CloseInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        inventoryPanel.SetActive(false);

        if (_pauseManager != null)
        {
            _pauseManager.ResumeGame();
        }

        Debug.Log("[InventoryUI] Inventory closed.");
    }

    /// <summary>
    /// Toggles the inventory panel visibility.
    /// </summary>
    public void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        if (inventoryPanel.activeSelf)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
}
