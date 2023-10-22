using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TwinStickMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform aimTransform;

    private Vector2 moveInput;
    private Vector2 aimInput;

    void Update()
    {
        // Movement
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Rotation
        if (aimInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimInput.x, aimInput.y) * Mathf.Rad2Deg;
            aimTransform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
    }
}
