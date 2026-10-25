
using UnityEngine;

public class ScarabJumpPad : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float launchForce = 25f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D _playerRigidbody;
    private bool _isActivating;
    private bool _hasLaunched;
    private bool _playerInside;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = true;
        TryActivate(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!_playerInside)
        {
            _playerInside = true;
            TryActivate(other);
        }
    }

    private void TryActivate(Collider2D other)
    {
        if (_isActivating || _hasLaunched)
            return;

        Rigidbody2D rb = other.attachedRigidbody;

        if (rb == null || rb.linearVelocity.y > 0.1f)
            return;

        _playerRigidbody = rb;
        _isActivating = true;

        animator.SetTrigger("Launch");
    }

    // Animation Event: when the light appears.
    public void LaunchPlayer()
    {
        if (!_isActivating || _hasLaunched ||
            _playerRigidbody == null)
            return;

        _hasLaunched = true;

        Vector2 velocity = _playerRigidbody.linearVelocity;
        velocity.y = launchForce;

        _playerRigidbody.linearVelocity = velocity;

        Debug.Log("ScarabJumpPad: Player launched!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = false;
        _hasLaunched = false;
        _playerRigidbody = null;
    }

    // Animation Event: at the end of the animation.
    public void FinishLaunch()
    {
        _isActivating = false;
    }
}
