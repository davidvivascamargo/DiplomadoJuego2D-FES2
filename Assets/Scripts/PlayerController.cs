using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PLAYER COMPONENTS
    private Rigidbody2D m_rigidbody2D;
    private GatherInput m_gatherInput;
    private Transform m_transform;
    private Animator m_animator; 

    [Header("Move and Jump Settings")]
    [SerializeField] private float speed;
    private int direction = 1;

    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private int counterextraJumps;
    private int idSpeed;

    [Header("Ground settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    [SerializeField] private bool isGrounded; 
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask groundLayer;
    private int idIsGrouded;

    void Start()
    {
        m_gatherInput = GetComponent<GatherInput>();
        m_transform = GetComponent<Transform>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        idSpeed = Animator.StringToHash("Speed");
        idIsGrouded = Animator.StringToHash("IsGrounded");
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
        Move();
        CheckGround(); 
        Jump();       
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
            }
            if (counterextraJumps > 0)
            {
                m_rigidbody2D.linearVelocity = new Vector2(m_rigidbody2D.linearVelocity.x, jumpForce);
                counterextraJumps--;
            }
            
            // Apagamos el salto ÚNICAMENTE cuando el jugador ya intentó saltar
            m_gatherInput.IsJumping = false;
        }
    }

    private void CheckGround()
    {
        RaycastHit2D lFootRay = Physics2D.Raycast(lFoot.position, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rFootRay = Physics2D.Raycast(rFoot.position, Vector2.down, rayLength, groundLayer);
        
        // Dibujamos los rayos en la escena para que puedas verlos mientras juegas
        Debug.DrawRay(lFoot.position, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rFoot.position, Vector2.down * rayLength, Color.red);

        if (lFootRay.collider != null || rFootRay.collider != null)
        {
            isGrounded = true;
            counterextraJumps = extraJumps;
        }
        else
        {
            isGrounded = false;
        }
    }
}
