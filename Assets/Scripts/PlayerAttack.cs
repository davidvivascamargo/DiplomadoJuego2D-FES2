using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Configuration")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireballPrefab;

    private void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Shoot");
        }
    }

    public void CastFireBall()
    {
        if (fireballPrefab == null || firePoint == null)
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