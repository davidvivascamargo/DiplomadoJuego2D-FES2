
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ChestInteraction))]
public class ChestEventController : MonoBehaviour
{
    [Header("Cinematic Characters")]
    [SerializeField] private MysteriousGirlAppearance mysteriousGirl;

    [Header("Chest Animation")]
    [SerializeField] private Animator chestAnimator;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Crystal")]
    [SerializeField] private CrystalAnimationController crystalController;

    [Header("Crystal Timing")]
    [SerializeField, Min(0f)]
    private float crystalAppearanceDelay = 1f;

    private bool eventInProgress;
    private bool eventCompleted;
    private bool chestOpened;

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

        if (CinematicManager.Instance == null ||
            !CinematicManager.Instance.IsPlaying)
            return;

        if (dialogueManager == null)
        {
            Debug.LogWarning(
                "[ChestEvent] DialogueManager is not assigned."
            );
            return;
        }

        CinematicManager.Instance.StartCinematic();

        if (!CinematicManager.Instance.IsCinematic)
            return;

        eventInProgress = true;
        chestOpened = false;

        mysteriousGirl?.Appear();

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Parece que finalmente has llegado...",
            OnFirstDialogueFinished
        );

        Debug.Log("[ChestEvent] Event started.");
    }

    private void OnFirstDialogueFinished()
    {
        if (!eventInProgress)
            return;

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "He estado esperando este momento.",
            OnSecondDialogueFinished
        );
    }

    private void OnSecondDialogueFinished()
    {
        if (!eventInProgress)
            return;

        dialogueManager.ShowCinematicDecision(
            "Chica misteriosa",
            "Dentro de este cofre hay algo que podría ayudarte. ¿Quieres que lo abra?",
            OnAcceptChest,
            OnDeclineChest
        );
    }

    private void OnAcceptChest()
    {
        if (!eventInProgress || chestOpened)
            return;

        chestOpened = true;

        Debug.Log("[ChestEvent] Player accepted opening the chest.");

        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Open");
        }
        else
        {
            Debug.LogWarning(
                "[ChestEvent] Chest Animator is not assigned."
            );
        }

        StartCoroutine(ShowCrystalSequence());
    }

    private IEnumerator ShowCrystalSequence()
    {
        yield return new WaitForSecondsRealtime(
            crystalAppearanceDelay
        );

        if (!eventInProgress || !chestOpened)
            yield break;

        if (crystalController != null)
        {
            crystalController.Appear();

            Debug.Log(
                "[ChestEvent] Crystal appearance started."
            );
        }
        else
        {
            Debug.LogWarning(
                "[ChestEvent] CrystalAnimationController is not assigned."
            );
        }

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Este cristal ha permanecido dormido durante mucho tiempo...",
            OnCrystalDialogueFinished
        );
    }

    private void OnCrystalDialogueFinished()
    {
        if (!eventInProgress || !chestOpened)
            return;

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Este cristal contiene un poder capaz de ayudarte a enfrentar tus pesadillas.",
            OnPowerExplanationFinished
        );
    }

    private void OnPowerExplanationFinished()
    {
        if (!eventInProgress || !chestOpened)
            return;

        dialogueManager.ShowCinematicDecision(
            "Chica misteriosa",
            "¿Deseas aceptar el poder de este cristal?",
            OnAcceptPower,
            OnDeclinePower
        );
    }


    private void OnAcceptPower()
    {
        if (!eventInProgress || !chestOpened)
            return;

        Debug.Log("[ChestEvent] Player accepted the crystal power.");

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Entonces, permite que este poder tome una nueva forma...",
            null
        );

        if (crystalController != null)
        {
            crystalController.TransformToSword();
        }
        else
        {
            Debug.LogWarning(
                "[ChestEvent] CrystalAnimationController is not assigned."
            );
        }
    }


    private void OnDeclinePower()
    {
        if (!eventInProgress || !chestOpened)
            return;

        Debug.Log(
            "[ChestEvent] Player declined the crystal power."
        );

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Entiendo... No tienes que tomar esta decisión todavía.",
            null
        );

        // TODO: Reset the crystal and chest.
        // TODO: Hide the mysterious girl.
        // TODO: Finish the cinematic.
    }

    private void OnDeclineChest()
    {
        if (!eventInProgress)
            return;

        Debug.Log(
            "[ChestEvent] Player declined opening the chest."
        );

        CancelEvent();
    }

    public void CancelEvent()
    {
        // The current reset only supports cancellation
        // before the chest has been opened.
        if (!eventInProgress || chestOpened)
            return;

        dialogueManager?.CloseCinematicDialogue();

        mysteriousGirl?.Disappear();

        eventInProgress = false;

        CinematicManager.Instance?.EndCinematic();

        Debug.Log("[ChestEvent] Event cancelled.");
    }
}
