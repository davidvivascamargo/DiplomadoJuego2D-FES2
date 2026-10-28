
using System.Collections;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject potionPrefab;

    [Header("Spawn Settings")]
    [SerializeField, Min(0.1f)]
    private float spawnInterval = 10f;

    [SerializeField] private Transform spawnPoint;

    [SerializeField] private bool spawnOnStart = true;

    [Header("Detection Settings")]
    [SerializeField, Min(0.1f)]
    private float detectionRadius = 0.5f;

    [SerializeField] private LayerMask potionLayer;

    private GameObject spawnedItem;

    private void Start()
    {
        if (spawnOnStart)
        {
            TrySpawnPotion();
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            TrySpawnPotion();
        }
    }

    private void TrySpawnPotion()
    {
        if (potionPrefab == null)
        {
            Debug.LogWarning("[PotionSpawner] Item prefab is missing.");
            return;
        }

        // Do not create another item while the previous one exists.
        if (spawnedItem != null)
            return;

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        // Detect an existing item in the spawn area.
        Collider2D existingItem = Physics2D.OverlapCircle(
            position,
            detectionRadius,
            potionLayer
        );

        if (existingItem != null)
        {
            Debug.Log("[PotionSpawner] Item already exists.");
            return;
        }

        spawnedItem = Instantiate(
            potionPrefab,
            position,
            Quaternion.identity
        );

        Debug.Log("[PotionSpawner] Item spawned.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        Gizmos.DrawWireSphere(position, detectionRadius);
    }
}
