
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Transform spawnPoint;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D checkpointCollider;
    private bool isActivated;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        checkpointCollider = GetComponent<BoxCollider2D>();

        checkpointCollider.isTrigger = true;

        if (inactiveSprite != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }

        Debug.Log("[Checkpoint] Initialized: " + gameObject.name);
    }

    private void Start()
    {
        if (CheckpointManager.Instance == null)
        {
            Debug.LogError(
                "[Checkpoint] CheckpointManager not found!",
                this
            );
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "[Checkpoint] SpawnPoint not assigned!",
                this
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "[Checkpoint] Collision detected: " + other.name,
            this
        );

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
        {
            Debug.LogWarning(
                "[Checkpoint] Object is not Player: " + other.name
            );
            return;
        }

        Debug.Log("[Checkpoint] Player detected!");

        if (isActivated)
        {
            Debug.Log("[Checkpoint] Already activated.");
            return;
        }

        if (CheckpointManager.Instance == null)
        {
            Debug.LogError(
                "[Checkpoint] Cannot activate. Manager not found!"
            );
            return;
        }

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        CheckpointManager.Instance.ActivateCheckpoint(position);

        isActivated = true;

        if (activeSprite != null)
        {
            spriteRenderer.sprite = activeSprite;
        }

        Debug.Log(
            "[Checkpoint] Successfully activated at: " + position
        );
    }
}
