
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarrelBonusManager : MonoBehaviour
{
    [Header("Bonus Settings")]
    [SerializeField, Min(1f)]
    private float bonusDuration = 15f;

    [Header("UI")]
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private TMP_Text timerText;

    [Header("Scene Transition")]
    [SerializeField] private UnityEngine.Object mapScene;

    [SerializeField, Min(0f)]
    private float returnDelay = 2f;

    private Coroutine _bonusCoroutine;
    private float _remainingTime;
    private bool _isRunning;
    private bool _isFinishing;

    public bool IsRunning => _isRunning;
    public float RemainingTime => _remainingTime;

    private void Start()
    {
        _remainingTime = bonusDuration;
        UpdateTimerUI();

        if (bonusPanel != null)
            bonusPanel.SetActive(false);

        // Starts automatically when the scene loads.
        StartBonus();
    }

    public void StartBonus()
    {
        if (_isRunning || _isFinishing)
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

        _bonusCoroutine = null;
        EndBonus();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int totalSeconds = Mathf.CeilToInt(_remainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void EndBonus()
    {
        if (!_isRunning || _isFinishing)
            return;

        _isRunning = false;
        _isFinishing = true;
        _remainingTime = 0f;

        UpdateTimerUI();

        if (_bonusCoroutine != null)
        {
            StopCoroutine(_bonusCoroutine);
            _bonusCoroutine = null;
        }

        Debug.Log("[BonusStage] Bonus finished!");

        StartCoroutine(ReturnToMapRoutine());
    }
   
    private IEnumerator ReturnToMapRoutine()
    {
        yield return new WaitForSeconds(returnDelay);

        if (mapScene == null)
        {
            Debug.LogError(
                "[BonusStage] Map scene is not assigned.",
                this
            );
            yield break;
        }

        string sceneName = mapScene.name;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[BonusStage] Scene '{sceneName}' is not in the build.",
                this
            );
            yield break;
        }

        // Save level 2 completion.
        LevelProgressManager.CompleteLevel(2);

        Debug.Log("[BonusStage] Level 2 completed!");

        SceneManager.LoadScene(sceneName);
    }

}
