
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ArrowProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 shootDirection)
    {
        if (shootDirection.sqrMagnitude < 0.001f)
            shootDirection = Vector2.right;

        direction = shootDirection.normalized;

        // Rotate the projectile to face its movement direction.
        float angle = Mathf.Atan2(direction.y, direction.x)
                      * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FixedUpdate()
    {
        if (hasHit)
            return;

        rb.MovePosition(
            rb.position + direction * speed * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit || collision.isTrigger)
            return;

        if (collision.CompareTag("Player"))
        {
            hasHit = true;

            if (GameManager.Instance != null)
            {
                for (int i = 0; i < damage; i++)
                    GameManager.Instance.ReduceHealth();
            }

            Destroy(gameObject);
            return;
        }

        if (collision.GetComponent<ArrowProjectile>() != null)
            return;

        hasHit = true;
        Destroy(gameObject);
    }
}
