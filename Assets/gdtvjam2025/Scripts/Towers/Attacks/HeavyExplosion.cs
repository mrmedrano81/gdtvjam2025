using System.Collections;
using UnityEngine;

public class HeavyAttack : MonoBehaviour
{
    public float damage;
    public float duration = 0.05f;


    private DestroyScript destroyScript;

    private void Awake()
    {
        destroyScript = GetComponentInParent<DestroyScript>();
    }

    private void OnEnable()
    {
        destroyScript.DestroyAfter(duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health health = other.GetComponentInParent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
                // Optionally, you can destroy the explosion object after dealing damage
            }
        }
    }
}
