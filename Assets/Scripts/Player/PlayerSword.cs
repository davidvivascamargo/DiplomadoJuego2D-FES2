
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : MonoBehaviour
{
    [Header("Sword")]
    [SerializeField] private GameObject swordVisual;

    [Header("Attack")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string attackTrigger = "SwordAttack";

    private bool hasSword;

    public bool HasSword => hasSword;

    private void Awake()
    {
        hasSword = false;

        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        if (swordVisual != null)
            swordVisual.SetActive(false);
    }

    public void UnlockSword()
    {
        if (hasSword)
            return;

        hasSword = true;

        Debug.Log("[PlayerSword] Red sword unlocked.");

        if (swordVisual != null)
            swordVisual.SetActive(true);
    }

    // Called by PlayerInput using Send Messages.
    public void OnSwordAttack(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (!hasSword)
        {
            Debug.Log("[PlayerSword] Sword not unlocked yet.");
            return;
        }

        if (CinematicManager.Instance != null &&
            CinematicManager.Instance.IsCinematic)
            return;

        Debug.Log("[PlayerSword] Sword attack executed.");

        if (playerAnimator != null)
            playerAnimator.SetTrigger(attackTrigger);
    }
}
