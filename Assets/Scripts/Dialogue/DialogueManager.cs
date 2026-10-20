
using System;
using UnityEngine;

/// <summary>
/// Manages standard confirmation dialogues and cinematic conversations.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("Standard Dialogue References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private PauseManager pauseManager;

    [Header("Cinematic Dialogue References")]
    [SerializeField] private CinematicDialogueUI cinematicDialogueUI;

    private bool _isDialogueOpen;
    private bool _pausedByDialogue;

    public bool IsDialogueOpen => _isDialogueOpen;

    public bool IsCinematicDialogueOpen =>
        cinematicDialogueUI != null &&
        cinematicDialogueUI.IsOpen;

    /// <summary>
    /// Opens a standard confirmation dialogue and pauses gameplay
    /// only when the game is not already paused.
    /// </summary>
    public void ShowConfirmation(
        string title,
        string message,
        Action onConfirm = null)
    {
        if (_isDialogueOpen || IsCinematicDialogueOpen || dialogueUI == null)
            return;

        _isDialogueOpen = true;

        _pausedByDialogue = Time.timeScale > 0f;

        if (_pausedByDialogue)
        {
            if (pauseManager != null)
            {
                pauseManager.PauseGame();
            }
            else
            {
                Time.timeScale = 0f;
            }
        }

        dialogueUI.ShowDialogue(
            title,
            message,
            () =>
            {
                CloseDialogue();
                onConfirm?.Invoke();
            },
            CloseDialogue
        );
    }

    /// <summary>
    /// Closes a standard confirmation dialogue.
    /// </summary>
    public void CloseDialogue()
    {
        if (!_isDialogueOpen)
            return;

        dialogueUI?.HideDialogue();

        _isDialogueOpen = false;

        if (_pausedByDialogue)
        {
            if (pauseManager != null)
            {
                pauseManager.ResumeGame();
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        _pausedByDialogue = false;
    }

    /// <summary>
    /// Displays a cinematic dialogue message.
    /// CinematicManager is responsible for pausing gameplay.
    /// </summary>
    public void ShowCinematicMessage(
        string speaker,
        string message,
        Action onContinue)
    {
        if (cinematicDialogueUI == null)
        {
            Debug.LogWarning(
                "[DialogueManager] CinematicDialogueUI is not assigned."
            );
            return;
        }

        if (_isDialogueOpen)
        {
            Debug.LogWarning(
                "[DialogueManager] Cannot open cinematic dialogue while a confirmation is active."
            );
            return;
        }

        if (CinematicManager.Instance == null ||
            !CinematicManager.Instance.IsCinematic)
        {
            Debug.LogWarning(
                "[DialogueManager] Cinematic dialogue requires an active cinematic."
            );
            return;
        }

        cinematicDialogueUI.ShowMessage(
            speaker,
            message,
            onContinue
        );
    }

    /// <summary>
    /// Displays a cinematic dialogue with Yes/No choices.
    /// CinematicManager remains responsible for pausing gameplay.
    /// </summary>
    public void ShowCinematicDecision(
        string speaker,
        string message,
        Action onYes,
        Action onNo)
    {
        if (cinematicDialogueUI == null)
        {
            Debug.LogWarning(
                "[DialogueManager] CinematicDialogueUI is not assigned."
            );
            return;
        }

        if (_isDialogueOpen)
        {
            Debug.LogWarning(
                "[DialogueManager] Cannot show a cinematic decision while a confirmation is active."
            );
            return;
        }

        if (CinematicManager.Instance == null ||
            !CinematicManager.Instance.IsCinematic)
        {
            Debug.LogWarning(
                "[DialogueManager] Cinematic decisions require an active cinematic."
            );
            return;
        }

        cinematicDialogueUI.ShowDecision(
            speaker,
            message,
            onYes,
            onNo
        );
    }

    /// <summary>
    /// Completes the typewriter effect or advances
    /// to the next cinematic message.
    /// </summary>
    public void ContinueCinematicDialogue()
    {
        if (!IsCinematicDialogueOpen)
            return;

        cinematicDialogueUI.Continue();
    }

    /// <summary>
    /// Closes the cinematic dialogue without resuming gameplay.
    /// </summary>
    public void CloseCinematicDialogue()
    {
        if (!IsCinematicDialogueOpen)
            return;

        cinematicDialogueUI.Hide();
    }
}
