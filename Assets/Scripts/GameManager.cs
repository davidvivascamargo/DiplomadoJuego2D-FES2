
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Points
    public int TotalPoints => _totalPoints;
    private int _totalPoints;

    private const string PointsSaveKey = "TotalPoints";

    // HUD
    private HUD _hud;

    // Player health
    [Header("Player Health")]
    [SerializeField] private int maxHealth = 10;
    private int _playerHealth;

    // Invulnerability
    [Header("Player Invulnerability")]
    [SerializeField] private float blinkInterval = 0.15f;

    private bool isInvulnerable;
    private Coroutine invulnerabilityCoroutine;
    private SpriteRenderer[] playerRenderers;

    public bool IsInvulnerable => isInvulnerable;

   private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _playerHealth = maxHealth;

        _totalPoints = PlayerPrefs.GetInt(PointsSaveKey, 0);

        Debug.Log($"[GameManager] Loaded points: {_totalPoints}");
    }
    private void Start()
    {
        _hud = Object.FindFirstObjectByType<HUD>();
    }

    public void AddPoints(int points)
    {
        _totalPoints += points;

        if (_hud != null)
        {
            _hud.UpdatePointsText();
        }
    }

    public void ReduceHealth()
    {
        if (isInvulnerable || _playerHealth <= 0)
            return;

        _playerHealth--;

        if (_hud != null)
        {
            _hud.DisableHealth(_playerHealth);
        }

        if (_playerHealth <= 0)
        {
            RestartLevel();
        }
    }

    public bool RestoreHealth()
    {
        if (_playerHealth >= maxHealth)
            return false;

        _playerHealth++;

        if (_hud != null)
        {
            _hud.EnableHealth(_playerHealth - 1);
        }

        return true;
    }

    public void KillPlayer()
    {
        if (_playerHealth <= 0)
            return;

        _playerHealth = 0;
        RestartLevel();
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ActivateInvulnerability(
        float duration,
        GameObject player)
    {
        if (duration <= 0f || player == null)
            return;

        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
            RestorePlayerVisibility();
        }

        playerRenderers =
            player.GetComponentsInChildren<SpriteRenderer>();

        isInvulnerable = true;

        invulnerabilityCoroutine = StartCoroutine(
            InvulnerabilityCoroutine(duration)
        );
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < duration)
        {
            visible = !visible;

            foreach (SpriteRenderer renderer in playerRenderers)
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        RestorePlayerVisibility();

        isInvulnerable = false;
        invulnerabilityCoroutine = null;
    }

    private void RestorePlayerVisibility()
    {
        if (playerRenderers == null)
            return;

        foreach (SpriteRenderer renderer in playerRenderers)
        {
            if (renderer != null)
                renderer.enabled = true;
        }
    }


    public bool RestoreFullHealth()
    {
        const int maxHealth = 10;

        if (_playerHealth >= maxHealth)
            return false;

        _playerHealth = maxHealth;

        HUD hud = FindFirstObjectByType<HUD>();

        if (hud != null)
        {
            hud.RestoreAllHearts();
        }
        else
        {
            Debug.LogWarning("[GameManager] HUD not found!");
        }

        Debug.Log("[GameManager] Player fully healed.");

        return true;
    }

}
