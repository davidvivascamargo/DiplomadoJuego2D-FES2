
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField, Min(0f)] private float horizontalForce = 7f;
    [SerializeField, Min(0f)] private float verticalForce = 3f;
    [SerializeField, Min(0f)] private float knockbackDuration = 0.2f;

    [Header("Knockback Behavior")]
    [SerializeField] private bool enableKnockback = true;
    [SerializeField] private bool resetVerticalVelocity = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string knockbackParameter = "IsKnockedBack";

    private Rigidbody2D rb;
    private float knockbackTimer;
    private int knockbackParameterHash;
    private bool hasKnockbackParameter;

    public bool IsKnockedBack => knockbackTimer > 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (!string.IsNullOrEmpty(knockbackParameter))
        {
            knockbackParameterHash =
                Animator.StringToHash(knockbackParameter);
        }

        ValidateAnimatorParameter();
    }

    private void FixedUpdate()
    {
        if (knockbackTimer <= 0f)
        {
            return;
        }

        knockbackTimer = Mathf.Max(
            0f,
            knockbackTimer - Time.fixedDeltaTime
        );
    }

    private void LateUpdate()
    {
        UpdateKnockbackAnimation();
    }

    public void ApplyKnockback(Vector2 attackerPosition)
    {
        if (!enableKnockback || rb == null)
        {
            return;
        }

        float horizontalDirection =
            transform.position.x >= attackerPosition.x
                ? 1f
                : -1f;

        float velocityY = resetVerticalVelocity
            ? verticalForce
            : Mathf.Max(
                rb.linearVelocity.y,
                verticalForce
            );

        rb.linearVelocity = new Vector2(
            horizontalDirection * horizontalForce,
            velocityY
        );

        knockbackTimer = knockbackDuration;

        UpdateKnockbackAnimation();
    }

    public void StopKnockback()
    {
        knockbackTimer = 0f;

        UpdateKnockbackAnimation();
    }

    private void UpdateKnockbackAnimation()
    {
        if (animator == null || !hasKnockbackParameter)
        {
            return;
        }

        animator.SetBool(
            knockbackParameterHash,
            IsKnockedBack
        );
    }

    private void ValidateAnimatorParameter()
    {
        hasKnockbackParameter = false;

        if (animator == null ||
            animator.runtimeAnimatorController == null ||
            string.IsNullOrEmpty(knockbackParameter))
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            if (parameter.name == knockbackParameter &&
                parameter.type == AnimatorControllerParameterType.Bool)
            {
                hasKnockbackParameter = true;
                break;
            }
        }
    }

    private void OnDisable()
    {
        knockbackTimer = 0f;

        if (animator != null && hasKnockbackParameter)
        {
            animator.SetBool(knockbackParameterHash, false);
        }
    }
}
