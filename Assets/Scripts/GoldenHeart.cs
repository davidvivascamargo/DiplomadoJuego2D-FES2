
using UnityEngine;

public class GoldenHeart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
            return;

        bool wasHealed = GameManager.Instance.RestoreFullHealth();

        // Consume only when health was restored.
        if (wasHealed)
        {
            Debug.Log("[GoldenHeart] Player fully healed!");
            Destroy(gameObject);
        }
    }
}
