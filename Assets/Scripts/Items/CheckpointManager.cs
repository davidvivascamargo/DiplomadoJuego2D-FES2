
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private static Vector3 savedSpawnPosition;
    private static string savedSceneName;
    private static bool hasCheckpoint;

    // Reset checkpoints when starting a new Play session.
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession()
    {
        hasCheckpoint = false;
        savedSpawnPosition = Vector3.zero;
        savedSceneName = null;
        Instance = null;

        Debug.Log("[CheckpointManager] New session started.");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        string currentScene = SceneManager.GetActiveScene().name;

        // Clear checkpoints from another scene.
        if (hasCheckpoint && savedSceneName != currentScene)
        {
            ClearCheckpoint();
        }

        Debug.Log("[CheckpointManager] Initialized.");
    }

    private void Start()
    {
        if (!hasCheckpoint)
        {
            Debug.Log(
                "[CheckpointManager] No checkpoint. Using initial spawn."
            );
            return;
        }

        RespawnPlayer();
    }

    public void ActivateCheckpoint(Vector3 position)
    {
        savedSpawnPosition = position;
        savedSceneName = SceneManager.GetActiveScene().name;
        hasCheckpoint = true;

        Debug.Log(
            "[Checkpoint] Activated! Position: " + position
        );
    }

    private void RespawnPlayer()
    {
        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            Debug.LogWarning(
                "[CheckpointManager] Player not found."
            );
            return;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = new Vector2(
                savedSpawnPosition.x,
                savedSpawnPosition.y
            );

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        player.transform.position = savedSpawnPosition;

        Debug.Log(
            "[Checkpoint] Player respawned at: " +
            savedSpawnPosition
        );
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public Vector3 GetSpawnPosition()
    {
        return savedSpawnPosition;
    }

    public static void ClearCheckpoint()
    {
        hasCheckpoint = false;
        savedSpawnPosition = Vector3.zero;
        savedSceneName = null;

        Debug.Log("[CheckpointManager] Checkpoint cleared.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
