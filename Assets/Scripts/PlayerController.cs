using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Transform m_transform;
    private Rigidbody2D m_rigidbody2D;
    private GatherInput m_gatherInput;
    private Animator m_animator; 

    // ANIMATOR IDS
    private int idIsGrouded;
    private int idSpeed;

    [Header("Move Settings")]
    [SerializeField] private float speed;
    private int direction = 1;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private int counterextraJumps;
    [SerializeField] private bool canDoubleJump;

    [Header("Ground settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    RaycastHit2D lFootRay;
    RaycastHit2D rFootRay;
    [SerializeField] private bool isGrounded; 
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall settings")]
    [SerializeField] private float checkWallDistance;
    [SerializeField] private bool isWallDetected;

    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_gatherInput = GetComponent<GatherInput>();
        m_transform = GetComponent<Transform>();
        m_animator = GetComponent<Animator>();
    }

    void Start()
    {
        idSpeed = Animator.StringToHash("speed");
        idIsGrouded = Animator.StringToHash("isGrounded");
        lFoot = GameObject.Find("LFoot").GetComponent<Transform>();
        rFoot = GameObject.Find("RFoot").GetComponent<Transform>();
        counterextraJumps = extraJumps;
    }

    void Update()
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        m_animator.SetFloat(idSpeed, Mathf.Abs(m_rigidbody2D.linearVelocity.x));
        m_animator.SetBool(idIsGrouded, isGrounded);
    }

    void FixedUpdate()
    {
        CheckCollision(); 
        Move();
        Jump();       
    }

    private void CheckCollision()
    {
        HandleGround();
        HandleWall();
    }


    private void HandleGround()
    {
        lFootRay = Physics2D.Raycast(lFoot.position, Vector2.down, rayLength, groundLayer);
        rFootRay = Physics2D.Raycast(rFoot.position, Vector2.down, rayLength, groundLayer);

        if (lFootRay.collider != null || rFootRay.collider != null)
        {
            isGrounded = true;
            counterextraJumps = extraJumps;
            canDoubleJump = false;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void HandleWall()
    {
        isWallDetected = Physics2D.Raycast(m_transform.position, Vector2.right * direction, checkWallDistance, groundLayer);
    }

    private void Move()
    {
        Flip();
        m_rigidbody2D.linearVelocity = new Vector2(speed *  m_gatherInput.ValueX, m_rigidbody2D.linearVelocity.y);
    }

    private void Flip()
    {
        if (m_gatherInput.ValueX * direction < 0)
        {
            m_transform.localScale = new Vector3(-m_transform.localScale.x, 1, 1);
            direction *= -1;
        }
    }

    private void Jump()
    {

        if (m_gatherInput.IsJumping)
        {
            // Solo si está tocando el suelo, aplicamos la fuerza hacia arriba
            if (isGrounded)
            {
                m_rigidbody2D.linearVelocity = new Vector2(m_rigidbody2D.linearVelocity.x, jumpForce);
                canDoubleJump = true;
            }
            else if (counterextraJumps > 0 && canDoubleJump)
            {
                m_rigidbody2D.linearVelocity = new Vector2(m_rigidbody2D.linearVelocity.x, jumpForce);
                counterextraJumps--;
            }
            
            // Apagamos el salto ÚNICAMENTE cuando el jugador ya intentó saltar
            m_gatherInput.IsJumping = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_transform.position, new Vector2(m_transform.position.x + (checkWallDistance * direction), m_transform.position.y));
    }

}
