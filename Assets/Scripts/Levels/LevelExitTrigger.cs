
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls level exit, key validation and progression.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class LevelExitTrigger : MonoBehaviour
{
    [Header("Level Progress")]
    [SerializeField, Min(1)] private int levelNumber = 1;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private string dialogueTitle = "FIN DEL NIVEL";

    [TextArea(2, 4)]
    [SerializeField] private string confirmationMessage =
        "Has completado el nivel. ¿Deseas regresar al mapa?";

    [TextArea(2, 4)]
    [SerializeField] private string lockedMessage =
        "Necesitas la llave para continuar.";

    [Header("Requirements")]
    [SerializeField] private ItemData requiredItem;

    [Header("Scene")]
    [SerializeField] private string destinationScene = "WorldMap";

    private bool _playerInside;
    private bool _dialogueShownDuringVisit;
    private bool _isExiting;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = true;

        TryShowExitDialogue(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!_dialogueShownDuringVisit)
            TryShowExitDialogue(other);
    }

    private void TryShowExitDialogue(Collider2D player)
    {
        if (_isExiting || !_playerInside)
            return;

        if (_dialogueShownDuringVisit || dialogueManager == null)
            return;

        if (dialogueManager.IsDialogueOpen)
            return;

        PlayerInventory inventory =
            player.GetComponentInParent<PlayerInventory>();

        bool hasRequiredItem =
            requiredItem == null ||
            (inventory != null && inventory.HasItem(requiredItem));

        _dialogueShownDuringVisit = true;

        if (!hasRequiredItem)
        {
            dialogueManager.ShowConfirmation(
                dialogueTitle,
                lockedMessage
            );
            return;
        }

        dialogueManager.ShowConfirmation(
            dialogueTitle,
            confirmationMessage,
            LoadDestinationScene
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = false;
        _dialogueShownDuringVisit = false;
    }

    private void LoadDestinationScene()
    {
        if (_isExiting || !_playerInside)
            return;

        if (string.IsNullOrWhiteSpace(destinationScene))
        {
            Debug.LogError(
                "[LevelExitTrigger] Destination scene is empty.",
                this
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(destinationScene))
        {
            Debug.LogError(
                $"[LevelExitTrigger] Scene '{destinationScene}' is unavailable.",
                this
            );
            return;
        }

        _isExiting = true;

        LevelProgressManager.CompleteLevel(levelNumber);

        Debug.Log(
            $"[LevelExitTrigger] Level {levelNumber} completed."
        );

        SceneManager.LoadScene(destinationScene);
    }
}
