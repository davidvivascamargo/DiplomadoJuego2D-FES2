using System.Collections;
using UnityEngine;

public class ElectricWaterEffect : MonoBehaviour
{
    [Header("Electric Area")]
    [SerializeField] private BoxCollider2D _waterArea;

    [Header("Lightning")]
    [SerializeField] private Material _lightningMaterial;
    [SerializeField] private int _maxBolts = 3;
    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private float _boltDuration = 0.25f;

    [Header("Lightning Size")]
    [SerializeField] private float _minLength = 2f;
    [SerializeField] private float _maxLength = 6f;
    [SerializeField] private float _width = 0.15f;

    [Header("Lightning Shape")]
    [SerializeField] private int _segments = 8;
    [SerializeField] private float _verticalVariation = 0.35f;

    private void Start()
    {
        if (_waterArea == null)
        {
            _waterArea = GetComponentInParent<BoxCollider2D>();
        }

        StartCoroutine(SpawnLightningRoutine());
    }

    private IEnumerator SpawnLightningRoutine()
    {
        while (true)
        {
            if (_waterArea != null)
            {
                StartCoroutine(CreateLightning());
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private IEnumerator CreateLightning()
    {
        Bounds bounds = _waterArea.bounds;

        float length = Random.Range(_minLength, _maxLength);

        bool goesRight = Random.value > 0.5f;

        float startX;

        if (goesRight)
        {
            startX = Random.Range(
                bounds.min.x,
                bounds.max.x - length
            );
        }
        else
        {
            startX = Random.Range(
                bounds.min.x + length,
                bounds.max.x
            );
        }

        float startY = Random.Range(
            bounds.min.y,
            bounds.max.y
        );

        GameObject lightningObject = new GameObject("Lightning");

        lightningObject.transform.SetParent(transform);

        LineRenderer line = lightningObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = _segments;
        line.startWidth = 0f;
        line.endWidth = 0f;

        if (_lightningMaterial != null)
        {
            line.material = _lightningMaterial;
        }

        line.startColor = Color.white;
        line.endColor = Color.white;

        line.sortingLayerName = "Default";
        line.sortingOrder = 10;

        Vector3[] positions = new Vector3[_segments];

        for (int i = 0; i < _segments; i++)
        {
            float progress = (float)i / (_segments - 1);

            float x;

            if (goesRight)
            {
                x = startX + (length * progress);
            }
            else
            {
                x = startX - (length * progress);
            }

            float y = startY;

            if (i != 0 && i != _segments - 1)
            {
                y += Random.Range(
                    -_verticalVariation,
                    _verticalVariation
                );
            }

            positions[i] = new Vector3(x, y, 0f);
        }

        line.SetPositions(positions);

        float elapsed = 0f;

        while (elapsed < _boltDuration)
        {
            elapsed += Time.deltaTime;

            float progress = elapsed / _boltDuration;

            float width = Mathf.Lerp(
                0f,
                _width,
                Mathf.Clamp01(progress)
            );

            line.startWidth = width;
            line.endWidth = width;

            yield return null;
        }

        Destroy(lightningObject);
    }
}
