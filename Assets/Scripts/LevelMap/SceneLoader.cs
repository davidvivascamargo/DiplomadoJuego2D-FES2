
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides centralized scene loading functionality.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private bool isLoading;

    /// <summary>
    /// Loads a scene using its configured name.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "[SceneLoader] Scene name is empty."
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[SceneLoader] Scene '{sceneName}' is not included in the build."
            );
            return;
        }

        isLoading = true;

        Debug.Log(
            $"[SceneLoader] Loading scene: {sceneName}"
        );

        SceneManager.LoadSceneAsync(sceneName);
    }
}
