
using UnityEngine;

public class SpeedPotion : MonoBehaviour
{
    [Header("Speed Potion Settings")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float speedMultiplier = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        player.ActivateSpeedBoost(
            duration,
            speedMultiplier
        );

        Destroy(gameObject);
    }
}
