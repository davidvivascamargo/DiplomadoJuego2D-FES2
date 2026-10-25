
using UnityEngine;

/// <summary>
/// Fades out while the player stands on the platform.
/// Restores itself when the player leaves or falls.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class FadingPlatform : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField, Min(0.1f)]
    private float fadeDuration = 2f;

    [SerializeField, Min(0.1f)]
    private float restoreDuration = 1.5f;

    [Header("Collision Settings")]
    [SerializeField, Range(0f, 1f)]
    private float disableCollisionAlpha = 0.05f;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _platformCollider;

    private float _currentAlpha = 1f;
    private float _originalAlpha = 1f;

    private bool _playerOnPlatform;
    private bool _collisionDisabled;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _platformCollider = GetComponent<Collider2D>();

        _originalAlpha = _spriteRenderer.color.a;
        _currentAlpha = 1f;
    }

    private void Update()
    {
        if (_playerOnPlatform)
        {
            _currentAlpha -= Time.deltaTime / fadeDuration;
        }
        else
        {
            _currentAlpha += Time.deltaTime / restoreDuration;
        }

        _currentAlpha = Mathf.Clamp01(_currentAlpha);

        Color color = _spriteRenderer.color;
        color.a = _currentAlpha * _originalAlpha;
        _spriteRenderer.color = color;

        // Remove collision when almost invisible.
        if (_currentAlpha <= disableCollisionAlpha &&
            !_collisionDisabled)
        {
            _collisionDisabled = true;
            _platformCollider.enabled = false;

            // The player will no longer be standing on it.
            _playerOnPlatform = false;
        }

        // Restore collision only when fully visible.
        if (_collisionDisabled && _currentAlpha >= 1f)
        {
            _platformCollider.enabled = true;
            _collisionDisabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (IsPlayerAbove(collision))
        {
            _playerOnPlatform = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        _playerOnPlatform = IsPlayerAbove(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerOnPlatform = false;
        }
    }

    private bool IsPlayerAbove(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Contact normal points upward from the platform.
            if (contact.normal.y < -0.5f)
            {
                return true;
            }
        }

        return false;
    }
}
