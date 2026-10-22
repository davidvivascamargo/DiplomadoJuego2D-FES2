
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

    [Header("Sword Delivery")]
    [SerializeField, Min(0f)]
    private float swordDeliveryTimeout = 10f;

    private bool eventInProgress;
    private bool eventCompleted;
    private bool chestOpened;
    private bool powerAccepted;
    private bool waitingForSword;

    private Coroutine crystalSequenceCoroutine;
    private Coroutine swordSequenceCoroutine;

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
        powerAccepted = false;
        waitingForSword = false;

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

        Debug.Log(
            "[ChestEvent] Player accepted opening the chest."
        );

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

        crystalSequenceCoroutine =
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

        crystalSequenceCoroutine = null;
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
        if (!eventInProgress ||
            !chestOpened ||
            powerAccepted)
            return;

        if (crystalController == null)
        {
            Debug.LogWarning(
                "[ChestEvent] CrystalAnimationController is not assigned."
            );
            return;
        }

        powerAccepted = true;

        Debug.Log(
            "[ChestEvent] Player accepted the crystal power."
        );

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Entonces, permite que este poder tome una nueva forma...",
            OnTransformationDialogueFinished
        );

        crystalController.TransformToSword();
    }

    private void OnTransformationDialogueFinished()
    {
        if (!eventInProgress || !powerAccepted)
            return;

        waitingForSword = true;

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "El poder del cristal ahora te pertenece...",
            OnFinalDialogueFinished
        );

        Debug.Log(
            "[ChestEvent] Waiting for sword delivery."
        );
    }

    private void OnFinalDialogueFinished()
    {
        if (!eventInProgress || !powerAccepted)
            return;

        if (swordSequenceCoroutine != null)
            return;

        swordSequenceCoroutine =
            StartCoroutine(WaitForSwordAndFinish());
    }

    private IEnumerator WaitForSwordAndFinish()
    {
        // The crystal handles the transformation,
        // flight and sword unlock independently.
        // Wait briefly for the delivery to finish.

        PlayerSword playerSword = null;

        if (crystalController != null)
        {
            playerSword =
                FindFirstObjectByType<PlayerSword>();
        }

        if (playerSword == null)
        {
            Debug.LogWarning(
                "[ChestEvent] PlayerSword component not found."
            );

            swordSequenceCoroutine = null;
            yield break;
        }

        float elapsed = 0f;

        while (!playerSword.HasSword &&
               elapsed < swordDeliveryTimeout)
        {
            if (!eventInProgress)
            {
                swordSequenceCoroutine = null;
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        swordSequenceCoroutine = null;

        if (!playerSword.HasSword)
        {
            Debug.LogWarning(
                "[ChestEvent] Sword delivery timed out."
            );
            yield break;
        }

        CompleteEvent();
    }

    private void CompleteEvent()
    {
        if (!eventInProgress || eventCompleted)
            return;

        eventCompleted = true;
        eventInProgress = false;
        waitingForSword = false;

        dialogueManager?.CloseCinematicDialogue();

        mysteriousGirl?.Disappear();

        CinematicManager.Instance?.EndCinematic();

        Debug.Log(
            "[ChestEvent] Event completed. Sword unlocked."
        );
    }

    private void OnDeclinePower()
    {
        if (!eventInProgress ||
            !chestOpened ||
            powerAccepted)
            return;

        Debug.Log(
            "[ChestEvent] Player declined the crystal power."
        );

        dialogueManager.ShowCinematicMessage(
            "Chica misteriosa",
            "Entiendo... No tienes que tomar esta decisión todavía.",
            OnDeclinePowerDialogueFinished
        );
    }

    private void OnDeclinePowerDialogueFinished()
    {
        // Reset after opening the chest will be
        // implemented separately.
        Debug.LogWarning(
            "[ChestEvent] Chest reset after declining power is pending."
        );
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
        if (!eventInProgress || chestOpened)
            return;

        if (crystalSequenceCoroutine != null)
        {
            StopCoroutine(crystalSequenceCoroutine);
            crystalSequenceCoroutine = null;
        }

        if (swordSequenceCoroutine != null)
        {
            StopCoroutine(swordSequenceCoroutine);
            swordSequenceCoroutine = null;
        }

        dialogueManager?.CloseCinematicDialogue();

        mysteriousGirl?.Disappear();

        eventInProgress = false;
        powerAccepted = false;
        waitingForSword = false;

        CinematicManager.Instance?.EndCinematic();

        Debug.Log("[ChestEvent] Event cancelled.");
    }
}
