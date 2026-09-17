using UnityEngine;

public class Heart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Try to restore player health
            bool wasHealed = GameManager.Instance.RestoreHealth();

            // Destroy the pickup only if health was restored
            if (wasHealed)
            {
                Destroy(gameObject);
            }
        }
    }
}
