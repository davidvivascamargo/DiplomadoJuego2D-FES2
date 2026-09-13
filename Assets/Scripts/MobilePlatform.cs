using UnityEngine;

public class MobilePlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform[] movementPoints;
    [SerializeField] private float movementSpeed = 2f;

    [Header("Movement State")]
    [SerializeField] private int currentPoint = 1;
    [SerializeField] private bool moveForward = true;

    private void Update()
    {
        if (moveForward && currentPoint + 1 >= movementPoints.Length)
        {
            moveForward = false;
        }

        if (!moveForward && currentPoint - 1 < 0)
        {
            moveForward = true;
        }

        if (Vector2.Distance(
                transform.position,
                movementPoints[currentPoint].position) < 0.1f)
        {
            if (moveForward)
            {
                currentPoint++;
            }
            else
            {
                currentPoint--;
            }
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            movementPoints[currentPoint].position,
            movementSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}