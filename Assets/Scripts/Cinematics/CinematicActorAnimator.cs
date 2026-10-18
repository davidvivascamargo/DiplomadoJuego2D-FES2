using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CinematicActorAnimator : MonoBehaviour
{
    private Animator animator;
    private AnimatorUpdateMode originalUpdateMode;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        originalUpdateMode = animator.updateMode;
    }

    private void Update()
    {
        if (CinematicManager.Instance == null)
            return;

        bool isCinematic = CinematicManager.Instance.IsCinematic;

        AnimatorUpdateMode expectedMode = isCinematic
            ? AnimatorUpdateMode.UnscaledTime
            : originalUpdateMode;

        if (animator.updateMode != expectedMode)
        {
            animator.updateMode = expectedMode;

            Debug.Log(
                $"[CinematicActorAnimator] Update Mode: {expectedMode}",
                this
            );
        }
    }

    private void OnDisable()
    {
        if (animator != null)
        {
            animator.updateMode = originalUpdateMode;
        }
    }
}