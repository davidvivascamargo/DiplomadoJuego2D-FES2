
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MummyMovement))]
[RequireComponent(typeof(Collider2D))]
public class MummyDamage : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private Transform playerTransform;
    [SerializeField, Min(0.1f)] private float detectionRange = 5f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField, Min(0f)] private float explosionDelay = 1.5f;

    private MummyMovement mummyMovement;
    private Collider2D mummyCollider;
    private Collider2D playerCollider;

    private Coroutine explosionCoroutine;
    private bool hasExploded;

    private void Awake()
    {
        mummyMovement = GetComponent<MummyMovement>();
        mummyCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (hasExploded)
            return;

        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        bool playerDetected = IsPlayerInRange();

        if (playerDetected)
        {
            if (explosionCoroutine == null)
            {
                explosionCoroutine =
                    StartCoroutine(PrepareExplosion());
            }
        }
        else
        {
            CancelExplosion();
        }
    }

    private void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        playerTransform = player.transform;

        playerCollider =
            player.GetComponent<Collider2D>();

        if (playerCollider == null)
        {
            playerCollider =
                player.GetComponentInChildren<Collider2D>();
        }
    }

    private bool IsPlayerInRange()
    {
        if (playerTransform == null)
            return false;

        if (playerCollider == null)
        {
            playerCollider =
                playerTransform.GetComponent<Collider2D>();

            if (playerCollider == null)
            {
                playerCollider =
                    playerTransform.GetComponentInChildren<Collider2D>();
            }
        }

        if (mummyCollider != null &&
            playerCollider != null &&
            mummyCollider.enabled &&
            playerCollider.enabled)
        {
            ColliderDistance2D distance =
                mummyCollider.Distance(playerCollider);

            return distance.isOverlapped ||
                   distance.distance <= detectionRange;
        }

        return Vector2.Distance(
            transform.position,
            playerTransform.position
        ) <= detectionRange;
    }

    private IEnumerator PrepareExplosion()
    {
        mummyMovement.StopMovement();

        yield return new WaitForSeconds(explosionDelay);

        hasExploded = true;
        explosionCoroutine = null;

        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    private void CancelExplosion()
    {
        if (explosionCoroutine == null)
            return;

        StopCoroutine(explosionCoroutine);
        explosionCoroutine = null;

        mummyMovement.ResumeMovement();
    }

    private void OnDisable()
    {
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null;
        }
    }
}
