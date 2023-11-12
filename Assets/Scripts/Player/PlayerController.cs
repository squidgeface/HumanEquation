using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Jump Detection")]
    [SerializeField] private LayerMask ledgeLayerMask;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private Vector3 jumpTarget;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 15f;
    [SerializeField] private float dodgeDuration = 0.2f;

    [Header("Physics")]
    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController controller;
    private WeaponManager weaponManager;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool isJumpingToTarget = false;
    private bool isDodging = false;
    private float dodgeTimeCounter;
    private InputHandler inputHandler;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = new InputHandler();
        weaponManager = GetComponent<WeaponManager>();
    }

    private void OnEnable()
    {
        inputHandler.Enable();
    }

    private void OnDisable()
    {
        inputHandler.Disable();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (!isJumpingToTarget && !isDodging)
        {
            HandleMovement();
            HandleRotation();
            HandleDodge();

            if (inputHandler.GetJumpInput())
            {
                if (jumpTarget != Vector3.zero)
                    TryJump();
            }
        }

        if (inputHandler.GetFireInput())
        {
            weaponManager.TryFire();
        }

        if (isDodging)
        {
            DodgeMovement();
        }
    }

    private void HandleMovement()
    {
        Vector2 movement = inputHandler.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * moveSpeed);

        if (!isGrounded)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        Vector3 aimDirection = inputHandler.GetAimDirection(transform);
        if (aimDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleDodge()
    {
        if (inputHandler.GetDodgeInput() && isGrounded)
        {
            isDodging = true;
            dodgeTimeCounter = dodgeDuration;
        }
    }

    private void DodgeMovement()
    {
        Vector2 movementInput = inputHandler.GetPlayerMovement();
        Vector3 dodgeDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        if (dodgeTimeCounter > 0)
        {
            controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            dodgeTimeCounter -= Time.deltaTime;
        }
        else
        {
            isDodging = false;
        }
    }

    private void TryJump()
    {
        // Jump target
        var jumpPoint = jumpTarget;
        // Calculate jump duration based on the distance to target and a fixed speed
        float distanceToTarget = Vector3.Distance(transform.position, jumpPoint);
        float jumpDuration = distanceToTarget / jumpSpeed;

        // Initiate the parabolic jump
        StartCoroutine(ParabolicJump(jumpPoint, jumpDuration));

        // Set the flag that we are jumping to a target
        isJumpingToTarget = true;
    }

    private IEnumerator ParabolicJump(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, startPosition.y, targetPosition.z); // Ensure same y level
        float peakHeight = Mathf.Clamp(Vector3.Distance(startPosition, endPosition) / 4f, 1f, 10f);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;
            float height = Mathf.Sin(Mathf.PI * normalizedTime) * peakHeight;

            // Calculate the next position using only horizontal movement
            Vector3 nextHorizontalPosition = Vector3.Lerp(startPosition, endPosition, normalizedTime);
            nextHorizontalPosition.y = startPosition.y;

            // Calculate the vertical offset separately
            Vector3 verticalOffset = Vector3.up * height;

            // Determine the next position
            Vector3 nextPosition = nextHorizontalPosition + verticalOffset - transform.position;

            // Move the character controller
            controller.Move(nextPosition * Time.deltaTime / Time.deltaTime); // Divide by deltaTime to counteract the multiplication in controller.Move

            yield return null;
        }

        // Snap to the target position on the same y level to ensure precision
        controller.Move(new Vector3(targetPosition.x, startPosition.y, targetPosition.z) - transform.position);

        // The jump is complete, so we're no longer jumping to a target
        isJumpingToTarget = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            // The trigger's paired Transform could be stored in a script attached to the trigger
            JumpPoints trigger = other.GetComponentInParent<JumpPoints>();
            if (trigger != null)
            {
                jumpTarget = other.transform == trigger.JumpPoint1 ? trigger.JumpPoint2.transform.position : trigger.JumpPoint1.transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            jumpTarget = Vector3.zero;
        }
    }


}
