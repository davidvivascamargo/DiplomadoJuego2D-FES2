using UnityEngine;

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

    [Header("Node Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite availableSprite;
    [SerializeField] private Sprite completedSprite;

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
}