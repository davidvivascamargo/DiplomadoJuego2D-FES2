using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    private GameManager _gameManager;    

    void Start()
    {
        // Busca automáticamente el objeto GameManager en la escena al iniciar
        _gameManager = Object.FindFirstObjectByType<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("¡No se encontró ningún GameManager en la escena!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que tocó la moneda tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            if (_gameManager != null)
            {
                // Se agregó el punto (.) para llamar correctamente al método
                _gameManager.AddPoints(coinValue); 
            }

            // Destruye la moneda
            Destroy(gameObject);
        }
    }
}
