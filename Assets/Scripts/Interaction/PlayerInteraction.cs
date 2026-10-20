
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Dialogue References")]
    [SerializeField] private DialogueManager dialogueManager;

    private IInteractable currentInteractable;
    private Collider2D currentInteractableCollider;

    private void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((interactableLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        IInteractable interactable =
            other.GetComponentInParent<IInteractable>();

        if (interactable == null)
            return;

        currentInteractable = interactable;
        currentInteractableCollider = other;

        Debug.Log("[Interaction] Object available: " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != currentInteractableCollider)
            return;

        currentInteractable = null;
        currentInteractableCollider = null;
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        // During a cinematic, E advances the dialogue.
        if (CinematicManager.Instance != null &&
            CinematicManager.Instance.IsCinematic)
        {
            if (dialogueManager != null &&
                dialogueManager.IsCinematicDialogueOpen)
            {
                dialogueManager.ContinueCinematicDialogue();
            }

            return;
        }

        // Do not interact while the game is paused.
        if (CinematicManager.Instance != null &&
            !CinematicManager.Instance.IsPlaying)
            return;

        // Normal gameplay interaction.
        currentInteractable?.Interact(gameObject);
    }
}
