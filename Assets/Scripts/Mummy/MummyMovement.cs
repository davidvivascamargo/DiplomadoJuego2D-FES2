
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MummyMovement : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Sprite")]
    [SerializeField] private bool flipSprite = true;

    private Transform targetPoint;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D mummyCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mummyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Validate patrol points.
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError(
                "[MummyMovement] Assign Start Point and End Point.",
                this
            );

            enabled = false;
            return;
        }

        // Initialize patrol.
        rb.position = startPoint.position;
        targetPoint = endPoint;

        UpdateSpriteDirection();
    }

    private void FixedUpdate()
    {
        MoveBetweenPoints();
    }

    /// <summary>
    /// Moves the mummy between patrol points using 2D physics.
    /// Stops movement when the player blocks its path.
    /// </summary>
    private void MoveBetweenPoints()
    {
        if (targetPoint == null)
        {
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 destination = targetPoint.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            destination,
            moveSpeed * Time.fixedDeltaTime
        );

        Vector2 movement = nextPosition - currentPosition;

        if (movement.sqrMagnitude > 0.000001f &&
            mummyCollider != null)
        {
            // Check for solid obstacles in the movement direction.
            RaycastHit2D[] hits = new RaycastHit2D[8];

            int hitCount = mummyCollider.Cast(
                movement.normalized,
                hits,
                movement.magnitude + 0.02f,
                true
            );

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = hits[i].collider;

                if (hitCollider == null || hitCollider.isTrigger)
                {
                    continue;
                }

                bool isPlayer =
                    hitCollider.CompareTag("Player") ||
                    hitCollider.transform.root.CompareTag("Player");

                if (isPlayer)
                {
                    // Stop before entering the player's collider.
                    return;
                }
            }
        }

        // Move through the physics system.
        rb.MovePosition(nextPosition);

        // Switch patrol direction when reaching a point.
        if (Vector2.Distance(nextPosition, destination) < 0.01f)
        {
            targetPoint = targetPoint == startPoint
                ? endPoint
                : startPoint;

            UpdateSpriteDirection();
        }
    }

    /// <summary>
    /// Updates sprite orientation based on the patrol destination.
    /// </summary>
    private void UpdateSpriteDirection()
    {
        if (spriteRenderer == null || !flipSprite || targetPoint == null)
        {
            return;
        }

        float directionX =
            targetPoint.position.x - rb.position.x;

        if (Mathf.Abs(directionX) < 0.01f)
        {
            return;
        }

        // The original sprite is assumed to face right.
        spriteRenderer.flipX = directionX < 0f;
    }
}
