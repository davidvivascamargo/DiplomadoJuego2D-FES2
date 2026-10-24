
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MummyMovement : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 1.5f;

    [Header("Player Collision")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField, Min(0f)] private float collisionMargin = 0.05f;

    [Header("Sprite")]
    [SerializeField] private bool flipSprite = true;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private SpriteRenderer spriteRenderer;

    private Transform targetPoint;
    private bool isMovementStopped;

    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            enabled = false;
            return;
        }

        rb.position = startPoint.position;
        targetPoint = endPoint;

        UpdateSpriteDirection();
    }

    private void FixedUpdate()
    {
        if (isMovementStopped || targetPoint == null)
            return;

        MoveBetweenPoints();
    }

    private void MoveBetweenPoints()
    {
        Vector2 destination = targetPoint.position;

        // Patrol only along the horizontal axis.
        float distanceX = destination.x - rb.position.x;

        if (Mathf.Abs(distanceX) <= 0.01f)
        {
            ChangeTarget();
            return;
        }

        float directionX = Mathf.Sign(distanceX);
        float step = Mathf.Min(
            moveSpeed * Time.fixedDeltaTime,
            Mathf.Abs(distanceX)
        );

        Vector2 direction = new Vector2(directionX, 0f);

        if (IsPlayerBlocking(direction, step))
            return;

        Vector2 nextPosition = rb.position +
            direction * step;

        rb.MovePosition(nextPosition);

        if (Mathf.Abs(destination.x - nextPosition.x) <= 0.01f)
            ChangeTarget();
    }

    private bool IsPlayerBlocking(Vector2 direction, float step)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        filter.useTriggers = false;

        int count = bodyCollider.Cast(
            direction,
            filter,
            hits,
            step + collisionMargin
        );

        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider != null)
                return true;
        }

        return false;
    }

    private void ChangeTarget()
    {
        targetPoint = targetPoint == startPoint
            ? endPoint
            : startPoint;

        UpdateSpriteDirection();
    }

    public void StopMovement()
    {
        isMovementStopped = true;
    }

    public void ResumeMovement()
    {
        isMovementStopped = false;
    }

    private void UpdateSpriteDirection()
    {
        if (spriteRenderer == null || !flipSprite)
            return;

        spriteRenderer.flipX = targetPoint == startPoint;
    }
}
