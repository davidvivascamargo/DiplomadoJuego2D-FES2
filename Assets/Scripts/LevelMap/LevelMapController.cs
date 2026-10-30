
using UnityEngine;

/// <summary>
/// Controls level nodes and restores saved progression.
/// </summary>
public class LevelMapController : MonoBehaviour
{
    [Header("Level Nodes")]
    [SerializeField] private LevelNode[] levelNodes;

    private void Start()
    {
        InitializeLevelMap();
    }

    private void InitializeLevelMap()
    {
        int highestUnlocked =
            LevelProgressManager.GetHighestUnlockedLevel();

        foreach (LevelNode node in levelNodes)
        {
            if (node == null)
                continue;

            int levelNumber = node.GetLevelNumber();

            if (levelNumber < highestUnlocked)
            {
                node.SetState(LevelNode.NodeState.Completed);
            }
            else if (levelNumber == highestUnlocked)
            {
                node.SetState(LevelNode.NodeState.Available);
            }
            else
            {
                node.SetState(LevelNode.NodeState.Locked);
            }
        }

        Debug.Log(
            $"[LevelMap] Highest unlocked level: {highestUnlocked}"
        );
    }
}
