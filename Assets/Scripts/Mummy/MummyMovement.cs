
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

    [Header("Player Distance Logs")]
    [SerializeField] private Transform player;
    [SerializeField] private float closeDistance = 2f;
    [SerializeField] private float logInterval = 1f;

    private Transform targetPoint;
    private SpriteRenderer spriteRenderer;
    private float logTimer;
    private bool playerWasClose;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError(
                    "MummyMovement: No se encontró un objeto con Tag 'Player'.",
                    this
                );
            }
        }

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError(
                "MummyMovement: Asigna Start Point y End Point.",
                this
            );

            enabled = false;
            return;
        }

        transform.position = startPoint.position;
        targetPoint = endPoint;

        UpdateSpriteDirection();
    }

    private void Update()
    {
        MoveBetweenPoints();
        LogPlayerDistance();
    }

    private void MoveBetweenPoints()
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

    private void LogPlayerDistance()
    {
        if (player == null)
        {
            return;
        }

        // Distancia entre la momia y el jugador en el plano XY.
        Vector2 mummyPosition = transform.position;
        Vector2 playerPosition = player.position;

        float distance = Vector2.Distance(
            mummyPosition,
            playerPosition
        );

        logTimer += Time.deltaTime;

        if (logTimer >= logInterval)
        {
            Debug.Log(
                $"[MummyMovement] Distancia al Player: {distance:F2} unidades.",
                this
            );

            logTimer = 0f;
        }

        bool playerIsClose = distance <= closeDistance;

        if (playerIsClose && !playerWasClose)
        {
            Debug.Log(
                $"[MummyMovement] ¡Player cerca! Distancia: {distance:F2} unidades.",
                this
            );
        }

        playerWasClose = playerIsClose;
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