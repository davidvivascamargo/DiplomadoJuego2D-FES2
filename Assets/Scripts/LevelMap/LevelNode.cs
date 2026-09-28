using UnityEngine;

[System.Serializable]
public class LevelNodeConnection
{
    [SerializeField] private LevelNode targetNode;
    [SerializeField] private Transform[] pathPoints;

    public LevelNode TargetNode => targetNode;

    public Transform[] PathPoints => pathPoints;
}

/// <summary>
/// Represents a level node on the world map and controls its visual state.
/// </summary>
public class LevelNode : MonoBehaviour
{
    public enum NodeState
    {
        Locked,
        Available,
        Completed
    }

    [Header("Level Configuration")]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private string sceneName;

    [Header("Node Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite availableSprite;
    [SerializeField] private Sprite completedSprite;

    [Header("Node Connections")]
    [SerializeField] private LevelNodeConnection[] connections;

    private SpriteRenderer _spriteRenderer;

    private NodeState _currentState;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetState(NodeState state)
    {
        _currentState = state;

        switch (state)
        {
            case NodeState.Locked:
                _spriteRenderer.sprite = lockedSprite;
                break;

            case NodeState.Available:
                _spriteRenderer.sprite = availableSprite;
                break;

            case NodeState.Completed:
                _spriteRenderer.sprite = completedSprite;
                break;
        }
    }

    public NodeState GetState()
    {
        return _currentState;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public string GetSceneName()
    {
        return sceneName;
    }

    public LevelNodeConnection[] GetConnections()
    {
        return connections;
    }
}