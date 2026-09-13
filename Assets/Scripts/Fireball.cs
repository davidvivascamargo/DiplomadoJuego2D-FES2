using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;

    private void Awake()
    {
        Debug.Log(
            $"[Fireball] AWAKE | " +
            $"Nombre: {gameObject.name} | " +
            $"Padre: {(transform.parent != null ? transform.parent.name : "NINGUNO")} | " +
            $"Posición: {transform.position}"
        );
    }

    void Start()
    {
            Debug.Log(
            $"[Fireball] START | " +
            $"Nombre: {gameObject.name} | " +
            $"Posición: {transform.position}"   
        );
        Destroy(gameObject, 2f); 
    }

    public void SetDirection(float direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(speed * direction, 0f);
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player")) return;

        Debug.Log("La Bola de Fuego ha chocado con: " + hitInfo.name);
        Destroy(gameObject);
    }
}