using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Configuration")]
    public Animator animator;
    public Transform firePoint;
    public GameObject fireballPrefab;

    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Shoot");
        }
    }
    void CastFireBall()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireballObject = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            Fireball fireballScript = fireballObject.GetComponent<Fireball>();
           //Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
           if (fireballScript != null)
            {
                fireballScript.SetDirection(direction);
            }
        }
        else
        {
            Debug.LogWarning("Falta asignar el FirePoint o el FireballPrefab en el Inspector.");        
        }
    }
}
