using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Referencia al transform del jugador
    public Transform playerTransform; 

    // Variables privadas con guion bajo (_)
    private float _cameraSize; 
    private float _screenHeight; 

    void Start()
    {
        _cameraSize = Camera.main.orthographicSize;
        _screenHeight = _cameraSize * 2f; 
    }

    void Update()
    {
        CalculateCameraPosition();
    }

    // Se corrigió a PascalCase (C mayúscula) para seguir las normas de C#
    void CalculateCameraPosition()
    {
        int playerScreen = (int)(playerTransform.position.x / _screenHeight);
        float screenSize = (playerScreen * _screenHeight) + _cameraSize;
        transform.position = new Vector3(screenSize, transform.position.y, transform.position.z);
    }
}
