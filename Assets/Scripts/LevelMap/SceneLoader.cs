using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides centralized scene loading functionality.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads a scene using its configured name.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SceneLoader] Scene name is empty.");
            return;
        }

        SceneManager.LoadSceneAsync(sceneName);
    }
}
