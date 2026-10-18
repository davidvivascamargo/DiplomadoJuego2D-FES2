using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Temporarily tests cinematic start and end.
/// F6 starts the cinematic.
/// F7 ends the cinematic.
/// </summary>
public class CinematicTester : MonoBehaviour
{
    private void Update()
    {
        if (CinematicManager.Instance == null)
            return;

        if (Keyboard.current == null)
            return;

        // Start cinematic
        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            CinematicManager.Instance.StartCinematic();
        }

        // End cinematic
        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            CinematicManager.Instance.EndCinematic();
        }
    }
}
