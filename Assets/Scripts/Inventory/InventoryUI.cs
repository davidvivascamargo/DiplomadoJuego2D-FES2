using UnityEngine;

/// <summary>
/// Controls the inventory user interface.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;

    private PlayerInventory _playerInventory;
    private PauseManager _pauseManager;

    private void Awake()
    {
        _playerInventory = FindFirstObjectByType<PlayerInventory>();
        _pauseManager = FindFirstObjectByType<PauseManager>();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Opens the inventory panel and pauses the game.
    /// </summary>
    public void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        inventoryPanel.SetActive(true);

        if (_pauseManager != null)
        {
            _pauseManager.PauseGame();
        }
    }

    /// <summary>
    /// Closes the inventory panel and resumes the game.
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
    }

    /// <summary>
    /// Toggles the inventory panel.
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