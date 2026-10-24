
using UnityEngine;

/// <summary>
/// Controls the mummy's patrol movement between two points.
/// </summary>
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
    private bool isMovementStopped;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (startPoint == null || endPoint == null)
        {
            enabled = false;
            return;
        }

        transform.position = startPoint.position;
        targetPoint = endPoint;

        UpdateSpriteDirection();
    }

    private void Update()
    {
        if (isMovementStopped || targetPoint == null)
        {
            return;
        }

        MoveBetweenPoints();
    }

    private void MoveBetweenPoints()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
            transform.position,
            targetPoint.position
        ) < 0.01f)
        {
            targetPoint = targetPoint == startPoint
                ? endPoint
                : startPoint;

            UpdateSpriteDirection();
        }
    }

    /// <summary>
    /// Stops the mummy when preparing to explode.
    /// </summary>
    public void StopMovement()
    {
        isMovementStopped = true;
    }

    /// <summary>
    /// Resumes patrol if the player escapes.
    /// </summary>
    public void ResumeMovement()
    {
        isMovementStopped = false;
    }

    private void UpdateSpriteDirection()
    {
        if (spriteRenderer == null || !flipSprite)
        {
            return;
        }

        spriteRenderer.flipX = targetPoint == startPoint;
    }
}
