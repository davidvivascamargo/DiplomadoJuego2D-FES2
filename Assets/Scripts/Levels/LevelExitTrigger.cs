
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the exit point of a level.
/// Requires a key and opens a confirmation dialogue.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class LevelExitTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private string dialogueTitle = "FIN DEL DESIERTO";

    [TextArea(2, 4)]
    [SerializeField] private string confirmationMessage =
        "La llave te permite abandonar este lugar. ¿Deseas regresar al mapa?";

    [TextArea(2, 4)]
    [SerializeField] private string lockedMessage =
        "Necesitas la llave del desierto para continuar.";

    [Header("Requirements")]
    [SerializeField] private ItemData requiredItem;

    [Header("Scene")]
    [SerializeField] private string destinationScene;

    private bool _playerInside;
    private bool _dialogueShownDuringVisit;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        _playerInside = true;

        if (_dialogueShownDuringVisit || dialogueManager == null)
        {
            return;
        }

        if (dialogueManager.IsDialogueOpen)
        {
            return;
        }

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

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
        {
            return;
        }

        _playerInside = false;
        _dialogueShownDuringVisit = false;
    }

    /// <summary>
    /// Loads the destination selected in the Inspector.
    /// </summary>
    private void LoadDestinationScene()
    {
        if (string.IsNullOrWhiteSpace(destinationScene))
        {
            Debug.LogWarning(
                "[LevelExitTrigger] Destination scene is not assigned.",
                this
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(destinationScene))
        {
            Debug.LogWarning(
                $"[LevelExitTrigger] Scene '{destinationScene}' is not in the build scene list.",
                this
            );
            return;
        }

        SceneManager.LoadScene(destinationScene);
    }
}
