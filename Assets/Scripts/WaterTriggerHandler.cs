using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    [Header("Water Detection")]
    [SerializeField] private LayerMask _waterMask;

    [Header("Splash")]
    [SerializeField] private GameObject _splashParticles;

    private EdgeCollider2D _edgeColl;
    private InteractableWater _water;

    private void Awake()
    {
        _edgeColl = GetComponent<EdgeCollider2D>();
        _water = GetComponent<InteractableWater>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if ((_waterMask.value & (1 << collision.gameObject.layer)) == 0)
        {

            return;
        }

        Rigidbody2D rb =
            collision.GetComponentInParent<Rigidbody2D>();

        if (rb == null)
        {

            return;
        }

        float splashForce = rb.linearVelocity.y;

        splashForce = Mathf.Clamp(
            splashForce,
            -_water.MaxForce,
            _water.MaxForce
        );

        if (_splashParticles != null)
        {
            Instantiate(
                _splashParticles,
                collision.transform.position,
                Quaternion.identity
            );
        }

        _water.Splash(
            collision,
            splashForce
        );
    }
}