
using UnityEngine;

public class WraithMovement : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float patrolSpeed = 1.5f;
    [SerializeField, Min(0.1f)] private float chaseSpeed = 2.5f;
    [SerializeField, Min(0.01f)] private float arrivalDistance = 0.15f;

    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float detectionRange = 7f;
    [SerializeField, Min(0.1f)] private float loseTargetRange = 12f;
    [SerializeField, Min(0f)] private float loseTargetDelay = 3f;

    [Header("Flying")]
    [SerializeField, Min(0f)] private float floatingAmplitude = 0.3f;
    [SerializeField, Min(0f)] private float floatingFrequency = 2f;
    [SerializeField] private float chaseHeight = 1f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private bool facesRightByDefault = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string flyingParameter = "IsFlying";
    [SerializeField, Min(0f)] private float flyingThreshold = 0.01f;

    private enum MovementState
    {
        Patrol,
        Chase,
        Return
    }

    private MovementState state = MovementState.Patrol;

    private Transform targetPoint;
    private WraithAttack wraithAttack;

    private Vector3 basePosition;
    private Vector3 returnPosition;

    private float loseTargetTimer;
    private bool attackControlsMovement;
    private bool isMoving;
    private bool hasFlyingParameter;

    public Transform Player => player;

    public bool IsChasing =>
        state == MovementState.Chase && !attackControlsMovement;

    private void Awake()
    {
        wraithAttack = GetComponent<WraithAttack>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        ValidateAnimatorParameters();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (startPoint == null || endPoint == null)
        {
            enabled = false;
            return;
        }

        basePosition = startPoint.position;
        transform.position = basePosition;

        targetPoint = endPoint;

        UpdateDirection(
            targetPoint.position.x - basePosition.x
        );

        UpdateFlyingAnimation(false);
    }

    private void Update()
    {
        if (attackControlsMovement)
        {
            return;
        }

        UpdateDetection();

        isMoving = false;

        switch (state)
        {
            case MovementState.Patrol:
                Patrol();
                break;

            case MovementState.Chase:
                Chase();
                break;

            case MovementState.Return:
                ReturnToPatrol();
                break;
        }

        UpdateFloating();
        UpdateFlyingAnimation(isMoving);
    }

    private void UpdateDetection()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(
            basePosition,
            player.position
        );

        if (state != MovementState.Chase)
        {
            if (distance <= detectionRange)
            {
                state = MovementState.Chase;
                loseTargetTimer = 0f;
            }

            return;
        }

        if (distance > loseTargetRange)
        {
            loseTargetTimer += Time.deltaTime;

            if (loseTargetTimer >= loseTargetDelay)
            {
                returnPosition = FindNearestPatrolPoint();
                state = MovementState.Return;
                loseTargetTimer = 0f;
            }
        }
        else
        {
            loseTargetTimer = 0f;
        }
    }

    private void Patrol()
    {
        MoveTo(
            targetPoint.position,
            patrolSpeed
        );

        if (Vector2.Distance(
            basePosition,
            targetPoint.position
        ) <= arrivalDistance)
        {
            targetPoint = targetPoint == startPoint
                ? endPoint
                : startPoint;
        }
    }

    private void Chase()
    {
        if (player == null)
        {
            return;
        }

        Vector3 destination = player.position;
        destination.y += chaseHeight;
        destination.z = basePosition.z;

        MoveTo(
            destination,
            chaseSpeed
        );

        if (wraithAttack != null &&
            wraithAttack.isActiveAndEnabled)
        {
            wraithAttack.TryAttack(player);
        }
    }

    private void ReturnToPatrol()
    {
        MoveTo(
            returnPosition,
            patrolSpeed
        );

        if (Vector2.Distance(
            basePosition,
            returnPosition
        ) <= arrivalDistance)
        {
            targetPoint = Vector2.Distance(
                returnPosition,
                startPoint.position
            ) <= arrivalDistance
                ? endPoint
                : startPoint;

            state = MovementState.Patrol;
        }
    }

    private void MoveTo(Vector3 destination, float speed)
    {
        Vector3 previousPosition = basePosition;

        basePosition = Vector3.MoveTowards(
            basePosition,
            destination,
            speed * Time.deltaTime
        );

        Vector3 movement = basePosition - previousPosition;

        isMoving =
            movement.sqrMagnitude >
            flyingThreshold * flyingThreshold *
            Time.deltaTime * Time.deltaTime;

        UpdateDirection(movement.x);
    }

    private Vector3 FindNearestPatrolPoint()
    {
        float distanceA = Vector2.Distance(
            basePosition,
            startPoint.position
        );

        float distanceB = Vector2.Distance(
            basePosition,
            endPoint.position
        );

        return distanceA <= distanceB
            ? startPoint.position
            : endPoint.position;
    }

    private void UpdateFloating()
    {
        float offset = Mathf.Sin(
            Time.time * floatingFrequency
        ) * floatingAmplitude;

        transform.position =
            basePosition + Vector3.up * offset;
    }

    private void UpdateDirection(float horizontalDirection)
    {
        if (!flipSprite || spriteRenderer == null)
        {
            return;
        }

        if (Mathf.Abs(horizontalDirection) < 0.0001f)
        {
            return;
        }

        bool movingRight = horizontalDirection > 0f;

        spriteRenderer.flipX = facesRightByDefault
            ? !movingRight
            : movingRight;
    }

    private void ValidateAnimatorParameters()
    {
        hasFlyingParameter = false;

        if (animator == null ||
            animator.runtimeAnimatorController == null)
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            if (parameter.name == flyingParameter &&
                parameter.type ==
                AnimatorControllerParameterType.Bool)
            {
                hasFlyingParameter = true;
                break;
            }
        }
    }

    private void UpdateFlyingAnimation(bool flying)
    {
        if (animator == null || !hasFlyingParameter)
        {
            return;
        }

        animator.SetBool(flyingParameter, flying);
    }

    public void BeginAttack()
    {
        attackControlsMovement = true;
        isMoving = false;

        UpdateFlyingAnimation(false);
    }

    public void MoveDuringAttack(
        Vector2 direction,
        float speed
    )
    {
        if (!attackControlsMovement)
        {
            return;
        }

        Vector3 movement =
            (Vector3)(direction.normalized *
            speed * Time.deltaTime);

        basePosition += movement;
        transform.position = basePosition;

        UpdateDirection(movement.x);
    }

    public void EndAttack()
    {
        if (!attackControlsMovement)
        {
            return;
        }

        basePosition = transform.position;
        attackControlsMovement = false;
        isMoving = false;

        UpdateFlyingAnimation(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(
            transform.position,
            loseTargetRange
        );
    }
}
