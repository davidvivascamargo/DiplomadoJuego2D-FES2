
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the main menu, new game and saved game navigation.
/// </summary>
public class MainMenuGameController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameModePanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private GameObject[] mainMenuButtons;

    [Header("Scenes")]
    [SerializeField] private string levelMapScene = "LevelMap";

    [Header("Saved Data")]
    [SerializeField] private string pointsSaveKey = "TotalPoints";

    private void Start()
    {
        ShowMainMenu();
    }

    public void OpenGameMode()
    {
        SetMainMenuButtons(false);

        if (gameModePanel != null)
            gameModePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void NewGame()
    {
        LevelProgressManager.ResetProgress();

        PlayerPrefs.DeleteKey(pointsSaveKey);
        PlayerPrefs.Save();

        Debug.Log("[MainMenu] New game started.");

        LoadLevelMap();
    }

    public void ContinueGame()
    {
        Debug.Log("[MainMenu] Continuing saved game.");

        LoadLevelMap();
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (gameModePanel != null)
            gameModePanel.SetActive(false);

        SetMainMenuButtons(true);
    }

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

    private void LoadLevelMap()
    {
        if (!Application.CanStreamedLevelBeLoaded(levelMapScene))
        {
            Debug.LogError(
                $"[MainMenu] Scene '{levelMapScene}' is not in the build."
            );
            return;
        }

        SceneManager.LoadScene(levelMapScene);
    }
}
