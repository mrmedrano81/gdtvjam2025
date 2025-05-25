using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyHealth))]
public class UIHealthBar : MonoBehaviour
{
    [Header("Settings")]
    public Image healthBarSprite;
    public EnemyHealth health;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();

        health.OnDamageTaken.AddListener(UpdateHealthbar);
    }

    public void UpdateHealthbar()
    {
        healthBarSprite.fillAmount = Mathf.Clamp(health.currentHealth/health.maxHealth, 0, 1);
    }
}
