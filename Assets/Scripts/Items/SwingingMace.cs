
using UnityEngine;

public class SwingingMace : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private float swingAngle = 90f;
    [SerializeField] private float swingSpeed = 1.5f;
    [SerializeField] private float startOffset = 0f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float angle = Mathf.Sin(
            (Time.time + startOffset) * swingSpeed
        ) * swingAngle;

        transform.localRotation =
            initialRotation * Quaternion.Euler(0f, 0f, angle);
    }
}
