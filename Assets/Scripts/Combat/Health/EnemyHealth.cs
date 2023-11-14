using UnityEngine;

public class EnemyHealth : HealthManager
{
    private Enemy enemyObject;

    private void Start()
    {
        enemyObject = GetComponent<Enemy>();
        MaxHealth = enemyObject.EnemyData.maxHealth; // Set the max health for the enemy
    }

    protected override void OnDamageTaken(int amount)
    {
        ShowHealthBar();
    }

    private void ShowHealthBar()
    {
        // Logic to show and update the health bar for the enemy
        // This could be a floating health bar above the enemy or a UI element on the screen
    }

    public override void Die()
    {
        enemyObject.Deactivate();
    }
}
