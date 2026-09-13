using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Puntos
    public int TotalPoints => _totalPoints;

    private int _totalPoints;

    // HUD
    private HUD _hud;

    // Vida del jugador
    private int _playerHealth = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _hud = Object.FindFirstObjectByType<HUD>();
    }

    public void AddPoints(int points)
    {
        _totalPoints += points;

        Debug.Log("Total points: " + _totalPoints);

        if (_hud != null)
        {
            _hud.UpdatePointsText();
        }
    }

    public void ReduceHealth()
    {
        _playerHealth--;

        if (_hud != null)
        {
            _hud.DisableHealth(_playerHealth);
        }

        if (_playerHealth <= 0)
        {
            Debug.Log("Player has died.");
            // Aquí posteriormente agregaremos la lógica de muerte.
        }
    }

    public void RestoreHealth()
    {
        if (_playerHealth < 3)
        {
            _playerHealth++;

            if (_hud != null)
            {
                _hud.EnableHealth(_playerHealth - 1);
            }
        }
    }
}

