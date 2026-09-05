using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;

    void Start()
    {
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