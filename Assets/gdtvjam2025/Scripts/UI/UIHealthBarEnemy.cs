using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyHealth))]
public class UIHealthBarEnemy : MonoBehaviour
{
    [Header("Settings")]
    public Image healthBarSprite;
    public EnemyHealth health;
    public GameObject healthBarObject;
    private float healthPercentage = 1f;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();

        health.OnDamageTaken.AddListener(UpdateHealthbar);
    }

    private void Update()
    {
        if (healthPercentage < 1f)
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
