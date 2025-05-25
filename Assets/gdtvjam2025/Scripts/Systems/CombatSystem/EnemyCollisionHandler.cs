using UnityEngine;

public class EnemyCollisionHandler : CollisionHandler
{
    private EnemyHealth health;
    private EnemyHitEffectPool enemyHitEffectPool;

    private void Awake()
    {
        health = GetComponentInParent<EnemyHealth>();
        enemyHitEffectPool = FindFirstObjectByType<EnemyHitEffectPool>();
    }

    public override void HandleOnTriggerEnter(Collider other)
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (health != null)
        {
            health.TakeDamage(damage);

            //enemyHitEffectPool.SpawnObjectAt(transform.position);

        }
    }

    public void PlayHitEffectAt(Vector3 position)
    {
        // Play the effect at the specified position
        enemyHitEffectPool.SpawnObjectAt(position, Quaternion.identity);
    }
}
