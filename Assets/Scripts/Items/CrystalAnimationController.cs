
using System.Collections;
using UnityEngine;

public class CrystalAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float riseHeight = 1.5f;
    [SerializeField] private float riseDuration = 2f;
    [SerializeField] private float floatAmplitude = 0.1f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Transformation")]
    [SerializeField] private Animator crystalAnimator;

    [Header("Sword Flight")]
    [SerializeField] private Transform playerTarget;
    [SerializeField, Min(0.1f)] private float flightSpeed = 4f;
    [SerializeField, Min(0f)] private float flightDelay = 0.5f;
    [SerializeField, Min(0.01f)] private float arrivalDistance = 0.2f;

    [Header("Sword Unlock")]
    [SerializeField] private PlayerSword playerSword;

    private Coroutine animationCoroutine;
    private Coroutine flightCoroutine;

    private Vector3 floatingPosition;

    private bool isTransforming;
    private bool isFlying;

    private void Awake()
    {
        if (crystalAnimator == null)
            crystalAnimator = GetComponent<Animator>();

        if (playerSword == null && playerTarget != null)
            playerSword = playerTarget.GetComponent<PlayerSword>();
    }

    public void Appear()
    {
        gameObject.SetActive(true);

        isTransforming = false;
        isFlying = false;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (flightCoroutine != null)
        {
            StopCoroutine(flightCoroutine);
            flightCoroutine = null;
        }

        animationCoroutine = StartCoroutine(RiseAndFloat());
    }

    private IEnumerator RiseAndFloat()
    {
        Vector3 startPosition = transform.position;

        Vector3 targetPosition =
            startPosition + Vector3.up * riseHeight;

        float elapsed = 0f;

        // Crystal rising animation.
        while (elapsed < riseDuration)
        {
            if (isTransforming)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, riseDuration)
            );

            float smoothProgress =
                Mathf.SmoothStep(0f, 1f, progress);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );

            yield return null;
        }

        floatingPosition = targetPosition;

        // Crystal floating animation.
        while (!isTransforming)
        {
            float offset =
                Mathf.Sin(Time.unscaledTime * floatSpeed)
                * floatAmplitude;

            transform.position =
                floatingPosition + Vector3.up * offset;

            yield return null;
        }

        animationCoroutine = null;
    }

    public void TransformToSword()
    {
        if (isTransforming)
            return;

        if (crystalAnimator == null)
            crystalAnimator = GetComponent<Animator>();

        if (crystalAnimator == null)
        {
            Debug.LogWarning(
                "[Crystal] Animator not found."
            );
            return;
        }

        isTransforming = true;

        // Stop crystal floating movement.
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        // Start crystal-to-sword transformation.
        crystalAnimator.SetTrigger("Transform");

        Debug.Log(
            "[Crystal] Sword transformation started."
        );
    }

    // Called by the Animation Event at the last frame.
    public void OnSwordTransformationCompleted()
    {
        if (!isTransforming || isFlying)
            return;

        Debug.Log(
            "[Crystal] Sword transformation completed."
        );

        if (playerTarget == null)
        {
            Debug.LogWarning(
                "[Crystal] Player Target is not assigned."
            );
            return;
        }

        flightCoroutine = StartCoroutine(FlyToPlayer());
    }

    private IEnumerator FlyToPlayer()
    {
        isFlying = true;

        // Wait before starting the sword flight.
        yield return new WaitForSecondsRealtime(flightDelay);

        Debug.Log("[Crystal] Sword flight started.");

        while (playerTarget != null)
        {
            Vector3 destination = playerTarget.position;

            // Keep the sword on its current Z plane.
            destination.z = transform.position.z;

            // Move the sword toward the player.
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                flightSpeed * Time.unscaledDeltaTime
            );

            // Check if the sword reached the player.
            if (Vector3.Distance(
                transform.position,
                destination
            ) <= arrivalDistance)
            {
                Debug.Log(
                    "[Crystal] Sword reached the player."
                );

                // Find PlayerSword if it was not assigned.
                if (playerSword == null && playerTarget != null)
                {
                    playerSword =
                        playerTarget.GetComponent<PlayerSword>();
                }

                if (playerSword != null)
                {
                    // Unlock the sword for the player.
                    playerSword.UnlockSword();

                    Debug.Log(
                        "[Crystal] Sword delivered to player."
                    );

                    isFlying = false;
                    flightCoroutine = null;

                    // Hide the crystal after delivering the sword.
                    gameObject.SetActive(false);

                    yield break;
                }
                else
                {
                    Debug.LogWarning(
                        "[Crystal] PlayerSword is not assigned."
                    );

                    break;
                }
            }

            yield return null;
        }

        isFlying = false;
        flightCoroutine = null;
    }
}
