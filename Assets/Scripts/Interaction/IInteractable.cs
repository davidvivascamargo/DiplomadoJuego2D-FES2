using UnityEngine;

/// <summary>
/// Contract for objects that can be interacted with.
/// </summary>
public interface IInteractable
{
    void Interact(GameObject interactor);
}