using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Configuration")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireballPrefab;

    private void Awake()
    {
        Debug.Log($"[PlayerAttack] Awake en {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log($"[PlayerAttack] Start en {gameObject.name}");
        Debug.Log($"[PlayerAttack] FirePoint = {firePoint}");
        Debug.Log($"[PlayerAttack] FireballPrefab = {fireballPrefab}");
    }

    private void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
             Debug.Log("[PlayerAttack] OnFire() EJECUTADO");
            animator.SetTrigger("Shoot");
        }
    }

    public void CastFireBall()
    {
        Debug.Log("[PlayerAttack] =================================");
        Debug.Log("[PlayerAttack] CastFireBall() EJECUTADO");
        Debug.Log($"[PlayerAttack] FirePoint = {firePoint}");
        Debug.Log($"[PlayerAttack] FireballPrefab = {fireballPrefab}");
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Falta asignar el FirePoint o el FireballPrefab.");
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