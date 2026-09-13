using UnityEngine;

public class Enemy : MonoBehaviour
{

    private void Awake()
    {
        Debug.Log("[Enemy] Script iniciado en: " + gameObject.name);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Enemy] Trigger detectado con: {collision.name}");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("[Enemy] El enemigo ha golpeado al jugador.");

            GameManager.Instance.ReduceHealth();
        }
    }
}