
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls inventory visibility, item slots, and item details.
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

    [Header("Item Details")]
    [SerializeField] private Image itemIconPreview;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    private PlayerInventory _playerInventory;
    private PauseManager _pauseManager;

    private void Awake()
    {
        _playerInventory = FindFirstObjectByType<PlayerInventory>();
        _pauseManager = FindFirstObjectByType<PauseManager>();

        CreateSlots();
        ClearItemDetails();

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
    /// Creates clickable inventory slots.
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
            GameObject slot = Instantiate(slotPrefab, slotGrid);

            int slotIndex = i;
            Button button = slot.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(() => SelectItem(slotIndex));
            }
        }

        Debug.Log($"[InventoryUI] Created {slotCount} inventory slots.");
    }

/// <summary>
/// Updates slot icons from the player's inventory.
/// </summary>
private void RefreshInventory()
{
    if (_playerInventory == null || slotGrid == null)
    {
        Debug.LogWarning("[InventoryUI] Inventory references are missing.");
        return;
    }

    for (int i = 0; i < slotGrid.childCount; i++)
    {
        Transform slot = slotGrid.GetChild(i);
        Transform iconTransform = slot.Find("ItemIcon");

        if (iconTransform == null)
        {
            continue;
        }

        Image icon = iconTransform.GetComponent<Image>();

        if (icon == null)
        {
            continue;
        }

        if (i < _playerInventory.Items.Count)
        {
            ItemData item = _playerInventory.Items[i];

            icon.sprite = item != null ? item.Icon : null;
            icon.enabled = item != null && item.Icon != null;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}

    /// <summary>
    /// Displays the selected item's information.
    /// </summary>
    private void SelectItem(int slotIndex)
    {
        if (_playerInventory == null ||
            slotIndex >= _playerInventory.Items.Count)
        {
            ClearItemDetails();
            return;
        }

        ItemData item = _playerInventory.Items[slotIndex];

        if (item == null)
        {
            ClearItemDetails();
            return;
        }

        itemIconPreview.sprite = item.Icon;
        itemIconPreview.enabled = item.Icon != null;

        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;

        Debug.Log($"[InventoryUI] Selected item: {item.ItemName}");
    }

    /// <summary>
    /// Clears the item details panel.
    /// </summary>
    private void ClearItemDetails()
    {
        if (itemIconPreview != null)
        {
            itemIconPreview.sprite = null;
            itemIconPreview.enabled = false;
        }

        if (itemNameText != null)
        {
            itemNameText.text = "";
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "";
        }
    }

    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        ToggleInventory();
    }

    public void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        RefreshInventory();
        ClearItemDetails();

        inventoryPanel.SetActive(true);

        if (_pauseManager != null)
        {
            _pauseManager.PauseGame();
        }

        Debug.Log("[InventoryUI] Inventory opened.");
    }

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
