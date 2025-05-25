using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StructureHealth))]
public class UIHealthBarStructure : MonoBehaviour
{

    [Header("Settings")]
    public Image healthBarSprite;
    public GameObject healthBarObject;
    public StructureHealth health;
    float healthPercentage = 1f;

    private void Awake()
    {
        health = GetComponent<StructureHealth>();

        health.OnDamageTaken.AddListener(UpdateHealthbar);
    }

    private void Update()
    {
        if (healthPercentage < 1)
        {
            healthBarObject.SetActive(true); // Hide the health bar if health is full
        }
        else
        {
            healthBarObject.SetActive(false); // Hide the health bar if health is full
        }
    }

    public void UpdateHealthbar()
    {
        healthPercentage = health.currentHealth / health.maxHealth;

        healthBarSprite.fillAmount = Mathf.Clamp(health.currentHealth / health.maxHealth, 0, 1);
    }

}
