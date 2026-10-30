
using UnityEngine;

/// <summary>
/// Activates the bonus stage when the player enters the trigger.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class BonusStageTrigger : MonoBehaviour
{
    [Header("Bonus Configuration")]
    [SerializeField] private BarrelBonusManager bonusManager;
    [SerializeField] private bool activateOnlyOnce = true;

    private bool _activated;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "[BonusTrigger] Object detected: " + other.name
        );

        if (!other.CompareTag("Player"))
            return;

        if (activateOnlyOnce && _activated)
            return;

        if (bonusManager == null)
        {
            Debug.LogError(
                "[BonusTrigger] Bonus Manager is missing!",
                this
            );
            return;
        }

        if (bonusManager.IsRunning)
            return;

        bonusManager.StartBonus();

        _activated = true;

        Debug.Log(
            "[BonusTrigger] Player activated the bonus!"
        );
    }
}
