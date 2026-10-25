
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CrushingBlockTrap : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionDistance = 20f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;

    [Header("Movement")]
    [SerializeField] private float fallSpeed = 20f;
    [SerializeField] private float riseSpeed = 3f;

    [Header("Timers")]
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float groundWaitTime = 4f;
    [SerializeField] private float cooldown = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Collider")]
    [SerializeField] private bool autoResizeCollider = true;

    private Rigidbody2D rb;
    private BoxCollider2D blockCollider;
    private SpriteRenderer spriteRenderer;

    private Vector2 initialPosition;
    private bool isActive;
    private bool isFalling;
    private bool playerKilled;

    private Vector2 previousColliderSize;
    private Vector2 previousColliderOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        blockCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;
        rb.simulated = true;

        blockCollider.isTrigger = false;
    }

    private void Start()
    {
        initialPosition = rb.position;
        PlayAnimation("CrushingBlock_Idle");
    }

    private void UpdateCollider()
    {
        if (spriteRenderer.sprite == null)
            return;

        Bounds spriteBounds = spriteRenderer.sprite.bounds;

        Vector2 newSize = spriteBounds.size;
        Vector2 newOffset = spriteBounds.center;

        if (newSize == previousColliderSize &&
            newOffset == previousColliderOffset)
            return;

        blockCollider.size = newSize;
        blockCollider.offset = newOffset;

        previousColliderSize = newSize;
        previousColliderOffset = newOffset;
    }

    private void Update()
    {
        if (!isActive && DetectPlayer())
            StartCoroutine(ActivateTrap());
    }

    private bool DetectPlayer()
    {
        Bounds bounds = blockCollider.bounds;

        Vector2 origin = new Vector2(
            bounds.center.x,
            bounds.min.y - 0.05f
        );

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            new Vector2(bounds.size.x * 0.9f, 0.05f),
            0f,
            Vector2.down,
            detectionDistance,
            playerLayer
        );

        return hit.collider != null &&
               hit.collider.CompareTag("Player");
    }

    private IEnumerator ActivateTrap()
    {
        isActive = true;
        playerKilled = false;

        Debug.Log("CrushingBlockTrap: Player detected.");

        // WARNING
        PlayAnimation("CrushingBlock_Warning");

        yield return new WaitForSeconds(
            Mathf.Max(0f, warningDuration)
        );

        // FALL
        isFalling = true;
        PlayAnimation("CrushingBlock_Fall");

        Debug.Log("CrushingBlockTrap: Starting fall.");

        while (isFalling)
        {
            yield return new WaitForFixedUpdate();

            float step =
                Mathf.Max(0.01f, fallSpeed) *
                Time.fixedDeltaTime;

            Bounds bounds = blockCollider.bounds;

            Vector2 groundOrigin = new Vector2(
                bounds.center.x,
                bounds.min.y + 0.02f
            );

            Vector2 groundBoxSize = new Vector2(
                bounds.size.x * 0.9f,
                0.02f
            );

            RaycastHit2D groundHit = Physics2D.BoxCast(
                groundOrigin,
                groundBoxSize,
                0f,
                Vector2.down,
                step + groundCheckDistance,
                groundLayer
            );

            RaycastHit2D playerHit = Physics2D.BoxCast(
                bounds.center,
                bounds.size,
                0f,
                Vector2.down,
                step + groundCheckDistance,
                playerLayer
            );

            if (playerHit.collider != null &&
                playerHit.collider.CompareTag("Player"))
            {
                KillPlayer();
            }

            if (groundHit.collider != null)
            {
                float moveDistance = Mathf.Max(0f, groundHit.distance);

                rb.MovePosition(
                    rb.position + Vector2.down * moveDistance
                );

                Debug.Log(
                    "CrushingBlockTrap: Ground detected: " +
                    groundHit.collider.name +
                    " | Distance: " +
                    groundHit.distance
                );

                isFalling = false;
            }
            else
            {
                rb.MovePosition(
                    rb.position + Vector2.down * step
                );
            }
        }

        // IMPACT
        PlayAnimation("CrushingBlock_Impact");

        Debug.Log("CrushingBlockTrap: Impact.");

        yield return new WaitForSeconds(
            Mathf.Max(0f, groundWaitTime)
        );

        // RISE
        Debug.Log("CrushingBlockTrap: Rising.");

        // Reutiliza la animación Fall sin velocidad negativa.
        // El bloque asciende físicamente con MovePosition.
        PlayAnimation("CrushingBlock_Fall");

        while (Vector2.Distance(
            rb.position,
            initialPosition
        ) > 0.01f)
        {
            yield return new WaitForFixedUpdate();

            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                initialPosition,
                Mathf.Max(0.01f, riseSpeed) *
                Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);
        }

        rb.MovePosition(initialPosition);

        PlayAnimation("CrushingBlock_Idle");

        yield return new WaitForSeconds(
            Mathf.Max(0f, cooldown)
        );

        isActive = false;

        Debug.Log("CrushingBlockTrap: Ready.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling || playerKilled)
            return;

        if (collision.gameObject.CompareTag("Player"))
            KillPlayer();
    }

    private void KillPlayer()
    {
        if (playerKilled)
            return;

        playerKilled = true;

        Debug.Log("CrushingBlockTrap: Player crushed.");

        if (GameManager.Instance != null)
            GameManager.Instance.KillPlayer();
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null)
            return;

        animator.speed = 1f;
        animator.Play(animationName, 0, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        if (col == null)
            return;

        Bounds bounds = col.bounds;

        Vector3 origin = new Vector3(
            bounds.center.x,
            bounds.min.y,
            bounds.center.z
        );

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            origin,
            origin + Vector3.down * detectionDistance
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            bounds.center,
            bounds.size
        );
    }
}
