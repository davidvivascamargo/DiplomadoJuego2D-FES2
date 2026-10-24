
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

    private readonly Collider2D[] results = new Collider2D[32];

    private void Awake()
    {
        explosionCollider = GetComponent<CircleCollider2D>();

        explosionCollider.isTrigger = true;
        explosionCollider.radius = explosionRadius;
        explosionCollider.enabled = true;
    }

    private void Start()
    {
        CheckExplosionDamage();

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
        if (other == null)
            return;

        GameObject player = GetPlayer(other);

        if (player == null)
            return;

        if (damagedPlayers.Contains(player))
            return;

        if (GameManager.Instance == null)
            return;

        damagedPlayers.Add(player);

        GameManager.Instance.ReduceHealth();
    }

    private GameObject GetPlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return other.gameObject;

        if (other.attachedRigidbody != null)
        {
            GameObject root =
                other.attachedRigidbody.gameObject;

            if (root.CompareTag("Player"))
                return root;
        }

        Transform current = other.transform.parent;

        while (current != null)
        {
            if (current.CompareTag("Player"))
                return current.gameObject;

            current = current.parent;
        }

        return null;
    }
}
