using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [SerializeField] private Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. FILTRO ÚNICO Y ESTRICTO: Si lo que entró NO es el jugador, lo ignoramos por completo
        if (!collision.gameObject.CompareTag("Player"))
        {
            return; 
        }

        // 2. Si el jugador ya viene saliendo del portal de destino, ignorar para evitar bucle infinito
        if (portalObjects.Contains(collision.gameObject))
        {
            return;
        }

        // 3. Registrar al jugador en el portal de DESTINO antes de moverlo
        if (destination != null && destination.TryGetComponent(out Teleport destinationPortal))
        {
            destinationPortal.portalObjects.Add(collision.gameObject);
        }

        // 4. Teletransportación física (Evita parpadeos y fallos de colisión)
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            rb.position = destination.position;
        }
        else
        {
            collision.transform.position = destination.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 5. Limpiar el registro cuando el jugador se aleje del portal
        if (collision.gameObject.CompareTag("Player") && portalObjects.Contains(collision.gameObject))
        {
            portalObjects.Remove(collision.gameObject);
        }
    }
}
