
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the bonus stage countdown timer and UI.
/// </summary>
public class BarrelBonusManager : MonoBehaviour
{
    [Header("Bonus Settings")]
    [SerializeField, Min(1f)]
    private float bonusDuration = 15f;

    [Header("UI")]
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private TMP_Text timerText;

    private Coroutine _bonusCoroutine;
    private float _remainingTime;
    private bool _isRunning;

    public bool IsRunning => _isRunning;
    public float RemainingTime => _remainingTime;

    private void Start()
    {
        _remainingTime = bonusDuration;

        UpdateTimerUI();

        if (bonusPanel != null)
        {
            bonusPanel.SetActive(false);
        }

        // Start automatically when the level begins.
        StartBonus();
    }

    /// <summary>
    /// Starts the bonus stage countdown.
    /// </summary>
    public void StartBonus()
    {
        if (_isRunning)
            return;

        if (bonusPanel == null || timerText == null)
        {
            Debug.LogError(
                "[BonusStage] UI references are missing.",
                this
            );
            return;
        }

        _bonusCoroutine = StartCoroutine(BonusRoutine());
    }

    private IEnumerator BonusRoutine()
    {
        _isRunning = true;
        _remainingTime = bonusDuration;

        bonusPanel.SetActive(true);
        UpdateTimerUI();

        Debug.Log("[BonusStage] Bonus started!");

        while (_remainingTime > 0f)
        {
            _remainingTime = Mathf.Max(
                0f,
                _remainingTime - Time.deltaTime
            );

            UpdateTimerUI();

            yield return null;
        }

        EndBonus();
    }

    /// <summary>
    /// Updates the countdown in MM:SS format.
    /// </summary>
    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int totalSeconds = Mathf.CeilToInt(_remainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Finishes the bonus stage.
    /// </summary>
    public void EndBonus()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _remainingTime = 0f;

        UpdateTimerUI();

        if (_bonusCoroutine != null)
        {
            StopCoroutine(_bonusCoroutine);
            _bonusCoroutine = null;
        }

        Debug.Log("[BonusStage] Bonus finished!");
    }
}
