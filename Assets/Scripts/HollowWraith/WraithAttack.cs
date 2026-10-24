
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WraithMovement))]
public class WraithAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField, Min(0.1f)] private float attackRange = 3f;
    [SerializeField, Min(0f)] private float preparationTime = 0.7f;
    [SerializeField, Min(0.1f)] private float dashSpeed = 8f;
    [SerializeField, Min(0.05f)] private float dashDuration = 0.35f;
    [SerializeField, Min(0f)] private float recoveryTime = 2f;

    [Header("Damage")]
    [SerializeField, Min(0.1f)] private float damageRadius = 0.8f;
    [SerializeField] private Vector2 damageOffset;
    [SerializeField] private LayerMask playerLayer;

    [Header("Knockback")]
    [SerializeField] private bool enableKnockback = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    private WraithMovement movement;
    private bool isAttacking;
    private bool isDashing;

    private readonly HashSet<GameObject> damagedPlayers = new();

    private void Awake()
    {
        movement = GetComponent<WraithMovement>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void TryAttack(Transform player)
    {
        if (isAttacking || player == null)
        {
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
        {
            return;
        }

        StartCoroutine(AttackRoutine(player));
    }

    private IEnumerator AttackRoutine(Transform player)
    {
        isAttacking = true;

        movement.BeginAttack();

        PlayAttackAnimation();

        yield return new WaitForSeconds(preparationTime);

        Vector2 direction = player != null
            ? (Vector2)(player.position - transform.position)
            : Vector2.right;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Vector2.right;
        }

        direction.Normalize();

        damagedPlayers.Clear();

        isDashing = true;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            movement.MoveDuringAttack(
                direction,
                dashSpeed
            );

            CheckDamage();

            elapsed += Time.deltaTime;

            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(recoveryTime);

        movement.EndAttack();

        isAttacking = false;
    }

    private void PlayAttackAnimation()
    {
        if (animator == null ||
            string.IsNullOrEmpty(attackTrigger))
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            if (parameter.name == attackTrigger &&
                parameter.type ==
                AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(attackTrigger);
                return;
            }
        }
    }

    private void CheckDamage()
    {
        if (!isDashing)
        {
            return;
        }

        Vector2 center =
            (Vector2)transform.position + damageOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            damageRadius,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            GameObject player =
                hit.attachedRigidbody != null
                    ? hit.attachedRigidbody.gameObject
                    : hit.gameObject;

            if (!player.CompareTag("Player"))
            {
                continue;
            }

            if (!damagedPlayers.Add(player))
            {
                continue;
            }

            ApplyDamage(player);
            ApplyKnockback(player);
        }
    }

    private void ApplyDamage(GameObject player)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.ReduceHealth();
    }

    private void ApplyKnockback(GameObject player)
    {
        if (!enableKnockback)
        {
            return;
        }

        PlayerKnockback knockback =
            player.GetComponent<PlayerKnockback>();

        if (knockback == null)
        {
            return;
        }

        knockback.ApplyKnockback(
            transform.position
        );
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        isDashing = false;
        isAttacking = false;

        damagedPlayers.Clear();

        if (movement != null)
        {
            movement.EndAttack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 damageCenter =
            transform.position + (Vector3)damageOffset;

        Gizmos.DrawWireSphere(
            damageCenter,
            damageRadius
        );

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
