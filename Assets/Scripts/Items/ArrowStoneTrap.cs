
using UnityEngine;

public class ArrowStoneTrap : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Shooting Settings")]
    [SerializeField, Min(0.1f)]
    private float fireInterval = 4f;

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
        nextFireTime = Time.time + fireInterval;
    }

    private void Update()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireInterval;

        ActivateTrap();
    }

    private void ActivateTrap()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError(
                "[ArrowStoneTrap] Arrow Prefab or FirePoint is missing.",
                this
            );
            return;
        }

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

    // Called by an Animation Event in ArrowTrap_Shoot.
    public void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
            return;

        // Get the direction from FirePoint's orientation.
        Vector2 direction = firePoint.right;

        // Restrict movement to left or right.
        direction = direction.x >= 0f
            ? Vector2.right
            : Vector2.left;

        // Create the projectile at FirePoint.
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
                "[ArrowStoneTrap] ArrowProjectile component is missing.",
                arrow
            );

            Destroy(arrow);
            return;
        }

        // Initialize movement and orientation.
        projectile.Initialize(direction);

        Debug.Log(
            $"[ArrowStoneTrap] Arrow fired " +
            $"towards {(direction.x > 0 ? "RIGHT" : "LEFT")} " +
            $"from {firePoint.position}",
            arrow
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null)
            return;

        Vector3 direction = firePoint.right.x >= 0f
            ? Vector3.right
            : Vector3.left;

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(firePoint.position, 0.1f);

        Gizmos.DrawRay(
            firePoint.position,
            direction * 3f
        );
    }
}
