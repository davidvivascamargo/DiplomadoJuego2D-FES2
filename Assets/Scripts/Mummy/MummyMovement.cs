using UnityEngine;

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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("MummyMovement: Asigna Start Point y End Point.");
            enabled = false;
            return;
        }

        transform.position = startPoint.position;
        targetPoint = endPoint;

        UpdateSpriteDirection();
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            targetPoint = targetPoint == startPoint
                ? endPoint
                : startPoint;

            UpdateSpriteDirection();
        }
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