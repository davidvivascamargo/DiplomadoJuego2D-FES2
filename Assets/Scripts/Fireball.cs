
using UnityEngine;

/// <summary>
/// Controls the movement and collision behavior of the player's fireball.
/// </summary>
public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Damage")]
    [SerializeField, Min(1)] private int damage = 1;

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

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

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            return;
        }

        // Damage regular enemies.
        EnemyHealth enemyHealth =
            hitInfo.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Damage flying enemies.
        WraithHealth wraithHealth =
            hitInfo.GetComponentInParent<WraithHealth>();

        if (wraithHealth != null)
        {
            wraithHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Damage bonus stage barrels.
        BonusBarrel bonusBarrel =
            hitInfo.GetComponentInParent<BonusBarrel>();

        if (bonusBarrel != null)
        {
            bonusBarrel.TakeDamage(damage);

            Debug.Log(
                "[Fireball] Bonus barrel hit!",
                bonusBarrel
            );

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
