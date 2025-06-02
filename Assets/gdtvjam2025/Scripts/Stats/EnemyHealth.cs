using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]

    [field: SerializeField]
    public float maxHealth { get; set; } = 100; // Maximum health of the object
    public float currentHealth { get; private set; } // Current health of the object

    [Header("Events")]
    public UnityEvent OnDamageTaken; 
    public UnityEvent OnHeal;
    public UnityEvent OnDeath;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth; // Reset current health to maximum health
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            return; // If current health is already zero or less, do not process further damage
        }

        currentHealth -= damage; // Subtract damage from current health

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        else
        {
            OnDamageTaken?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount; // Add healing amount to current health

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Ensure current health does not exceed maximum health
        }
        else
        {
            OnHeal?.Invoke();
        }
    }
}
