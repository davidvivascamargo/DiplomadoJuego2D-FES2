
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoubleJumpPowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [SerializeField, Min(0.1f)]
    private float effectDuration = 10f;

    [SerializeField, Min(1)]
    private int totalJumps = 2;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerController player =
            collision.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        player.ActivateDoubleJump(effectDuration, totalJumps);

        Debug.Log(
            $"[DoubleJumpPowerUp] Activated: " +
            $"{totalJumps} jumps for {effectDuration} seconds."
        );

        Destroy(gameObject);
    }
}
