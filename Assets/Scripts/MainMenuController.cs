using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls navigation and actions from the main menu.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Scenes")]
    [SerializeField] private string levelMapScene = "LevelMap";

    public void OpenOptionsPanel()
    {
        SetPanelState(mainMenuPanel, false);
        SetPanelState(optionsPanel, true);
    }

    public void OpenMainMenuPanel()
    {
        SetPanelState(optionsPanel, false);
        SetPanelState(mainMenuPanel, true);
    }

    public void PlayGame()
    {
        Debug.Log($"[MainMenuController] Loading scene: {levelMapScene}");
        LoadScene(levelMapScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[MainMenuController] Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void SetPanelState(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }
}