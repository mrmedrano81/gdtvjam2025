using UnityEngine;

public class EnemyCollisionHandler : CollisionHandler
{
    private Health health;

    private void Awake()
    {
        health = GetComponentInParent<Health>();    
    }

    public override void HandleOnTriggerEnter(Collider other)
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
