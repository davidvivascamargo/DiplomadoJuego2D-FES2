using System;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsCinematic => CurrentState == GameState.Cinematic;
    public bool IsPaused => CurrentState == GameState.Paused;

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartCinematic()
    {
        if (CurrentState != GameState.Playing)
            return;

        if (PauseManager.Instance == null)
        {
            Debug.LogWarning("[Cinematic] PauseManager not found.");
            return;
        }

        if (PauseManager.Instance.IsPaused)
            return;

        SetGameState(GameState.Cinematic);

        PauseManager.Instance.PauseGame(PauseReason.Cinematic);
    }

    public void EndCinematic()
    {
        if (CurrentState != GameState.Cinematic)
            return;

        PauseManager.Instance?.ResumeGame(PauseReason.Cinematic);

        SetGameState(GameState.Playing);
    }

    private void SetGameState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        Debug.Log($"Game State Changed: {CurrentState}");

        OnGameStateChanged?.Invoke(CurrentState);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
