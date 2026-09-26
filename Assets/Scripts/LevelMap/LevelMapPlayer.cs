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

    private bool _isMoving;

    private void Start()
    {
        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;
        }

        // TEMPORARY TEST:
        // Allows movement to Level 2 without completing Level 1.
        // REMOVE AFTER TESTING THE MAP MOVEMENT.
        UnlockNextNodeForTesting();
    }

    private void Update()
    {
        if (_isMoving || currentNode == null)
        {
            return;
        }

        HandleMovementInput();
    }

    /// <summary>
    /// Handles directional input for moving between connected map nodes.
    /// </summary>
    private void HandleMovementInput()
    {
        float horizontal = Keyboard.current.dKey.isPressed
            ? 1f
            : Keyboard.current.aKey.isPressed
                ? -1f
                : 0f;

        float vertical = Keyboard.current.wKey.isPressed
            ? 1f
            : Keyboard.current.sKey.isPressed
                ? -1f
                : 0f;

        Vector2 inputDirection = new Vector2(horizontal, vertical);

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

            /*
            // TEMPORARILY DISABLED FOR MOVEMENT TESTING.
            // The real progression system must prevent movement
            // to locked nodes.
            //
            // REMOVE THIS COMMENTED BLOCK AFTER TESTING.
            if (targetNode.GetState() == LevelNode.NodeState.Locked)
            {
                continue;
            }
            */

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

    // TEMPORARY TEST:
    // Unlocks Level 2 so the path between Level 1 and Level 2
    // can be tested without implementing level completion yet.
    //
    // REMOVE THIS METHOD AFTER TESTING THE MAP MOVEMENT.
    private void UnlockNextNodeForTesting()
    {
        if (currentNode == null)
        {
            return;
        }

        LevelNodeConnection[] connections = currentNode.GetConnections();

        if (connections == null || connections.Length == 0)
        {
            return;
        }

        foreach (LevelNodeConnection connection in connections)
        {
            if (connection != null && connection.TargetNode != null)
            {
                connection.TargetNode.SetState(LevelNode.NodeState.Available);
                break;
            }
        }
    }
}