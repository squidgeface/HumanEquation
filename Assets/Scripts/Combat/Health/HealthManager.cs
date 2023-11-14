using UnityEngine;
using UnityEngine.Events;

public abstract class HealthManager : MonoBehaviour
{
    public int CurrentHealth { get; protected set; }
    public int MaxHealth { get; protected set; }

    public delegate void HealthChanged(int currentHealth, int maxHealth);
    public event HealthChanged OnHealthUpdated;

    protected virtual void OnHealthChanged()
    {
        // Notify any listeners that the health has changed
        OnHealthUpdated?.Invoke(CurrentHealth, MaxHealth);
    }

    protected virtual void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            OnDamageTaken(amount);
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthChanged();
    }

    protected abstract void OnDamageTaken(int amount);
    public abstract void Die();

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        OnHealthChanged();
    }
}
