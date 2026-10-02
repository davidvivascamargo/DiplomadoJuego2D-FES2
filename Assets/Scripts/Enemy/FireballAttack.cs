using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the enemy's special attack by launching multiple temporary fireball effects.
/// </summary>
public class FireballAttack : MonoBehaviour
{
    [Header("Attack Point")]
    [SerializeField] private Transform fireballAttackPoint;

    [Header("Temporary Visual")]
    [SerializeField] private ParticleSystem fireballEffect;

    [Header("Attack Settings")]
    [SerializeField] private int fireballCount = 3;
    [SerializeField] private float delayBetweenFireballs = 0.4f;
    [SerializeField] private float delayBeforeAttack = 0.5f;

    [Header("Damage")]
    [SerializeField] private int damage = 3;

    [Header("Testing")]
    [SerializeField] private bool enableTestAttack = true;
    [SerializeField] private KeyCode testAttackKey = KeyCode.F;

    private bool _isAttacking;

    public bool IsAttacking => _isAttacking;

    private void Update()
    {
        if (!enableTestAttack)
        {
            return;
        }

        if (Input.GetKeyDown(testAttackKey))
        {
            StartSpecialAttack();
        }
    }

    /// <summary>
    /// Starts the special attack if it is not already running.
    /// </summary>
    public void StartSpecialAttack()
    {
        if (_isAttacking)
        {
            return;
        }

        if (fireballAttackPoint == null)
        {
            Debug.LogWarning(
                "[FireballAttack] FireballAttackPoint is not assigned."
            );

            return;
        }

        if (fireballEffect == null)
        {
            Debug.LogWarning(
                "[FireballAttack] Fireball Effect is not assigned."
            );

            return;
        }

        StartCoroutine(LaunchFireballs());
    }

    /// <summary>
    /// Launches the configured number of fireballs one after another.
    /// </summary>
    private IEnumerator LaunchFireballs()
    {
        _isAttacking = true;

        yield return new WaitForSeconds(delayBeforeAttack);

        for (int i = 0; i < fireballCount; i++)
        {
            LaunchFireballEffect();

            if (i < fireballCount - 1)
            {
                yield return new WaitForSeconds(delayBetweenFireballs);
            }
        }

        _isAttacking = false;
    }

    /// <summary>
    /// Creates and plays one fireball particle effect.
    /// </summary>
    private void LaunchFireballEffect()
    {
        ParticleSystem effect = Instantiate(
            fireballEffect,
            fireballAttackPoint.position,
            Quaternion.identity
        );

        effect.transform.localScale = Vector3.one;

        Vector3 direction = transform.localScale.x < 0f
            ? Vector3.right
            : Vector3.left;

        effect.transform.right = direction;

        effect.Play(true);

        Destroy(effect.gameObject, 3f);
    }
}