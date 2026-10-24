
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ExplosionDamage : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField, Min(0.1f)] private float explosionRadius = 2f;
    [SerializeField, Min(0.1f)] private float explosionLifetime = 1f;

    private CircleCollider2D explosionCollider;
    private readonly HashSet<GameObject> damagedPlayers = new();

    private void Awake()
    {
        explosionCollider = GetComponent<CircleCollider2D>();

        explosionCollider.isTrigger = true;
        explosionCollider.radius = explosionRadius;
    }

    private void Start()
    {
        // Check players already inside the explosion radius.
        CheckExplosionDamage();

        // Remove the explosion after its lifetime.
        Destroy(gameObject, explosionLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ApplyDamage(other);
    }

    private void CheckExplosionDamage()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[20];

        int count = explosionCollider.Overlap(
            filter,
            results
        );

        for (int i = 0; i < count; i++)
        {
            ApplyDamage(results[i]);
        }
    }

    private void ApplyDamage(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        GameObject player = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (damagedPlayers.Contains(player))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            return;
        }

        damagedPlayers.Add(player);

        GameManager.Instance.ReduceHealth();
    }
}
