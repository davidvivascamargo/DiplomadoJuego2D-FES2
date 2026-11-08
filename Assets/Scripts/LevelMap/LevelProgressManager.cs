
using UnityEngine;

/// <summary>
/// Manages and saves linear level progression.
/// </summary>
public static class LevelProgressManager
{
    private const string ProgressKey = "HighestUnlockedLevel";
    private const string CompletedKey = "HighestCompletedLevel";

    // Change this to match the number of playable levels.
    private const int MaxLevel = 4;

    public static int GetHighestUnlockedLevel()
    {
        int saved = PlayerPrefs.GetInt(ProgressKey, 1);
        return Mathf.Clamp(saved, 1, MaxLevel);
    }

    public static int GetHighestCompletedLevel()
    {
        int saved = PlayerPrefs.GetInt(CompletedKey, 0);

        // Compatibility with existing saved games.
        int legacyCompleted = GetHighestUnlockedLevel() - 1;

        return Mathf.Clamp(
            Mathf.Max(saved, legacyCompleted),
            0,
            MaxLevel
        );
    }

    public static bool IsLevelUnlocked(int levelNumber)
    {
        return IsValidLevel(levelNumber) &&
               levelNumber <= GetHighestUnlockedLevel();
    }

    public static bool IsLevelCompleted(int levelNumber)
    {
        return IsValidLevel(levelNumber) &&
               levelNumber <= GetHighestCompletedLevel();
    }

    public static void CompleteLevel(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
        {
            Debug.LogWarning(
                $"[LevelProgress] Invalid level: {levelNumber}"
            );
            return;
        }

        if (!IsLevelUnlocked(levelNumber))
        {
            Debug.LogWarning(
                $"[LevelProgress] Level {levelNumber} is locked."
            );
            return;
        }

        int highestCompleted = GetHighestCompletedLevel();

        if (levelNumber > highestCompleted)
        {
            PlayerPrefs.SetInt(CompletedKey, levelNumber);
        }

        int nextLevel = Mathf.Min(levelNumber + 1, MaxLevel);

        if (nextLevel > GetHighestUnlockedLevel())
        {
            PlayerPrefs.SetInt(ProgressKey, nextLevel);
        }

        PlayerPrefs.Save();

        Debug.Log(
            $"[LevelProgress] Level {levelNumber} completed. " +
            $"Highest unlocked: {GetHighestUnlockedLevel()}"
        );
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(ProgressKey);
        PlayerPrefs.DeleteKey(CompletedKey);
        PlayerPrefs.Save();

        Debug.Log("[LevelProgress] Progress reset.");
    }

    private static bool IsValidLevel(int levelNumber)
    {
        return levelNumber >= 1 && levelNumber <= MaxLevel;
    }
}
