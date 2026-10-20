
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CinematicDialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Decision References")]
    [SerializeField] private GameObject decisionPanel;
    [SerializeField] private TMP_Text yesOptionText;
    [SerializeField] private TMP_Text noOptionText;

    [Header("Typewriter Settings")]
    [SerializeField, Min(0.001f)]
    private float characterDelay = 0.04f;

    private Coroutine typingCoroutine;
    private Action onContinue;
    private Action onYes;
    private Action onNo;

    private bool isTyping;
    private bool isOpen;
    private bool isDecision;
    private bool decisionReady;

    public bool IsOpen => isOpen;
    public bool IsDecisionActive => isOpen && isDecision && decisionReady;

    private void Awake()
    {
        dialoguePanel.SetActive(false);

        if (decisionPanel != null)
            decisionPanel.SetActive(false);
    }

    public void ShowMessage(
        string speaker,
        string message,
        Action continueAction)
    {
        onContinue = continueAction;
        onYes = null;
        onNo = null;

        isDecision = false;
        decisionReady = false;

        PrepareMessage(speaker, message);
    }

    public void ShowDecision(
        string speaker,
        string message,
        Action yesAction,
        Action noAction)
    {
        onContinue = null;
        onYes = yesAction;
        onNo = noAction;

        isDecision = true;
        decisionReady = false;

        PrepareMessage(speaker, message);
    }

    private void PrepareMessage(string speaker, string message)
    {
        isOpen = true;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        speakerName.text = speaker;
        dialogueText.text = message;

        dialoguePanel.SetActive(true);
        continueIndicator.SetActive(false);

        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        dialogueText.maxVisibleCharacters = 0;
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;

        dialogueText.ForceMeshUpdate();
        int characterCount = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= characterCount; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(characterDelay);
        }

        isTyping = false;
        typingCoroutine = null;

        FinishTyping();
    }

    private void FinishTyping()
    {
        if (isDecision)
        {
            decisionReady = true;

            if (decisionPanel != null)
                decisionPanel.SetActive(true);
        }
        else
        {
            continueIndicator.SetActive(true);
        }
    }

    public void Continue()
    {
        if (!isOpen)
            return;

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        // Decisions must be answered explicitly.
        if (isDecision)
            return;

        onContinue?.Invoke();
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;

        FinishTyping();
    }

    public void SelectYes()
    {
        if (!IsDecisionActive)
            return;

        Action callback = onYes;
        decisionReady = false;

        callback?.Invoke();
    }

    public void SelectNo()
    {
        if (!IsDecisionActive)
            return;

        Action callback = onNo;
        decisionReady = false;

        callback?.Invoke();
    }

    public void Hide()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        isOpen = false;
        isDecision = false;
        decisionReady = false;

        onContinue = null;
        onYes = null;
        onNo = null;

        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        dialoguePanel.SetActive(false);
    }
}
