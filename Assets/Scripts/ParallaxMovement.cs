using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls horizontal parallax movement while keeping the complete
/// background aligned with the camera vertically.
/// </summary>
public class ParallaxMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

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
    private float _initialCameraY;
    private float _initialBackgroundY;

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
            Debug.LogError("[ParallaxMovement] Main Camera was not found.");
            return;
        }

        _initialCameraX = cameraTransform.position.x;
        _initialCameraY = cameraTransform.position.y;
        _initialBackgroundY = transform.position.y;

        CreateLayerData();
    }

    /// <summary>
    /// Creates the additional tiles required to keep each layer continuous.
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

            Renderer layerRenderer = layer.GetComponent<Renderer>();

            if (layerRenderer == null)
            {
                Debug.LogWarning(
                    $"[ParallaxMovement] Renderer not found on layer {layer.name}."
                );

                continue;
            }

            float width = layerRenderer.bounds.size.x;

            Transform leftTile = Instantiate(layer, transform);
            Transform rightTile = Instantiate(layer, transform);

            leftTile.name = $"{layer.name}_Left";
            rightTile.name = $"{layer.name}_Right";

            leftTile.position =
                layer.position + Vector3.left * width;

            rightTile.position =
                layer.position + Vector3.right * width;

            _layers.Add(new LayerData
            {
                original = layer,
                leftTile = leftTile,
                rightTile = rightTile,
                width = width,
                initialX = layer.position.x,
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

        float cameraDeltaX =
            cameraTransform.position.x - _initialCameraX;

        float cameraDeltaY =
            cameraTransform.position.y - _initialCameraY;

        // Keep the complete background aligned with the camera vertically.
        Vector3 backgroundPosition = transform.position;

        backgroundPosition.y =
            _initialBackgroundY + cameraDeltaY;

        transform.position = backgroundPosition;

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
    /// Updates only the horizontal position of a background tile.
    /// Vertical movement is handled by the Background parent.
    /// </summary>
    private void SetTilePosition(
        Transform tile,
        float x)
    {
        Vector3 position = tile.localPosition;
        position.x = x;
        tile.localPosition = position;
    }

    /// <summary>
    /// Repositions tiles when they move outside the camera area.
    /// </summary>
    private void WrapTiles(LayerData layer)
    {
        float cameraX = cameraTransform.position.x;

        Transform[] tiles =
        {
            layer.original,
            layer.leftTile,
            layer.rightTile
        };

        foreach (Transform tile in tiles)
        {
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
    /// Returns the configured parallax factor for a layer.
    /// </summary>
    private float GetParallaxFactor(int index)
    {
        if (index < parallaxFactors.Length)
        {
            return parallaxFactors[index];
        }

        return 0.5f;
    }
}