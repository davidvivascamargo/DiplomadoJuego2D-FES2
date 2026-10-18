using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game pauses from multiple independent systems.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private readonly HashSet<PauseReason> activePauseReasons = new();

    public bool IsPaused => activePauseReasons.Count > 0;

    public event Action<bool> OnPauseStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Requests a pause for a specific reason.
    /// </summary>
    public void PauseGame(PauseReason reason)
    {
        bool wasPaused = IsPaused;

        activePauseReasons.Add(reason);

        UpdateTimeScale(wasPaused);
    }

    /// <summary>
    /// Removes a specific pause reason.
    /// </summary>
    public void ResumeGame(PauseReason reason)
    {
        bool wasPaused = IsPaused;

        activePauseReasons.Remove(reason);

        UpdateTimeScale(wasPaused);
    }

    /// <summary>
    /// Checks whether a specific system is requesting a pause.
    /// </summary>
    public bool IsPausedBy(PauseReason reason)
    {
        return activePauseReasons.Contains(reason);
    }

    /// <summary>
    /// Maintains compatibility with existing inventory calls.
    /// </summary>
    public void PauseGame()
    {
        PauseGame(PauseReason.Inventory);
    }

    public void ResumeGame()
    {
        ResumeGame(PauseReason.Inventory);
    }

    public void TogglePause()
    {
        if (IsPausedBy(PauseReason.Inventory))
        {
            ResumeGame(PauseReason.Inventory);
        }
        else
        {
            PauseGame(PauseReason.Inventory);
        }
    }

    private void UpdateTimeScale(bool wasPaused)
    {
        Time.timeScale = IsPaused ? 0f : 1f;

        if (wasPaused != IsPaused)
        {
            Debug.Log(
                $"[PauseManager] Game paused: {IsPaused}"
            );

            OnPauseStateChanged?.Invoke(IsPaused);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Time.timeScale = 1f;
        Instance = null;
    }
}