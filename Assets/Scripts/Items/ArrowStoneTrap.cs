
using UnityEngine;

public class ArrowStoneTrap : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Shooting Settings")]
    [SerializeField] private float fireInterval = 4f;
    [SerializeField] private bool shootRight = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private float nextFireTime;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        nextFireTime = Time.time + Mathf.Max(0.1f, fireInterval);
    }

    private void Update()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + Mathf.Max(0.1f, fireInterval);

        ActivateTrap();
    }

    private void ActivateTrap()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError(
                "ArrowStoneTrap: Arrow Prefab or FirePoint is missing.",
                this
            );
            return;
        }

        Debug.Log("ArrowStoneTrap: Activating trap.");

        if (animator != null)
        {
            animator.ResetTrigger("Shoot");
            animator.SetTrigger("Shoot");
        }
        else
        {
            ShootArrow();
        }
    }

    // Called by an Animation Event in ArrowTrap_Shoot
    public void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
            return;

        Vector2 direction = shootRight
            ? Vector2.right
            : Vector2.left;

        GameObject arrow = Instantiate(
            arrowPrefab,
            firePoint.position,
            Quaternion.identity
        );

        ArrowProjectile projectile =
            arrow.GetComponent<ArrowProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "ArrowStoneTrap: Arrow prefab is missing ArrowProjectile.",
                arrow
            );

            Destroy(arrow);
            return;
        }

        projectile.Initialize(direction);

        SpriteRenderer sprite =
            arrow.GetComponent<SpriteRenderer>();

        if (sprite != null)
            sprite.flipX = !shootRight;

        Debug.Log(
            "ArrowStoneTrap: Arrow created at " +
            firePoint.position,
            arrow
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null)
            return;

        Gizmos.color = Color.red;

        Vector3 direction = shootRight
            ? Vector3.right
            : Vector3.left;

        Gizmos.DrawSphere(firePoint.position, 0.1f);
        Gizmos.DrawRay(firePoint.position, direction * 3f);
    }
}
