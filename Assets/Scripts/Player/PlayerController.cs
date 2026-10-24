
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GatherInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Transform m_transform;

    private Rigidbody2D m_rigidbody2D;
    private GatherInput m_gatherInput;
    private Animator m_animator;
    private CapsuleCollider2D capsuleCollider;

    private PlayerKnockback playerKnockback;

    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    // Animator IDs
    private int idIsGrounded;
    private int idSpeed;
    private int idVerticalSpeed;
    private int idIsSkidding;

    [Header("Move Settings")]
    [SerializeField] private float speed;
    private int direction = 1;

    [Header("Skid Settings")]
    [SerializeField] private float skidDuration = 0.25f;
    [SerializeField] private float skidDeceleration = 15f;
    [SerializeField] private float minSkidSpeed = 0.5f;

    private bool isSkidding;
    private bool skidRequested;
    private float skidTimer;
    private float skidVelocity;

    [Header("Skid Collider Settings")]

    [SerializeField, Range(0.1f, 1f)]
    private float skidWidthMultiplier = 0.65f;

    [SerializeField, Range(0.1f, 1f)]
    private float skidHeightMultiplier = 0.40f;

    [SerializeField] private Vector2 standingColliderSize;
    [SerializeField] private Vector2 standingColliderOffset;

    [SerializeField] private Vector2 skidColliderSize;
    [SerializeField] private Vector2 skidColliderOffset;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private int counterextraJumps;
    [SerializeField] private bool canDoubleJump;

    [Header("Ground Settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;

    private RaycastHit2D lFootRay;
    private RaycastHit2D rFootRay;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Settings")]
    [SerializeField] private float checkWallDistance;
    [SerializeField] private bool isWallDetected;

    [Header("Collider Debug")]
    [SerializeField] private bool enableColliderDebug = true;

    private Vector3 previousScale;
    private Vector3 previousRotation;
    private Vector2 previousColliderSize;
    private Vector2 previousColliderOffset;
    private CapsuleDirection2D previousColliderDirection;

    private bool IsCinematicActive =>
    CinematicManager.Instance != null &&
    CinematicManager.Instance.IsCinematic;

    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_gatherInput = GetComponent<GatherInput>();
        m_transform = transform;

        m_animator = GetComponent<Animator>();

        if (m_animator == null)
        {
            m_animator = GetComponentInChildren<Animator>();
        }

        capsuleCollider = GetComponent<CapsuleCollider2D>();

        playerKnockback = GetComponent<PlayerKnockback>();

        // Obtener el SpriteRenderer del personaje.
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        idSpeed = Animator.StringToHash("speed");
        idIsGrounded = Animator.StringToHash("isGrounded");
        idVerticalSpeed = Animator.StringToHash("verticalSpeed");
        idIsSkidding = Animator.StringToHash("isSkidding");

        if (lFoot == null)
        {
            lFoot = transform.Find("LFoot");
        }

        if (rFoot == null)
        {
            rFoot = transform.Find("RFoot");
        }

        counterextraJumps = extraJumps;

        direction = m_transform.localScale.x >= 0 ? 1 : -1;

        if (capsuleCollider != null)
        {
            standingColliderSize = capsuleCollider.size;
            standingColliderOffset = capsuleCollider.offset;
        }

        InitializeColliderDebug();
    }

    private void Update()
    {
        SetAnimatorValues();
    }

    private void FixedUpdate()
    {
        CheckCollision();

        if (IsCinematicActive)
        {
            m_rigidbody2D.linearVelocity = new Vector2(
                0f,
                m_rigidbody2D.linearVelocity.y
            );

            m_gatherInput.IsJumping = false;
            skidRequested = false;

            if (isSkidding)
            {
                StopSkid();
            }

            return;
        }

        if (playerKnockback != null &&
            playerKnockback.IsKnockedBack)
        {
            m_gatherInput.IsJumping = false;
            skidRequested = false;

            if (isSkidding)
            {
                StopSkid();
            }

            return;
        }

        Move();
        Jump();
    }

    private void LateUpdate()
    {
        if (isSkidding)
        {
            UpdateSkidColliderFromSprite();
        }

        CheckColliderChanges();
    }

    private void SetAnimatorValues()
    {
        if (m_animator == null)
        {
            return;
        }

        float horizontalSpeed =
            Mathf.Abs(m_rigidbody2D.linearVelocity.x);

        float verticalSpeed =
            m_rigidbody2D.linearVelocity.y;

        m_animator.SetFloat(idSpeed, horizontalSpeed);
        m_animator.SetBool(idIsGrounded, isGrounded);
        m_animator.SetFloat(idVerticalSpeed, verticalSpeed);
        m_animator.SetBool(idIsSkidding, isSkidding);
    }

    private void CheckCollision()
    {
        HandleGround();
        HandleWall();
    }

    private void HandleGround()
    {
        if (lFoot == null || rFoot == null)
        {
            return;
        }

        lFootRay = Physics2D.Raycast(
            lFoot.position,
            Vector2.down,
            rayLength,
            groundLayer
        );

        rFootRay = Physics2D.Raycast(
            rFoot.position,
            Vector2.down,
            rayLength,
            groundLayer
        );

        bool detectedGround =
            lFootRay.collider != null ||
            rFootRay.collider != null;

        isGrounded =
            detectedGround &&
            m_rigidbody2D.linearVelocity.y <= 0.01f;

        if (isGrounded)
        {
            counterextraJumps = extraJumps;
            canDoubleJump = false;
        }
    }

    private void HandleWall()
    {
        isWallDetected = Physics2D.Raycast(
            m_transform.position,
            Vector2.right * direction,
            checkWallDistance,
            groundLayer
        );
    }

    // ----------------------------------------
    // Movement and Skid
    // ----------------------------------------

    // Called by PlayerInput (Behavior: Send Messages) for the Skid action.
    public void OnSkid(InputValue value)
    {
        if (IsCinematicActive)
        {
            skidRequested = false;
            return;
        }

        if (value.isPressed)
        {
            skidRequested = true;
        }
    }

    private void Move()
    {
        float inputX = m_gatherInput.ValueX;
        float currentVelocityX = m_rigidbody2D.linearVelocity.x;
        bool startSkidRequested = skidRequested;
        skidRequested = false;

        // Do not start a skid while airborne.
        if (!isGrounded)
        {
            if (isSkidding)
            {
                StopSkid();
            }

            Flip();

            SetHorizontalVelocity(speed * inputX);
            return;
        }

        // Continue an active skid.
        if (isSkidding)
        {
            UpdateSkid();
            return;
        }

        // Skid only when the configured action is pressed while running.
        bool isMovingFastEnough =
            Mathf.Abs(currentVelocityX) >= minSkidSpeed;

        if (startSkidRequested && isMovingFastEnough)
        {
            StartSkid();
            UpdateSkid();
            return;
        }

        Flip();

        SetHorizontalVelocity(speed * inputX);
    }

    private void StartSkid()
    {
        isSkidding = true;
        skidTimer = skidDuration;

        // Preserve the direction and speed before braking.
        skidVelocity = m_rigidbody2D.linearVelocity.x;

        UpdateSkidCollider(true);

        Debug.Log(
            "[Player Skid] Skid started.",
            this
        );
    }

    private void UpdateSkid()
    {
        skidTimer -= Time.fixedDeltaTime;

        // Gradually reduce horizontal speed.
        skidVelocity = Mathf.MoveTowards(
            skidVelocity,
            0f,
            skidDeceleration * Time.fixedDeltaTime
        );

        SetHorizontalVelocity(skidVelocity);

        // Finish when the timer expires or the player stops.
        if (
            skidTimer <= 0f ||
            Mathf.Abs(skidVelocity) <= 0.01f
        )
        {
            StopSkid();
        }
    }

    private void StopSkid()
    {
        if (!isSkidding)
        {
            return;
        }

        isSkidding = false;
        skidTimer = 0f;
        skidVelocity = 0f;

        UpdateSkidCollider(false);

        Debug.Log(
            "[Player Skid] Skid finished.",
            this
        );
    }

    private void UpdateSkidCollider(bool sliding)
    {
        if (capsuleCollider == null)
            return;

        if (!sliding)
        {
            capsuleCollider.direction = CapsuleDirection2D.Vertical;
            capsuleCollider.size = standingColliderSize;
            capsuleCollider.offset = standingColliderOffset;
            return;
        }

        UpdateSkidColliderFromSprite();
    }

    private void UpdateSkidColliderFromSprite()
    {
        if (playerSpriteRenderer == null ||
            playerSpriteRenderer.sprite == null)
            return;

        Sprite sprite = playerSpriteRenderer.sprite;
        Vector2[] vertices = sprite.vertices;

        if (vertices == null || vertices.Length == 0)
            return;

        Vector2 min = vertices[0];
        Vector2 max = vertices[0];

        foreach (Vector2 vertex in vertices)
        {
            min = Vector2.Min(min, vertex);
            max = Vector2.Max(max, vertex);
        }

        Vector2 spriteSize = max - min;
        Vector2 spriteCenter = (min + max) * 0.5f;

        float width = Mathf.Max(
            0.1f,
            spriteSize.x * skidWidthMultiplier
        );

        float height = Mathf.Max(
            0.1f,
            spriteSize.y * skidHeightMultiplier
        );

        capsuleCollider.direction = CapsuleDirection2D.Horizontal;

        capsuleCollider.size = new Vector2(
            Mathf.Max(width, height),
            Mathf.Min(width, height)
        );

        capsuleCollider.offset = spriteCenter;
    }

    private void SetHorizontalVelocity(float velocityX)
    {
        m_rigidbody2D.linearVelocity = new Vector2(
            velocityX,
            m_rigidbody2D.linearVelocity.y
        );
    }

    private void Flip()
    {
        if (m_gatherInput.ValueX * direction < 0)
        {
            Vector3 currentScale = m_transform.localScale;

            // Preserve Y and Z scale.
            currentScale.x *= -1f;

            m_transform.localScale = currentScale;

            direction *= -1;
        }
    }

    // ----------------------------------------
    // Jump
    // ----------------------------------------

    private void Jump()
    {
        if (!m_gatherInput.IsJumping)
        {
            return;
        }

        if (isGrounded)
        {
            if (isSkidding)
            {
                StopSkid();
            }

            ApplyJump();

            canDoubleJump = true;
            isGrounded = false;
        }
        else if (counterextraJumps > 0 && canDoubleJump)
        {
            ApplyJump();

            counterextraJumps--;
            isGrounded = false;
        }

        m_gatherInput.IsJumping = false;
    }

    private void ApplyJump()
    {
        m_rigidbody2D.linearVelocity = new Vector2(
            m_rigidbody2D.linearVelocity.x,
            jumpForce
        );
    }

    // ----------------------------------------
    // Capsule Collider Debug
    // ----------------------------------------

    private void InitializeColliderDebug()
    {
        if (capsuleCollider == null)
        {
            Debug.LogWarning(
                "[Player Debug] CapsuleCollider2D not found.",
                this
            );

            return;
        }

        previousScale = m_transform.localScale;
        previousRotation = m_transform.eulerAngles;
        previousColliderSize = capsuleCollider.size;
        previousColliderOffset = capsuleCollider.offset;
        previousColliderDirection = capsuleCollider.direction;

        if (enableColliderDebug)
        {
            LogColliderState("INITIAL STATE");
        }
    }

    private void CheckColliderChanges()
    {
        if (!enableColliderDebug || capsuleCollider == null)
        {
            return;
        }

        bool scaleChanged =
            m_transform.localScale != previousScale;

        bool rotationChanged =
            m_transform.eulerAngles != previousRotation;

        bool sizeChanged =
            capsuleCollider.size != previousColliderSize;

        bool offsetChanged =
            capsuleCollider.offset != previousColliderOffset;

        bool directionChanged =
            capsuleCollider.direction != previousColliderDirection;

        if (
            scaleChanged ||
            rotationChanged ||
            sizeChanged ||
            offsetChanged ||
            directionChanged
        )
        {
            LogColliderState("CHANGE DETECTED");

            previousScale = m_transform.localScale;
            previousRotation = m_transform.eulerAngles;
            previousColliderSize = capsuleCollider.size;
            previousColliderOffset = capsuleCollider.offset;
            previousColliderDirection = capsuleCollider.direction;
        }
    }

    private void LogColliderState(string state)
    {
        Debug.Log(
            $"[Player Debug] {state}\n" +
            $"Scale: {m_transform.localScale}\n" +
            $"Rotation: {m_transform.eulerAngles}\n" +
            $"Collider Size: {capsuleCollider.size}\n" +
            $"Collider Offset: {capsuleCollider.offset}\n" +
            $"Collider Direction: {capsuleCollider.direction}\n" +
            $"World Bounds: {capsuleCollider.bounds.size}",
            this
        );
    }

    private void OnDrawGizmos()
    {
        Transform currentTransform =
            m_transform != null ? m_transform : transform;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            currentTransform.position,
            new Vector2(
                currentTransform.position.x +
                (checkWallDistance * direction),
                currentTransform.position.y
            )
        );

        if (lFoot != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(
                lFoot.position,
                lFoot.position + Vector3.down * rayLength
            );
        }

        if (rFoot != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(
                rFoot.position,
                rFoot.position + Vector3.down * rayLength
            );
        }
    }
}
