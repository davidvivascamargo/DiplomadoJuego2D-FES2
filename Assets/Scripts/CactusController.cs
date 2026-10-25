
using UnityEngine;

public class CactusController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float activationRange = 3f;
    [SerializeField] private float deactivationRange = 4f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Transform player;
    private bool spikesActive;
    private float nextDamageTime;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Update()
    {
        if (player == null || animator == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        // Activate cactus
        if (!spikesActive && distance <= activationRange)
        {
            spikesActive = true;

            animator.ResetTrigger("Deactivate");
            animator.SetTrigger("Activate");
        }
        // Deactivate cactus
        else if (spikesActive && distance >= deactivationRange)
        {
            spikesActive = false;

            animator.ResetTrigger("Activate");
            animator.SetTrigger("Deactivate");
        }

        // Damage player only when spikes are fully extended
        if (spikesActive &&
            distance <= activationRange &&
            IsSpikesActive() &&
            Time.time >= nextDamageTime)
        {
            DamagePlayer();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private bool IsSpikesActive()
    {
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        return state.IsName("Cactus_SpikesActive")
            && !animator.IsInTransition(0);
    }

    private void DamagePlayer()
    {
        if (GameManager.Instance == null)
            return;

        for (int i = 0; i < damage; i++)
        {
            GameManager.Instance.ReduceHealth();
        }

        Debug.Log("Cactus damaged Player: " + damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            activationRange
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            deactivationRange
        );
    }
}
