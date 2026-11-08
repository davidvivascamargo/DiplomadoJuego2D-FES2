
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls main menu navigation, game modes,
/// controls panel, credits panel, game exit
/// and saved game progression.
/// </summary>
public class MainMenuGameController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameModePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private GameObject[] mainMenuButtons;

    [Header("Scenes")]
    [SerializeField] private string levelMapScene = "LevelMap";

    [Header("Saved Data")]
    [SerializeField] private string pointsSaveKey = "TotalPoints";

    private bool _isLoading;

    private void Start()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Opens the new game and continue game panel.
    /// </summary>
    public void OpenGameMode()
    {
        SetMainMenuButtons(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (gameModePanel != null)
            gameModePanel.SetActive(true);
    }

    /// <summary>
    /// Opens the controls information panel.
    /// </summary>
    public void OpenControls()
    {
        if (gameModePanel != null)
            gameModePanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        Debug.Log("[MainMenu] Controls panel opened.");
    }

    /// <summary>
    /// Closes the controls panel and returns to main menu.
    /// </summary>
    public void CloseControls()
    {
        ShowMainMenu();

        Debug.Log("[MainMenu] Returned from controls.");
    }

    /// <summary>
    /// Opens the credits information panel.
    /// </summary>
    public void OpenCredits()
    {
        if (gameModePanel != null)
            gameModePanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        Debug.Log("[MainMenu] Credits panel opened.");
    }

    /// <summary>
    /// Closes the credits panel and returns to main menu.
    /// </summary>
    public void CloseCredits()
    {
        ShowMainMenu();

        Debug.Log("[MainMenu] Returned from credits.");
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Starts a new game and resets saved progression.
    /// </summary>
    public void NewGame()
    {
        if (_isLoading || !CanLoadMap())
            return;

        LevelProgressManager.ResetProgress();

        PlayerPrefs.DeleteKey(pointsSaveKey);
        PlayerPrefs.Save();

        Debug.Log("[MainMenu] New game started.");

        LoadLevelMap();
    }

    /// <summary>
    /// Continues the previously saved game.
    /// </summary>
    public void ContinueGame()
    {
        if (_isLoading || !CanLoadMap())
            return;

        Debug.Log(
            "[MainMenu] Continuing game. Highest unlocked: " +
            LevelProgressManager.GetHighestUnlockedLevel()
        );

        LoadLevelMap();
    }

    /// <summary>
    /// Exits the game on Android and standalone builds.
    /// </summary>
        public void QuitGame()
    {
        Debug.Log("[MainMenu] Exiting game...");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    /// <summary>
    /// Shows the main menu and hides secondary panels.
    /// </summary>
    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (gameModePanel != null)
            gameModePanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        SetMainMenuButtons(true);
    }

    /// <summary>
    /// Shows or hides main menu buttons.
    /// </summary>
    private void SetMainMenuButtons(bool visible)
    {
        if (mainMenuButtons == null)
            return;

        foreach (GameObject button in mainMenuButtons)
        {
            if (button != null)
                button.SetActive(visible);
        }
    }

    /// <summary>
    /// Validates that the level map scene is available.
    /// </summary>
    private bool CanLoadMap()
    {
        if (string.IsNullOrWhiteSpace(levelMapScene))
        {
            Debug.LogError("[MainMenu] Map scene is empty.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(levelMapScene))
        {
            Debug.LogError(
                $"[MainMenu] Scene '{levelMapScene}' is unavailable."
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads the level map scene.
    /// </summary>
    private void LoadLevelMap()
    {
        _isLoading = true;

        SceneManager.LoadScene(levelMapScene);
    }

    
}
