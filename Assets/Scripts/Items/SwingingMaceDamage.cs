
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SwingingMaceDamage : MonoBehaviour
{
    private bool hasHitPlayer;

    private void Awake()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer)
            return;

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        hasHitPlayer = true;

        Debug.Log("[SwingingMace] Player killed!");

        GameManager gameManager =
            FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.KillPlayer();
        }
    }
}
