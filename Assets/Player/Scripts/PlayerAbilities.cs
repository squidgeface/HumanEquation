using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    public float rollDistance = 5f;  // Distance to roll
    public float rollDuration = 0.5f;  // Duration of the roll
    public float rollCooldown = 2f;  // Cooldown between rolls

    private float lastRollTime;
    private Vector3 rollDirection;
    private bool isRolling;

    void Update()
    {
        if (!isRolling && Time.time >= lastRollTime + rollCooldown)
        {
            StartRoll();
        }

        if (isRolling)
        {
            Roll();
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollDirection = transform.forward;  // Set the roll direction (customize this to your needs)
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

        Vector3 targetPosition = transform.position + rollDirection * rollDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, rollProgress);
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRolling = true;
    }
}
