
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls horizontal parallax movement and keeps the background
/// vertically aligned with the camera.
/// </summary>
public class ParallaxMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Vertical Alignment")]
    [SerializeField] private float verticalOffset = 0f;

    [Header("Parallax Configuration")]
    [SerializeField]
    private float[] parallaxFactors =
    {
        0.05f,
        0.15f,
        0.30f,
        0.50f,
        0.75f
    };

    private readonly List<LayerData> _layers = new();

    private float _initialCameraX;

    private class LayerData
    {
        public Transform original;
        public Transform leftTile;
        public Transform rightTile;

        public float width;
        public float initialX;
        public float parallaxFactor;
    }

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        if (cameraTransform == null)
        {
            Debug.LogError(
                "[ParallaxMovement] Main Camera was not found."
            );

            enabled = false;
            return;
        }

        _initialCameraX = cameraTransform.position.x;

        // Align the background vertically at startup.
        UpdateVerticalPosition();

        CreateLayerData();
    }

    /// <summary>
    /// Creates additional copies of each background layer
    /// to allow horizontal repetition.
    /// </summary>
    private void CreateLayerData()
    {
        int layerCount = transform.childCount;

        for (int i = 0; i < layerCount; i++)
        {
            Transform layer = transform.GetChild(i);

            if (layer == null)
            {
                continue;
            }

            Renderer layerRenderer =
                layer.GetComponent<Renderer>();

            if (layerRenderer == null)
            {
                Debug.LogWarning(
                    $"[ParallaxMovement] Renderer not found on {layer.name}."
                );

                continue;
            }

            float width = layerRenderer.bounds.size.x;

            if (width <= 0f)
            {
                Debug.LogWarning(
                    $"[ParallaxMovement] Invalid width on {layer.name}."
                );

                continue;
            }

            Transform leftTile =
                Instantiate(layer, transform);

            Transform rightTile =
                Instantiate(layer, transform);

            leftTile.name = $"{layer.name}_Left";
            rightTile.name = $"{layer.name}_Right";

            // Keep all positions in local coordinates.
            leftTile.localPosition =
                layer.localPosition + Vector3.left * width;

            rightTile.localPosition =
                layer.localPosition + Vector3.right * width;

            _layers.Add(new LayerData
            {
                original = layer,
                leftTile = leftTile,
                rightTile = rightTile,
                width = width,
                initialX = layer.localPosition.x,
                parallaxFactor = GetParallaxFactor(i)
            });
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }

        UpdateVerticalPosition();

        float cameraDeltaX =
            cameraTransform.position.x - _initialCameraX;

        foreach (LayerData layer in _layers)
        {
            float targetX =
                layer.initialX +
                cameraDeltaX * layer.parallaxFactor;

            SetTilePosition(
                layer.original,
                targetX
            );

            SetTilePosition(
                layer.leftTile,
                targetX - layer.width
            );

            SetTilePosition(
                layer.rightTile,
                targetX + layer.width
            );

            WrapTiles(layer);
        }
    }

    /// <summary>
    /// Keeps the background aligned with the camera vertically.
    /// </summary>
    private void UpdateVerticalPosition()
    {
        Vector3 backgroundPosition = transform.position;

        backgroundPosition.y =
            cameraTransform.position.y + verticalOffset;

        transform.position = backgroundPosition;
    }

    /// <summary>
    /// Updates the horizontal position of a background tile.
    /// </summary>
    private void SetTilePosition(
        Transform tile,
        float x)
    {
        if (tile == null)
        {
            return;
        }

        Vector3 position = tile.localPosition;

        position.x = x;

        tile.localPosition = position;
    }

    /// <summary>
    /// Repositions background tiles when they move
    /// outside the visible horizontal area.
    /// </summary>
    private void WrapTiles(LayerData layer)
    {
        if (layer.width <= 0f)
        {
            return;
        }

        float cameraX = cameraTransform.position.x;

        Transform[] tiles =
        {
            layer.original,
            layer.leftTile,
            layer.rightTile
        };

        foreach (Transform tile in tiles)
        {
            if (tile == null)
            {
                continue;
            }

            float worldX = tile.position.x;

            if (worldX + layer.width <
                cameraX - layer.width)
            {
                Vector3 position = tile.position;

                position.x += layer.width * 3f;

                tile.position = position;
            }
            else if (worldX - layer.width >
                     cameraX + layer.width)
            {
                Vector3 position = tile.position;

                position.x -= layer.width * 3f;

                tile.position = position;
            }
        }
    }

    /// <summary>
    /// Returns the parallax factor assigned to a layer.
    /// </summary>
    private float GetParallaxFactor(int index)
    {
        if (parallaxFactors != null &&
            index < parallaxFactors.Length)
        {
            return parallaxFactors[index];
        }

        return 0.5f;
    }
}
