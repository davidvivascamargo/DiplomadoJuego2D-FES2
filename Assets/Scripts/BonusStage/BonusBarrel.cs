
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BonusBarrel : MonoBehaviour
{
    [Header("Barrel Settings")]
    [SerializeField, Min(1)] private int maxHealth = 3;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string breakTrigger = "Break";
    [SerializeField] private float destroyDelay = 0.7f;

    [Header("Rewards")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject diamondPrefab;

    [SerializeField, Min(1)] private int minRewards = 8;
    [SerializeField, Min(1)] private int maxRewards = 15;

    [SerializeField, Range(0f, 1f)]
    private float diamondChance = 0.25f;

    [SerializeField] private Vector2 spawnOffset =
        new Vector2(0f, 0.5f);

    private int _currentHealth;
    private bool _isDestroyed;

    private void Awake()
    {
        _currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (_isDestroyed || damage <= 0)
            return;

        _currentHealth = Mathf.Max(
            0,
            _currentHealth - damage
        );

        Debug.Log(
            $"[BonusBarrel] Health: {_currentHealth}",
            this
        );

        if (_currentHealth <= 0)
            BreakBarrel();
    }

    private void BreakBarrel()
    {
        if (_isDestroyed)
            return;

        _isDestroyed = true;

        GetComponent<Collider2D>().enabled = false;

        if (animator != null)
            animator.SetTrigger(breakTrigger);

        SpawnRewards();

        StartCoroutine(DestroyRoutine());
    }

    private void SpawnRewards()
    {
        int min = Mathf.Min(minRewards, maxRewards);
        int max = Mathf.Max(minRewards, maxRewards);

        int amount = Random.Range(min, max + 1);

        for (int i = 0; i < amount; i++)
        {
            bool spawnDiamond =
                Random.value < diamondChance;

            GameObject prefab = spawnDiamond
                ? diamondPrefab
                : coinPrefab;

            if (prefab == null)
                continue;

            Vector3 position =
                transform.position +
                (Vector3)spawnOffset;

            GameObject reward = Instantiate(
                prefab,
                position,
                Quaternion.identity
            );

            BonusReward bonusReward =
                reward.GetComponent<BonusReward>();

            if (bonusReward != null)
                bonusReward.Launch();
        }
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}
