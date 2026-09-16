using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(EdgeCollider2D))]
[RequireComponent(typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour
{
    [Header("Springs")]
    [SerializeField] private float _spriteConstant = 1.4f;
    [SerializeField] private float _damping = 1.1f;

    [SerializeField] private float _spread = 0.5f;
    [SerializeField, Range(1, 10)] private int _wavePropagationIterations = 8;
    [SerializeField, Range(0f, 20f)] private float _speedMult = 5.5f;

    [Header("Force")]
    public float wavePropagationsIterations = 8;
    [Range(1f, 50f)] public float MaxForce = 5f;

    [Header("Collision")]
    [SerializeField, Range(1f, 10f)]
    private float _playerCollisionRadiusMult = 4.15f;

    [Header("Mesh Generation")]
    [Range(2, 500)] public int NumOfXVertices = 70;

    public float Width = 10f;
    public float Height = 4f;
    public Material WaterMaterial;

    private const int NUM_OF_Y_VERTICES = 2;

    [Header("Gizmo")]
    public Color GizmoColor = Color.white;

    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Vector3[] _vertices;
    private int[] _topVerticesIndex;

    private EdgeCollider2D _coll;

    private class WaterPoint
    {
        public float velocity;
        public float aceleration;
        public float pos;
        public float targetHeight;
    }

    private List<WaterPoint> _waterPoints = new List<WaterPoint>();

    private void Start()
    {
        _coll = GetComponent<EdgeCollider2D>();
        GenerateMesh();
        CreateWaterPoints();
    }

    private void Reset()
    {
        _coll = GetComponent<EdgeCollider2D>();

        if (_coll != null)
        {
            _coll.isTrigger = true;
        }
    }

    private void FixedUpdate()
    {
        // Update all spring positions
        for (int i = 1; i < _waterPoints.Count - 1; i++)
        {
            WaterPoint point = _waterPoints[i];

            float acceleration =
                -_spriteConstant * (point.pos - point.targetHeight)
                - _damping * point.velocity;

            point.pos += point.velocity * _speedMult * Time.fixedDeltaTime;

            _vertices[_topVerticesIndex[i]].y = point.pos;

            point.velocity += acceleration * _speedMult * Time.fixedDeltaTime;
        }

        // Wave propagation
        for (int j = 0; j < _wavePropagationIterations; j++)
        {
            for (int i = 1; i < _waterPoints.Count - 1; i++)
            {
                float leftDelta =
                    _spread *
                    (_waterPoints[i].pos - _waterPoints[i - 1].pos) *
                    _speedMult *
                    Time.fixedDeltaTime;

                _waterPoints[i - 1].velocity += leftDelta;

                float rightDelta =
                    _spread *
                    (_waterPoints[i].pos - _waterPoints[i + 1].pos) *
                    _speedMult *
                    Time.fixedDeltaTime;

                _waterPoints[i + 1].velocity += rightDelta;
            }
        }

        // Update the mesh
        _mesh.vertices = _vertices;
    }

    public void Splash(Collider2D collision, float force)
    {
        float radius =
            collision.bounds.extents.x * _playerCollisionRadiusMult;

        Vector2 center = collision.transform.position;

        for (int i = 0; i < _waterPoints.Count; i++)
        {
            Vector2 vertexWorldPos =
                transform.TransformPoint(
                    _vertices[_topVerticesIndex[i]]);

            if (IsPointInsideCircle(vertexWorldPos, center, radius))
            {
                _waterPoints[i].velocity = force;
            }
        }
    }

    private bool IsPointInsideCircle(
        Vector2 point,
        Vector2 center,
        float radius)
    {
        float distanceSquared =
            (point - center).sqrMagnitude;

        return distanceSquared <= radius * radius;
    }

    public void ResetEdgeCollider()
    {
        if (_vertices == null ||
            _topVerticesIndex == null ||
            _topVerticesIndex.Length < NumOfXVertices)
        {
            GenerateMesh();
        }

        _coll = GetComponent<EdgeCollider2D>();
        _coll.isTrigger = true;

        Vector2[] newPoints = new Vector2[2];

        Vector2 firstPoint =
            new Vector2(
                _vertices[_topVerticesIndex[0]].x,
                _vertices[_topVerticesIndex[0]].y);

        Vector2 secondPoint =
            new Vector2(
                _vertices[_topVerticesIndex[NumOfXVertices - 1]].x,
                _vertices[_topVerticesIndex[NumOfXVertices - 1]].y);

        newPoints[0] = firstPoint;
        newPoints[1] = secondPoint;

        _coll.offset = Vector2.zero;
        _coll.points = newPoints;
    }

    public void GenerateMesh()
    {
        _mesh = new Mesh();

        // 1. Asignar Vértices
        _vertices =
            new Vector3[NumOfXVertices * NUM_OF_Y_VERTICES];

        _topVerticesIndex =
            new int[NumOfXVertices];

        for (int y = 0; y < NUM_OF_Y_VERTICES; y++)
        {
            for (int x = 0; x < NumOfXVertices; x++)
            {
                float xPos =
                    (x / (float)(NumOfXVertices - 1)) *
                    Width -
                    (Width / 2f);

                float yPos =
                    (y / (float)(NUM_OF_Y_VERTICES - 1)) *
                    Height -
                    (Height / 2f);

                int indexVertex =
                    y * NumOfXVertices + x;

                _vertices[indexVertex] =
                    new Vector3(xPos, yPos, 0);

                if (y == NUM_OF_Y_VERTICES - 1)
                {
                    _topVerticesIndex[x] = indexVertex;
                }
            }
        }

        // 2. Construir Triángulos
        int[] triangles =
            new int[
                (NumOfXVertices - 1) *
                (NUM_OF_Y_VERTICES - 1) *
                6];

        int index = 0;

        for (int y = 0; y < NUM_OF_Y_VERTICES - 1; y++)
        {
            for (int x = 0; x < NumOfXVertices - 1; x++)
            {
                int bottomLeft =
                    y * NumOfXVertices + x;

                int bottomRight =
                    bottomLeft + 1;

                int topLeft =
                    bottomLeft + NumOfXVertices;

                int topRight =
                    topLeft + 1;

                // Primer triángulo
                triangles[index++] = bottomLeft;
                triangles[index++] = topLeft;
                triangles[index++] = bottomRight;

                // Segundo triángulo
                triangles[index++] = bottomRight;
                triangles[index++] = topLeft;
                triangles[index++] = topRight;
            }
        }

        // 3. Generar UVs
        Vector2[] uvs =
            new Vector2[_vertices.Length];

        for (int i = 0; i < _vertices.Length; i++)
        {
            uvs[i] =
                new Vector2(
                    (_vertices[i].x + Width / 2f) / Width,
                    (_vertices[i].y + Height / 2f) / Height);
        }

        // 4. Asignar Componentes y Malla
        if (_meshRenderer == null)
        {
            _meshRenderer =
                GetComponent<MeshRenderer>();
        }

        if (_meshFilter == null)
        {
            _meshFilter =
                GetComponent<MeshFilter>();
        }

        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _meshFilter.mesh = _mesh;

        if (WaterMaterial != null)
        {
            _meshRenderer.sharedMaterial =
                WaterMaterial;
        }
    }

    private void CreateWaterPoints()
    {
        _waterPoints.Clear();

        for (int i = 0; i < _topVerticesIndex.Length; i++)
        {
            _waterPoints.Add(
                new WaterPoint
                {
                    pos =
                        _vertices[_topVerticesIndex[i]].y,

                    targetHeight =
                        _vertices[_topVerticesIndex[i]].y
                });
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor
{
    private InteractableWater _water;

    private void OnEnable()
    {
        _water = (InteractableWater)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root =
            new VisualElement();

        InspectorElement.FillDefaultInspector(
            root,
            serializedObject,
            this);

        root.Add(
            new VisualElement
            {
                style =
                {
                    height = 10
                }
            });

        Button generateButton =
            new Button(
                () => _water.GenerateMesh())
            {
                text = "Generate Mesh"
            };

        root.Add(generateButton);

        Button placeEdgeColliderButton =
            new Button(
                () => _water.ResetEdgeCollider())
            {
                text = "Place Edge Collider"
            };

        root.Add(placeEdgeColliderButton);

        return root;
    }

    private void OnSceneGUI()
    {
        if (_water == null)
        {
            return;
        }

        // Draw the wireFrame box
        Handles.color = _water.GizmoColor;

        Vector3 center =
            _water.transform.position;

        Vector3 size =
            new Vector3(
                _water.Width,
                _water.Height,
                0.1f);

        Handles.DrawWireCube(center, size);

        // Handles for width and height
        float handleSize =
            HandleUtility.GetHandleSize(center) *
            0.1f;

        Vector3 snap =
            Vector3.one * 0.1f;

        // Corner handles
        Vector3[] corners =
            new Vector3[4];

        corners[0] =
            center +
            new Vector3(
                -_water.Width / 2,
                -_water.Height / 2,
                0);

        corners[1] =
            center +
            new Vector3(
                _water.Width / 2,
                -_water.Height / 2,
                0);

        corners[2] =
            center +
            new Vector3(
                -_water.Width / 2,
                _water.Height / 2,
                0);

        corners[3] =
            center +
            new Vector3(
                _water.Width / 2,
                _water.Height / 2,
                0);

        // Bottom-Left Handle
        EditorGUI.BeginChangeCheck();

        Vector3 newBottomLeft =
            Handles.FreeMoveHandle(
                corners[0],
                handleSize,
                snap,
                Handles.DotHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(
                _water,
                "Resize Water");

            ChangeDimensions(
                ref _water.Width,
                ref _water.Height,
                corners[1].x - newBottomLeft.x,
                corners[2].y - newBottomLeft.y);

            _water.transform.position +=
                new Vector3(
                    (newBottomLeft.x - corners[0].x) / 2,
                    (newBottomLeft.y - corners[0].y) / 2,
                    0);
        }

        // Top-Left Handle
        EditorGUI.BeginChangeCheck();

        Vector3 newTopLeft =
            Handles.FreeMoveHandle(
                corners[2],
                handleSize,
                snap,
                Handles.DotHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(
                _water,
                "Resize Water");

            ChangeDimensions(
                ref _water.Width,
                ref _water.Height,
                corners[3].x - newTopLeft.x,
                newTopLeft.y - corners[0].y);

            _water.transform.position +=
                new Vector3(
                    (newTopLeft.x - corners[2].x) / 2,
                    (newTopLeft.y - corners[2].y) / 2,
                    0);
        }

        // Top-Right Handle
        EditorGUI.BeginChangeCheck();

        Vector3 newTopRight =
            Handles.FreeMoveHandle(
                corners[3],
                handleSize,
                snap,
                Handles.DotHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(
                _water,
                "Resize Water");

            ChangeDimensions(
                ref _water.Width,
                ref _water.Height,
                newTopRight.x - corners[2].x,
                newTopRight.y - corners[1].y);

            _water.transform.position +=
                new Vector3(
                    (newTopRight.x - corners[3].x) / 2,
                    (newTopRight.y - corners[3].y) / 2,
                    0);
        }
    }

    private void ChangeDimensions(
        ref float width,
        ref float height,
        float calculatedWidthMax,
        float calculatedHeightMax)
    {
        width =
            Mathf.Clamp(
                calculatedWidthMax,
                0.1f,
                500f);

        height =
            Mathf.Clamp(
                calculatedHeightMax,
                0.1f,
                500f);
    }
}

#endif