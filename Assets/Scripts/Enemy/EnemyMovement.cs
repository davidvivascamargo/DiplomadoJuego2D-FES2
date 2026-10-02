using UnityEngine;

/// <summary>
/// Controls the enemy's horizontal movement toward the player.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float followRange = 12f;
    [SerializeField] private float stopDistance = 1.5f;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null)
        {
            StopMovement();
            return;
        }

        float distanceToPlayer = Mathf.Abs(
            playerTransform.position.x - transform.position.x
        );

        if (distanceToPlayer > followRange)
        {
            StopMovement();
            return;
        }

        if (distanceToPlayer <= stopDistance)
        {
            StopMovement();
            return;
        }

        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(
            playerTransform.position.x - transform.position.x
        );

        _rigidbody.linearVelocity = new Vector2(
            direction * moveSpeed,
            _rigidbody.linearVelocity.y
        );

        UpdateAnimation(true);
        FacePlayer(direction);
    }

    private void StopMovement()
    {
        _rigidbody.linearVelocity = new Vector2(
            0f,
            _rigidbody.linearVelocity.y
        );

        UpdateAnimation(false);
    }

    private void UpdateAnimation(bool isMoving)
    {
        if (_animator != null)
        {
            _animator.SetFloat(
                "Speed",
                isMoving ? moveSpeed : 0f
            );
        }
    }

    private void FacePlayer(float direction)
    {
        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * -direction;

        transform.localScale = scale;
    }
}