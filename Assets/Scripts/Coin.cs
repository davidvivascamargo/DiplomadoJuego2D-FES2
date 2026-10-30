
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = Object.FindFirstObjectByType<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError(
                "[Coin] GameManager not found!"
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (_gameManager != null)
        {
            _gameManager.AddPoints(coinValue);
        }

        Destroy(gameObject);
    }
}
