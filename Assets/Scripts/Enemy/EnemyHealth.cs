using UnityEngine;

/// <summary>
/// Controls the enemy's health and death behavior.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Drop")]
    [SerializeField] private GameObject dropItemPrefab;

    private int _currentHealth;
    private bool _isDead;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Collider2D[] _colliders;
    private EnemyMovement _enemyMovement;
    private EnemyCombat _enemyCombat;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _colliders = GetComponentsInChildren<Collider2D>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyCombat = GetComponent<EnemyCombat>();

        _currentHealth = Mathf.Max(1, maxHealth);
    }

    /// <summary>
    /// Applies damage to the enemy.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (_isDead)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Starts the enemy's death behavior.
    /// </summary>
    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        _currentHealth = 0;

        StopEnemy();

        if (_animator != null)
        {
            _animator.Play("Enemy_Death", 0, 0f);
        }
    }

    /// <summary>
    /// Stops the enemy's movement, combat, physics and collisions.
    /// </summary>
    private void StopEnemy()
    {
        if (_enemyMovement != null)
        {
            _enemyMovement.enabled = false;
        }

        if (_enemyCombat != null)
        {
            _enemyCombat.enabled = false;
        }

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.gravityScale = 0f;
        }

        foreach (Collider2D collider in _colliders)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Called by the Animation Event on the final frame of Enemy_Death.
    /// </summary>
    public void FinishDeath()
    {
        if (!_isDead)
        {
            return;
        }

        if (dropItemPrefab != null)
        {
            Instantiate(
                dropItemPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}