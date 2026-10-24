
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MummyMovement))]
public class MummyDamage : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 5f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDelay = 1.5f;

    private MummyMovement mummyMovement;
    private Coroutine explosionCoroutine;
    private bool hasExploded;

    private void Awake()
    {
        mummyMovement = GetComponent<MummyMovement>();
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null || hasExploded)
            return;

        float distance = Vector2.Distance(
            transform.position,
            playerTransform.position
        );

        if (distance <= detectionRange)
        {
            if (explosionCoroutine == null)
            {
                explosionCoroutine =
                    StartCoroutine(PrepareExplosion());
            }
        }
        else if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null;

            mummyMovement.ResumeMovement();
        }
    }

    private IEnumerator PrepareExplosion()
    {
        mummyMovement.StopMovement();

        yield return new WaitForSeconds(explosionDelay);

        hasExploded = true;

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
}
