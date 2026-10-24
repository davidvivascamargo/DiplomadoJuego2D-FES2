using System.Collections;
using UnityEngine;

public class WraithHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField, Min(1)] private int maxHealth = 3;

    [Header("Death")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathAnimation = "Wraith_Death";
    [SerializeField, Min(0f)] private float deathDuration = 1f;

    [Header("Drop")]
    [SerializeField] private GameObject dropItemPrefab;

    private int currentHealth;
    private bool isDead;

    private WraithMovement movement;
    private WraithAttack attack;
    private Collider2D[] colliders;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);

        movement = GetComponent<WraithMovement>();
        attack = GetComponent<WraithAttack>();

        colliders = GetComponentsInChildren<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        StopWraith();
        PlayDeathAnimation();

        StartCoroutine(DeathRoutine());
    }

    private void StopWraith()
    {
        if (movement != null)
        {
            movement.enabled = false;
        }

        if (attack != null)
        {
            attack.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }

    private void PlayDeathAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");

        animator.SetBool("IsFlying", false);

        animator.SetTrigger("Death");
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDuration);

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