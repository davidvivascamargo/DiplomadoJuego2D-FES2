
using System.Collections;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    [Header("Potion Settings")]
    [SerializeField] private GameObject potionPrefab;

    [Header("Spawn Settings")]
    [SerializeField, Min(0.1f)]
    private float spawnInterval = 10f;

    [SerializeField] private Transform spawnPoint;

    [Header("Detection Settings")]
    [SerializeField, Min(0.1f)]
    private float detectionRadius = 0.5f;

    [SerializeField] private LayerMask potionLayer;

    private Coroutine spawnCoroutine;

    private void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
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
            Debug.LogWarning("[PotionSpawner] Potion prefab is missing.");
            return;
        }

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        Collider2D existingPotion = Physics2D.OverlapCircle(
            position,
            detectionRadius,
            potionLayer
        );

        if (existingPotion != null)
        {
            Debug.Log("[PotionSpawner] Potion already exists.");
            return;
        }

        Instantiate(potionPrefab, position, Quaternion.identity);

        Debug.Log("[PotionSpawner] Potion spawned.");
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
