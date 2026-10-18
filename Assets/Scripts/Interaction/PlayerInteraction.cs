
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask interactableLayer;

    private IInteractable currentInteractable;
    private Collider2D currentInteractableCollider;

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

        if (CinematicManager.Instance != null &&
            !CinematicManager.Instance.IsPlaying)
            return;

        currentInteractable?.Interact(gameObject);
    }
}
