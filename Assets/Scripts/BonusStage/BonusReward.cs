
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls bonus reward launching, blinking,
/// and automatic destruction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BonusReward : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float horizontalForce = 3f;
    [SerializeField] private float verticalForce = 8f;

    [Header("Lifetime Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float blinkDuration = 2f;
    [SerializeField] private float blinkInterval = 0.12f;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(LifetimeRoutine());
    }

    public void Launch()
    {
        float horizontal = Random.Range(
            -horizontalForce,
            horizontalForce
        );

        float vertical = Random.Range(
            verticalForce * 0.8f,
            verticalForce * 1.2f
        );

        _rb.AddForce(
            new Vector2(horizontal, vertical),
            ForceMode2D.Impulse
        );
    }

    private IEnumerator LifetimeRoutine()
    {
        float waitTime = Mathf.Max(
            0f,
            lifetime - blinkDuration
        );

        yield return new WaitForSeconds(waitTime);

        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled =
                    !_spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval;
        }

        Destroy(gameObject);
    }
}
