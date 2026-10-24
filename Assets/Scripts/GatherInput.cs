using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    private Controls controls;
    private PlayerEnergy playerEnergy;

    [SerializeField] private float _valueX;
    public float ValueX => _valueX;

    [SerializeField] private bool _isJumping;
    public bool IsJumping
    {
        get => _isJumping;
        set => _isJumping = value;
    }

    private void Awake()
    {
        controls = new Controls();
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += StartMove;
        controls.Player.Move.canceled += StopMove;

        controls.Player.Jump.performed += StartJump;
        controls.Player.Jump.canceled += StopJump;

        // Temporalmente desactivado para probar R directamente
        // controls.Player.Charge.performed += StartCharge;
        // controls.Player.Charge.canceled += StopCharge;

        controls.Player.Enable();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R DETECTADA");
        }

        if (playerEnergy != null)
        {
            playerEnergy.SetCharging(
                Keyboard.current.rKey.isPressed
            );
        }
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= StartMove;
        controls.Player.Move.canceled -= StopMove;

        controls.Player.Jump.performed -= StartJump;
        controls.Player.Jump.canceled -= StopJump;

        // controls.Player.Charge.performed -= StartCharge;
        // controls.Player.Charge.canceled -= StopCharge;

        playerEnergy?.SetCharging(false);

        controls.Player.Disable();
    }

    private void StartMove(InputAction.CallbackContext context)
    {
        _valueX = Mathf.RoundToInt(context.ReadValue<float>());
    }

    private void StopMove(InputAction.CallbackContext context)
    {
        _valueX = 0f;
    }

    private void StartJump(InputAction.CallbackContext context)
    {
        _isJumping = true;
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        _isJumping = false;
    }
}