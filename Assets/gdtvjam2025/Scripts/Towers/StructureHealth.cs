using UnityEngine;
using UnityEngine.Events;

public class StructureHealth : MonoBehaviour
{
    [Header("Settings")]
    public EStructureType structureType; // Type of the structure (e.g., Wall, Tower, etc.)
    public GameObject mainObject;

    [field: SerializeField]
    public float maxHealth { get; private set; } = 100; // Maximum health of the object
    public float currentHealth { get; private set; } // Current health of the object

    [Header("Events")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnHeal;
    public UnityEvent OnDeath;

    private StructureManager structureManager; // Reference to the StructureManager for managing structures

    private void Awake()
    {
        structureManager = FindFirstObjectByType<StructureManager>(); // Find the StructureManager in the scene

        if (structureManager == null)
        {
            Debug.LogError("StructureManager not found in the scene. Please ensure it is present.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth; // Reset current health to maximum health
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth; // Set a new maximum health
        ResetHealth(); // Reset current health to the new maximum
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

            structureManager.OnStructureRemoved(mainObject, structureType);
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
