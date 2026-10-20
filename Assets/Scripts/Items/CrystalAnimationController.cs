
using System.Collections;
using UnityEngine;

public class CrystalAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float riseHeight = 1.5f;
    [SerializeField] private float riseDuration = 2f;
    [SerializeField] private float floatAmplitude = 0.1f;
    [SerializeField] private float floatSpeed = 2f;

    private Coroutine animationCoroutine;
    private Vector3 floatingPosition;

    public void Appear()
    {
        gameObject.SetActive(true);

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(RiseAndFloat());
    }

    private IEnumerator RiseAndFloat()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition =
            startPosition + Vector3.up * riseHeight;

        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
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

        while (true)
        {
            float offset =
                Mathf.Sin(Time.unscaledTime * floatSpeed)
                * floatAmplitude;

            transform.position =
                floatingPosition + Vector3.up * offset;

            yield return null;
        }
    }
}
