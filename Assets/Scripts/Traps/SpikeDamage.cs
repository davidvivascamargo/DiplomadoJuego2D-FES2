
using UnityEngine;

/// <summary>
/// Instantly kills the player when touching spikes.
/// </summary>
public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
            return;

        GameManager.Instance.KillPlayer();
    }
}
