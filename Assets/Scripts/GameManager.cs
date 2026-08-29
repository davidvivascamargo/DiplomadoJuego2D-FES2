using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Propiedad pública para leer los puntos desde otros scripts
    public int TotalPoints { get { return _totalPoints; } }
    
    // Variable privada que almacena el valor real
    private int _totalPoints;

    // Referencia al script del HUD
    private HUD _hud;

    void Start()
    {
        // Busca automáticamente el HUD en la escena al iniciar la partida
        _hud = Object.FindFirstObjectByType<HUD>();
    }

    public void AddPoints(int points)
    {
        _totalPoints += points;
        Debug.Log("Total points: " + _totalPoints);

        // Le dice al HUD que actualice el texto en la pantalla si lo encuentra
        if (_hud != null)
        {
            _hud.UpdatePointsText();
        }
    }
}

