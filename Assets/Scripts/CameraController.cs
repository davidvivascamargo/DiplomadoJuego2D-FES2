using UnityEngine;

/// <summary>
/// Controls the camera position based on the player's horizontal position.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerTransform;

    private float _cameraSize;
    private float _screenHeight;

    private void Start()
    {
        _cameraSize = Camera.main.orthographicSize;
        _screenHeight = _cameraSize * 2f;
    }

    private void Update()
    {
        CalculateCameraPosition();
    }

    /// <summary>
    /// Positions the camera at the center of the current screen section.
    /// </summary>
    private void CalculateCameraPosition()
    {
        int playerScreen =
            (int)(playerTransform.position.x / _screenHeight);

        float screenSize =
            (playerScreen * _screenHeight) + _cameraSize;

        transform.position = new Vector3(
            screenSize,
            transform.position.y,
            transform.position.z
        );
    }
}