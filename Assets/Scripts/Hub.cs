using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private GameManager _gameManager; 
    public TextMeshProUGUI pointsText; 

    public GameObject[] healthBar;
    
    void Start()
    {
        // 1. Busca el GameManager en la escena
        _gameManager = Object.FindFirstObjectByType<GameManager>();

        // 2. Si lo encuentra, fuerza la primera actualización con los puntos actuales (0)
        if (_gameManager != null)
        {
            UpdatePointsText();
        }
    }

    // Método que el GameManager llamará cada vez que sumes puntos
    public void UpdatePointsText()
    {
        if (pointsText != null && _gameManager != null)
        {
            pointsText.text = "Points: " + _gameManager.TotalPoints.ToString();
        }
    }

    public void DisableHealth(int index)
    {
        healthBar[index].SetActive(false);
    }

    public void EnableHealth(int index)
    {
        healthBar[index].SetActive(true);
    }


}

