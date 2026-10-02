using UnityEngine;

/// <summary>
/// Controls the enemy's attack behavior when the player is within attack range.
/// </summary>
public class EnemyCombat : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    private Animator _animator;
    private float _lastAttackTime = -Mathf.Infinity;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Mathf.Abs(
            playerTransform.position.x - transform.position.x
        );

        if (distanceToPlayer > attackRange)
        {
            return;
        }

        if (Time.time < _lastAttackTime + attackCooldown)
        {
            return;
        }

        Attack();
    }

    private void Attack()
    {
        _lastAttackTime = Time.time;

        if (_animator == null)
        {
            return;
        }

        bool hasAttackParameter = false;

        foreach (AnimatorControllerParameter parameter in _animator.parameters)
        {
            if (parameter.name == "Attack")
            {
                hasAttackParameter = true;
                break;
            }
        }

        if (!hasAttackParameter)
        {
            return;
        }

        _animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Applies damage to the player when the attack animation reaches the hit frame.
    /// </summary>
    public void DealDamage()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReduceHealth();
        }
    }
}