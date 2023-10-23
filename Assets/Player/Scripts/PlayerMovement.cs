using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _controllerDeadZone = 0.1f;
    [SerializeField] private float _gamepadRotateSmoothing = 1000f;
    [SerializeField] private Transform _aimPoint;
    [SerializeField] private float _aimPointOffset = 5.0f;
    [SerializeField] private float rollDistance = 10f;  // Distance to roll
    [SerializeField] private float rollDuration = 0.5f;  // Duration of the roll
    [SerializeField] private float rollCooldown = 2f;  // Cooldown between rolls

    public static bool IsActionHappening = false;

    private float lastRollTime;
    private Vector3 rollDirection;
    private bool isRolling;
    private Vector3 rollStartPosition;

    public static Vector3 LastMovementDirection { get; private set; }

    private bool _isGamePad;
    private bool _isSprinting;

    private Vector2 _moveInput;
    private Vector2 _aimInput;
    private Vector3 _playerVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleAim();
        if (isRolling)
        {
            Roll();
        }
    }

    public void OnDeviceChange(PlayerInput input)
    {
        _isGamePad = input.currentControlScheme.Equals("Gamepad");
    }

    private void HandleAim()
    {
        if (_isGamePad)
        {
            if (Mathf.Abs(_aimInput.x) > _controllerDeadZone || Mathf.Abs(_aimInput.y) > _controllerDeadZone)
            {
                Vector3 playerDirection = Vector3.right * _aimInput.x + Vector3.forward * _aimInput.y;
                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, _gamepadRotateSmoothing * Time.deltaTime);
                    _aimPoint.position = transform.position + transform.forward * _aimPointOffset;
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(_aimInput);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
                Vector3 directionToMouse = (point - transform.position).normalized;
                directionToMouse.y = 0f;
                // Set the reticle position
                _aimPoint.position = transform.position + directionToMouse * _aimPointOffset;
            }

        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    private void HandleMovement()
    {
        // Movement
        float totalMovespeed = _moveSpeed;
        if (_isSprinting)
            totalMovespeed = _moveSpeed + _sprintSpeed;
        
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y) * totalMovespeed * Time.deltaTime;
        characterController.Move(move * Time.deltaTime * _moveSpeed);
        LastMovementDirection = characterController.velocity.normalized;

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        characterController.Move(_playerVelocity * Time.deltaTime);
    }

    private void StartRoll()
    {
        isRolling = true;
        rollDirection = LastMovementDirection;
        if (rollDirection == Vector3.zero)
        {
            rollDirection = transform.forward;
        }
        rollStartPosition = transform.position;
        lastRollTime = Time.time;
    }

    private void Roll()
    {
        float rollProgress = (Time.time - lastRollTime) / rollDuration;
        if (rollProgress >= 1f)
        {
            isRolling = false;
            return;
        }

        Vector3 targetPosition = rollStartPosition + rollDirection * rollDistance;
        Vector3 nextPosition = Vector3.Lerp(rollStartPosition, targetPosition, rollProgress);
        Vector3 moveVector = nextPosition - transform.position;

        characterController.Move(moveVector);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _aimInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isSprinting = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isSprinting = false;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsActionHappening)
            {
                StartRoll();
                isRolling = true;
            }
        }
    }
}