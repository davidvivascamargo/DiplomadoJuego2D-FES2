
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Centralized scene loader.
/// Validates scene availability and prevents duplicate loads.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private static bool isLoading;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetLoadingState()
    {
        isLoading = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Loads a scene by name or full asset path.
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

        sceneName = sceneName.Trim();

        if (showDebugLogs)
        {
            Debug.Log(
                $"[SceneLoader] Requested scene: '{sceneName}'"
            );
        }

        string resolvedScene = ResolveScene(sceneName);

        if (string.IsNullOrEmpty(resolvedScene))
        {
            Debug.LogError(
                $"[SceneLoader] Scene '{sceneName}' is unavailable. " +
                "Check Build Profiles > Scene List."
            );

            if (showDebugLogs)
                PrintAvailableScenes();

            return;
        }

        string currentScenePath =
            SceneManager.GetActiveScene().path;

        string targetScenePath = resolvedScene;

        if (currentScenePath == targetScenePath)
        {
            Debug.LogWarning(
                "[SceneLoader] Scene is already active."
            );
            return;
        }

        isLoading = true;

        if (showDebugLogs)
        {
            Debug.Log(
                $"[SceneLoader] Loading: {resolvedScene}"
            );
        }

        try
        {
            SceneManager.LoadScene(resolvedScene);
        }
        catch (System.Exception exception)
        {
            isLoading = false;
            Debug.LogException(exception);
        }
    }

    /// <summary>
    /// Resolves a scene from its name or asset path.
    /// </summary>
    private string ResolveScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
            return sceneName;

        string normalizedName =
            System.IO.Path.GetFileNameWithoutExtension(sceneName);

        int sceneCount =
            SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath =
                SceneUtility.GetScenePathByBuildIndex(i);

            string availableName =
                System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (string.Equals(
                availableName,
                normalizedName,
                System.StringComparison.OrdinalIgnoreCase))
            {
                if (Application.CanStreamedLevelBeLoaded(scenePath))
                    return scenePath;
            }
        }

        return null;
    }

    /// <summary>
    /// Prints scenes registered in the active build configuration.
    /// </summary>
    private void PrintAvailableScenes()
    {
        int sceneCount =
            SceneManager.sceneCountInBuildSettings;

        Debug.Log(
            $"[SceneLoader] Registered scenes: {sceneCount}"
        );

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath =
                SceneUtility.GetScenePathByBuildIndex(i);

            Debug.Log(
                $"[SceneLoader] Scene {i}: {scenePath}"
            );
        }
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        isLoading = false;

        if (showDebugLogs)
        {
            Debug.Log(
                $"[SceneLoader] Scene loaded: {scene.name}"
            );
        }
    }
}
