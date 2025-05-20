using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class UIHealthBar : MonoBehaviour
{
    [Header("Settings")]
    public Image healthBarSprite;
    public Health health;

    private void Awake()
    {
        health = GetComponent<Health>();

        health.OnDamageTaken.AddListener(UpdateHealthbar);
    }

    public void UpdateHealthbar()
    {
        healthBarSprite.fillAmount = Mathf.Clamp(health.currentHealth/health.maxHealth, 0, 1);
    }
}
