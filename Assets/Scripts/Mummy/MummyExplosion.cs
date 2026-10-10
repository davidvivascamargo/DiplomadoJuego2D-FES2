
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the mummy's self-destruction behavior.
/// </summary>
[RequireComponent(typeof(MummyMovement))]
public class MummyExplosion : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 5f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform explosionPoint;
    [SerializeField] private float explosionDelay = 0.6f;
    [SerializeField] private float explosionLifetime = 3f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTrigger = "Die";

    private MummyMovement movement;
    private bool isExploding;

    private void Awake()
    {
        movement = GetComponent<MummyMovement>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (isExploding || player == null)
        {
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRadius)
        {
            StartCoroutine(ExplodeSequence());
        }
    }

    private IEnumerator ExplodeSequence()
    {
        isExploding = true;

        // Stop patrolling immediately.
        movement.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Play the death animation.
        if (animator != null)
        {
            animator.SetTrigger(deathTrigger);
        }

        Debug.Log(
            "[MummyExplosion] Explosion sequence started.",
            this
        );

        // Wait before spawning the explosion.
        yield return new WaitForSeconds(explosionDelay);

        Vector3 position = explosionPoint != null
            ? explosionPoint.position
            : transform.position;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                position,
                Quaternion.identity
            );

            Destroy(explosion, explosionLifetime);
        }
        else
        {
            Debug.LogWarning(
                "[MummyExplosion] Explosion prefab is missing.",
                this
            );
        }

        Debug.Log(
            "[MummyExplosion] Mummy exploded.",
            this
        );

        // Remove the mummy after the explosion.
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}
