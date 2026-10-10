
using System;
using UnityEngine;

/// <summary>
/// Manages dialogue visibility and game pause state.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private PauseManager pauseManager;

    private bool _isDialogueOpen;
    private bool _pausedByDialogue;

    public bool IsDialogueOpen => _isDialogueOpen;

    /// <summary>
    /// Opens a confirmation dialogue and pauses gameplay.
    /// </summary>
    public void ShowConfirmation(
        string title,
        string message,
        Action onConfirm = null)
    {
        if (_isDialogueOpen || dialogueUI == null)
        {
            return;
        }

        _isDialogueOpen = true;

        // Remember whether the game was already paused.
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
    /// Closes the dialogue and restores gameplay when appropriate.
    /// </summary>
    public void CloseDialogue()
    {
        if (!_isDialogueOpen)
        {
            return;
        }

        dialogueUI.HideDialogue();
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
}
