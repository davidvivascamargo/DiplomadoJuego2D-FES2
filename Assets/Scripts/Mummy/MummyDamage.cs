
using UnityEngine;

public class MummyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    public int Damage => damage;

    private Collider2D mummyCollider;
    private Rigidbody2D mummyRigidbody;
    private float diagnosticTimer;

    private void Awake()
    {
        mummyCollider = GetComponent<Collider2D>();
        mummyRigidbody = GetComponent<Rigidbody2D>();

        Debug.Log(
            $"[MummyDamage] INICIO | " +
            $"Objeto: {name} | " +
            $"Capa: {LayerMask.LayerToName(gameObject.layer)} | " +
            $"Collider: {(mummyCollider != null ? mummyCollider.GetType().Name : "NO ENCONTRADO")} | " +
            $"Collider habilitado: {(mummyCollider != null && mummyCollider.enabled)} | " +
            $"IsTrigger: {(mummyCollider != null && mummyCollider.isTrigger)} | " +
            $"Rigidbody2D: {(mummyRigidbody != null ? "ENCONTRADO" : "NO ENCONTRADO")} | " +
            $"Simulated: {(mummyRigidbody != null && mummyRigidbody.simulated)}",
            this
        );

        if (mummyCollider == null)
            Debug.LogError("[MummyDamage] Falta un Collider2D en Mummy.", this);

        if (mummyRigidbody == null)
            Debug.LogError("[MummyDamage] Falta Rigidbody2D en Mummy.", this);
    }

    private void Update()
    {
        diagnosticTimer += Time.deltaTime;

        if (diagnosticTimer < 2f)
            return;

        diagnosticTimer = 0f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            5f
        );

        foreach (Collider2D hit in hits)
        {
            if (hit == mummyCollider)
                continue;

            Debug.Log(
                $"[MummyDamage] OBJETO CERCANO | " +
                $"Nombre: {hit.name} | " +
                $"Tag: {hit.tag} | " +
                $"Capa: {LayerMask.LayerToName(hit.gameObject.layer)} | " +
                $"Trigger: {hit.isTrigger} | " +
                $"Rigidbody2D: {(hit.attachedRigidbody != null ? hit.attachedRigidbody.name : "NINGUNO")}",
                this
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogError(
            $"[MummyDamage] TRIGGER DETECTADO: {other.name}",
            this
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogError(
            $"[MummyDamage] COLISIÓN DETECTADA: {collision.gameObject.name}",
            this
        );
    }
}