
using UnityEngine;

[RequireComponent(typeof(ChestInteraction))]
public class ChestEventController : MonoBehaviour
{
    [Header("Cinematic Characters")]
    [SerializeField] private MysteriousGirlAppearance mysteriousGirl;

    [Header("Chest Animation")]
    [SerializeField] private Animator chestAnimator;

    private bool eventInProgress;
    private bool eventCompleted;

    public bool IsCompleted => eventCompleted;

    private void Awake()
    {
        if (chestAnimator == null)
            chestAnimator = GetComponent<Animator>();
    }

    public void BeginEvent()
    {
        if (eventInProgress || eventCompleted)
            return;

        if (CinematicManager.Instance == null)
        {
            Debug.LogWarning("[ChestEvent] CinematicManager not found.");
            return;
        }

        if (!CinematicManager.Instance.IsPlaying)
            return;

        CinematicManager.Instance.StartCinematic();

        if (!CinematicManager.Instance.IsCinematic)
            return;

        eventInProgress = true;

        mysteriousGirl?.Appear();

        Debug.Log("[ChestEvent] Event started.");
    }

    public void CancelEvent()
    {
        if (!eventInProgress)
            return;

        mysteriousGirl?.Disappear();

        eventInProgress = false;

        CinematicManager.Instance?.EndCinematic();
    }
}
