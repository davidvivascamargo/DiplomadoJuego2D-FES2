
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    [Header("Sword")]
    [SerializeField] private GameObject swordVisual;

    private bool hasSword;

    public bool HasSword => hasSword;

    private void Awake()
    {
        hasSword = false;

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

    public void OnSwordAttack()
    {
        if (!hasSword)
        {
            Debug.Log("[PlayerSword] Sword not unlocked yet.");
            return;
        }

        if (CinematicManager.Instance != null &&
            CinematicManager.Instance.IsCinematic)
            return;

        Debug.Log("[PlayerSword] Sword attack executed.");

        // Next: play the sword attack animation
        // and apply damage to enemies.
    }
}
