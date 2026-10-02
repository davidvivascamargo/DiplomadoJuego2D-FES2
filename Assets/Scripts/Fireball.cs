using UnityEngine;

/// <summary>
/// Controls the movement and collision behavior of the player's fireball.
/// </summary>
public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    /// <summary>
    /// Sets the horizontal direction of the fireball.
    /// </summary>
    public void SetDirection(float direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                speed * direction,
                0f
            );
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    /// <summary>
    /// Handles collisions with enemies and other objects.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            return;
        }

        EnemyHealth enemyHealth = hitInfo.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(1);
        }

        Destroy(gameObject);
    }
}