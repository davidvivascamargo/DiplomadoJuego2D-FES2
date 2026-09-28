using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's movement between level nodes on the world map.
/// </summary>
public class LevelMapPlayer : MonoBehaviour
{
    [Header("Map Configuration")]
    [SerializeField] private LevelNode currentNode;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private InputActionReference moveAction;

    [Header("Selection")]
    [SerializeField] private InputActionReference selectNodeAction;

    private bool _isMoving;

    private void Start()
    {
        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }

        if (selectNodeAction != null)
        {
            selectNodeAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (selectNodeAction != null)
        {
            selectNodeAction.action.Disable();
        }
    }

    private void Update()
    {
        if (currentNode == null)
        {
            return;
        }

        if (!_isMoving)
        {
            HandleMovementInput();
            HandleNodeSelection();
        }
    }

    /// <summary>
    /// Handles directional input for moving between connected map nodes.
    /// </summary>
    private void HandleMovementInput()
    {
        if (moveAction == null)
        {
            return;
        }

        Vector2 inputDirection = moveAction.action.ReadValue<Vector2>();

        if (inputDirection == Vector2.zero)
        {
            return;
        }

        LevelNodeConnection connection = FindAvailableConnection(inputDirection);

        if (connection != null)
        {
            StartCoroutine(MoveAlongPath(connection));
        }
    }

    /// <summary>
    /// Handles the input used to enter the currently selected level.
    /// </summary>
    private void HandleNodeSelection()
    {
        if (selectNodeAction == null)
        {
            return;
        }

        if (!selectNodeAction.action.WasPressedThisFrame())
        {
            return;
        }

        SelectCurrentNode();
    }

    /// <summary>
    /// Loads the scene associated with the current node.
    /// </summary>
    private void SelectCurrentNode()
    {
        if (currentNode.GetState() == LevelNode.NodeState.Locked)
        {
            return;
        }

        string sceneName = currentNode.GetSceneName();

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning(
                $"[LevelMapPlayer] No scene configured for level {currentNode.GetLevelNumber()}."
            );

            return;
        }

        SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();

        if (sceneLoader == null)
        {
            Debug.LogError("[LevelMapPlayer] SceneLoader was not found.");
            return;
        }

        sceneLoader.LoadScene(sceneName);
    }

    /// <summary>
    /// Finds the connected node that best matches the requested movement direction.
    /// </summary>
    private LevelNodeConnection FindAvailableConnection(Vector2 inputDirection)
    {
        LevelNodeConnection[] connections = currentNode.GetConnections();

        if (connections == null || connections.Length == 0)
        {
            return null;
        }

        inputDirection.Normalize();

        LevelNodeConnection bestConnection = null;
        float bestScore = -1f;

        foreach (LevelNodeConnection connection in connections)
        {
            if (connection == null || connection.TargetNode == null)
            {
                continue;
            }

            LevelNode targetNode = connection.TargetNode;

            if (targetNode.GetState() == LevelNode.NodeState.Locked)
            {
                continue;
            }

            Vector2 directionToTarget =
                targetNode.transform.position - currentNode.transform.position;

            if (directionToTarget == Vector2.zero)
            {
                continue;
            }

            directionToTarget.Normalize();

            float score = Vector2.Dot(inputDirection, directionToTarget);

            if (score > bestScore)
            {
                bestScore = score;
                bestConnection = connection;
            }
        }

        return bestConnection;
    }

    /// <summary>
    /// Moves the player through every point in the selected path.
    /// </summary>
    private IEnumerator MoveAlongPath(LevelNodeConnection connection)
    {
        _isMoving = true;

        Transform[] pathPoints = connection.PathPoints;

        if (pathPoints != null)
        {
            foreach (Transform pathPoint in pathPoints)
            {
                if (pathPoint == null)
                {
                    continue;
                }

                yield return MoveToPosition(pathPoint.position);
            }
        }

        if (connection.TargetNode != null)
        {
            yield return MoveToPosition(connection.TargetNode.transform.position);

            currentNode = connection.TargetNode;
        }

        _isMoving = false;
    }

    /// <summary>
    /// Moves the player smoothly toward a target position.
    /// </summary>
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
    }
}