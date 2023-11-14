using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : HealthManager
{
    [SerializeField] private UIHealthBar uiHealthBar;

    protected override void Awake()
    {
        MaxHealth = 100; // Set the max health for the player
        base.Awake();
    }

    private void Start()
    {
        OnHealthUpdated += uiHealthBar.SetHealth; // Subscribe to the event
        ResetHealth(); // To update the UI at the start of the game
    }

    protected override void OnDamageTaken(int amount)
    {
        base.TakeDamage(amount);
        // Update player's health bar or other UI elements
        UpdateHealthUI();
    }

    protected override void OnHealthChanged()
    {
        // Update player's health bar or other UI elements
        UpdateHealthUI();
    }

    public override void Die()
    {
        // Handle player death (e.g., respawn, game over screen, etc.)
    }

    private void UpdateHealthUI()
    {
        // Code to update the UI
    }
}
