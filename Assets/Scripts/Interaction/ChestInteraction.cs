
using UnityEngine;

[RequireComponent(typeof(ChestEventController))]
public class ChestInteraction : MonoBehaviour, IInteractable
{
    private ChestEventController chestEventController;

    private void Awake()
    {
        chestEventController = GetComponent<ChestEventController>();
    }

    public void Interact(GameObject interactor)
    {
        chestEventController.BeginEvent();
    }
}
