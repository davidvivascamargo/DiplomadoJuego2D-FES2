using UnityEngine;

/// <summary>
/// Controls the level nodes and their progression state on the world map.
/// </summary>
public class LevelMapController : MonoBehaviour
{
    [Header("Level Nodes")]
    [SerializeField] private LevelNode[] levelNodes;

    private void Start()
    {
        InitializeLevelMap();
    }

    /// <summary>
    /// Initializes all level nodes as locked and makes the first level available.
    /// </summary>
    private void InitializeLevelMap()
    {
        foreach (LevelNode node in levelNodes)
        {
            if (node == null)
            {
                continue;
            }

            node.SetState(LevelNode.NodeState.Locked);
        }

        LevelNode firstLevel = GetLevelNode(1);

        if (firstLevel != null)
        {
            firstLevel.SetState(LevelNode.NodeState.Available);
        }
    }

    /// <summary>
    /// Finds a level node by its configured level number.
    /// </summary>
    private LevelNode GetLevelNode(int levelNumber)
    {
        foreach (LevelNode node in levelNodes)
        {
            if (node != null && node.GetLevelNumber() == levelNumber)
            {
                return node;
            }
        }

        return null;
    }
}