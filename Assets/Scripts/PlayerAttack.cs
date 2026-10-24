using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Configuration")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireballPrefab;

    private PlayerEnergy playerEnergy;

    private void Awake()
    {
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    private void OnFire(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        if (playerEnergy == null || !playerEnergy.CanFire)
        {
            return;
        }

        animator.SetTrigger("Shoot");
    }

    public void CastFireBall()
    {
        if (fireballPrefab == null || firePoint == null)
        {
            return;
        }

        if (playerEnergy == null ||
            !playerEnergy.TryConsumeFireballEnergy())
        {
            return;
        }

        float direction = transform.localScale.x > 0 ? 1f : -1f;

        GameObject fireballObject = Instantiate(
            fireballPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Fireball fireball = fireballObject.GetComponent<Fireball>();

        if (fireball != null)
        {
            fireball.SetDirection(direction);
        }
    }
}