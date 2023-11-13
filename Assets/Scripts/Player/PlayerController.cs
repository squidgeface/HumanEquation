using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Vector3 lastMovementDirection;

    [Header("Jump Detection")]
    [SerializeField] private LayerMask ledgeLayerMask;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private Vector3 jumpTarget;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 15f;
    [SerializeField] private float dodgeDuration = 0.2f;

    [Header("Physics")]
    [SerializeField] private float gravityValue = -9.81f;

    [Header("Wall Running")]
    [SerializeField] private float maxWallRunTime = 5f; // Maximum duration of wall run
    [SerializeField] private float wallRunSpeed = 6f; // Speed of the wall run
    [SerializeField] private float wallRunJumpForce = 5f; // Force of the jump off the wall
    private bool isWallRunning = false;
    private Transform currentWallRunTarget;
    private Vector3 wallRunDirection; // The direction along the wall
    private float wallRunStartTime;

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

        if (!isJumpingToTarget && !isDodging && !isWallRunning)
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

        if ((int)inputHandler.GetSwapWeaponInput() != 0)
            weaponManager.SwitchGun((int)inputHandler.GetSwapWeaponInput());

        if (inputHandler.GetJumpInput() && currentWallRunTarget != null)
        {
            if (!isWallRunning)
            {
                StartWallRun();
            }
            else
            {
                JumpOffWall();
            }
        }


        if (isWallRunning)
        {
            PerformWallRun();
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
        lastMovementDirection = move;

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
        EndWallRun();
    }


    private void StartWallRun()
    {
        isWallRunning = true;
        wallRunStartTime = Time.time;

        // Optional: Adjust player's gravity or apply a constant force to stick to the wall
        // For example, set gravity to 0 or a lower value
        playerVelocity.y = 0; // Reset vertical velocity
        // ... (More code as needed for your specific game mechanics)
    }

    // Call this in Update when isWallRunning is true
    private void PerformWallRun()
    {
        if (Time.time - wallRunStartTime > maxWallRunTime)
        {
            EndWallRun();
            return;
        }

        // Move the player along the wall run direction with a constant speed
        Vector3 wallRunMovement = wallRunDirection * wallRunSpeed * Time.deltaTime;
        // Apply only horizontal movement
        wallRunMovement.y = 0;
        controller.Move(wallRunMovement);

        // Optional: Apply a slight force or modify gravity to simulate wall running better
        // ... (More code as needed for your specific game mechanics)
    }

    // Call this when the player jumps off the wall
    private void JumpOffWall()
    {
        // Calculate force direction, which is a combination of away from the wall and upward
        Vector3 jumpOffForce = currentWallRunTarget.forward.normalized * wallRunJumpForce;

        // Apply the jump off force to the player's velocity
        // Initiate the parabolic jump
        StartCoroutine(ParabolicJump(jumpOffForce, wallRunJumpForce));
    }

    // Call this to end wall run due to various conditions
    private void EndWallRun()
    {
        isWallRunning = false;
        playerVelocity = Vector3.zero;
        // Optional: Reset player's gravity if it was changed during wall run
        // For example, set gravity back to its original value
        // ... (More code as needed for your specific game mechanics)
    }

    private void DetermineWallRunDirection(Transform wallTransform)
    {
        // Assuming the wallTransform's forward vector is the normal of the wall
        Vector3 wallNormal = wallTransform.forward;

        // Use the player's last movement direction to determine which way to run on the wall
        wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
        if (Vector3.Dot(lastMovementDirection, wallNormal) > 0)
        {
            // If the player's movement direction is towards the wall, change wall run direction to the opposite side
            wallRunDirection = -wallRunDirection;
        }
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
        if (other.CompareTag("WallRunTrigger"))
        {
            currentWallRunTarget = other.transform; // Assuming the trigger has a transform that indicates the run direction
            DetermineWallRunDirection(currentWallRunTarget);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            jumpTarget = Vector3.zero;
        }
        if (other.CompareTag("WallRunTrigger"))
        {
            currentWallRunTarget = null;
            wallRunDirection = Vector3.zero;
            isWallRunning = false;
        }
    }


}
