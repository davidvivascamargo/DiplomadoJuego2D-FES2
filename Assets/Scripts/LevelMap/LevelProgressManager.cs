
using UnityEngine;

/// <summary>
/// Saves and manages level progression.
/// </summary>
public static class LevelProgressManager
{
    private const string ProgressKey = "HighestUnlockedLevel";

    public static int GetHighestUnlockedLevel()
    {
        return Mathf.Max(1, PlayerPrefs.GetInt(ProgressKey, 1));
    }

    public static void CompleteLevel(int levelNumber)
    {
        if (levelNumber < 1)
            return;

        int highestUnlocked = GetHighestUnlockedLevel();

        if (levelNumber > highestUnlocked)
            return;

        int nextLevel = levelNumber + 1;

        if (nextLevel > highestUnlocked)
        {
            PlayerPrefs.SetInt(ProgressKey, nextLevel);
            PlayerPrefs.Save();
        }

        Debug.Log($"[LevelProgress] Level {levelNumber} completed.");
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(ProgressKey);
        PlayerPrefs.Save();
    }
}
