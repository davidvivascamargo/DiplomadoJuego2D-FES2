
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MysteriousGirlAppearance : MonoBehaviour
{
    [Header("Appearance Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Optional Effects")]
    [SerializeField] private ParticleSystem magicParticles;

    private SpriteRenderer spriteRenderer;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlpha(0f);
    }

    public void Appear()
    {
        StartFade(1f);
    }

    public void Disappear()
    {
        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (targetAlpha > 0f && magicParticles != null)
            magicParticles.Play();

        fadeCoroutine = StartCoroutine(Fade(targetAlpha));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float initialAlpha = spriteRenderer.color.a;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(initialAlpha, targetAlpha, progress);

            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);

        if (targetAlpha <= 0f && magicParticles != null)
            magicParticles.Stop();

        fadeCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
