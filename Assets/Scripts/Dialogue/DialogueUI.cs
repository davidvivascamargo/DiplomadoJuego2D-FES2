
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays dialogue text with a typewriter effect and confirmation buttons.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue Panel")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueTitle;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Dialogue Buttons")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Typewriter Settings")]
    [SerializeField, Min(0.001f)]
    private float characterDelay = 0.04f;

    private Coroutine _typingCoroutine;
    private Action _onYes;
    private Action _onNo;
    private bool _isTyping;

    private void Awake()
    {
        // Connect buttons only once.
        yesButton.onClick.AddListener(ConfirmYes);
        noButton.onClick.AddListener(ConfirmNo);

        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Opens a dialogue with configurable text and actions.
    /// </summary>
    public void ShowDialogue(
        string title,
        string message,
        Action onYes,
        Action onNo)
    {
        _onYes = onYes;
        _onNo = onNo;

        dialogueTitle.text = title;
        dialogueText.text = message;

        dialoguePanel.SetActive(true);

        // Hide all characters without changing the text layout.
        dialogueText.maxVisibleCharacters = 0;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(TypeText());
    }

    /// <summary>
    /// Reveals text using unscaled time so it works while paused.
    /// </summary>
    private IEnumerator TypeText()
    {
        _isTyping = true;

        dialogueText.ForceMeshUpdate();
        int characterCount = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= characterCount; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            yield return new WaitForSecondsRealtime(characterDelay);
        }

        _isTyping = false;
        _typingCoroutine = null;

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Immediately reveals the complete dialogue message.
    /// </summary>
    public void CompleteTyping()
    {
        if (!_isTyping)
        {
            return;
        }

        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        _isTyping = false;

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    private void ConfirmYes()
    {
        _onYes?.Invoke();
    }

    private void ConfirmNo()
    {
        _onNo?.Invoke();
    }

    /// <summary>
    /// Closes the dialogue and clears its callbacks.
    /// </summary>
    public void HideDialogue()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        _isTyping = false;
        _onYes = null;
        _onNo = null;

        dialoguePanel.SetActive(false);
    }
}
