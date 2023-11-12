using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler
{
    private PlayerControls playerControls;
    private Camera mainCamera;
    private bool isGamepadActive;

    public InputHandler()
    {
        playerControls = new PlayerControls();
        mainCamera = Camera.main; // Cache the main camera
        playerControls.Player.Aim.performed += ctx => isGamepadActive = ctx.control.device.ToString().Contains("Gamepad");
    }

    public void Enable()
    {
        playerControls.Player.Enable();
    }

    public void Disable()
    {
        playerControls.Player.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetPlayerAim()
    {
        return playerControls.Player.Aim.ReadValue<Vector2>();
    }

    public Vector3 GetAimDirection(Transform playerTransform)
    {
        if (isGamepadActive) // Gamepad aiming
        {
            Vector2 aimInput = playerControls.Player.Aim.ReadValue<Vector2>();
            if (aimInput.sqrMagnitude < 0.1f)
                return playerTransform.forward; // If there's little input, maintain current direction.
            return new Vector3(aimInput.x, 0f, aimInput.y);
        }
        else // Mouse aiming
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, playerTransform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                return (point - playerTransform.position).normalized;
            }
            return playerTransform.forward;
        }
    }

    public bool GetJumpInput()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool GetDodgeInput()
    {
        return playerControls.Player.Dodge.triggered;
    }

    public bool GetFireInput()
    {
        return playerControls.Player.Shoot.ReadValue<float>() > 0.1f;
    }
}
