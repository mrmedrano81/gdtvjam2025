using UnityEngine;


public class EnemyGruntPool : ObjectPool
{
    public override void OnPoolAwake(GameObject poolObject)
    {
        poolObject.GetComponent<EnemyAIController>().enemyPool = this;
    }

    public override void OnObjectReturned(GameObject poolObject)
    {
        Health health = poolObject.GetComponent<Health>();

        if (health != null)
        {
            health.ResetHealth();
        }
        else
        {
            Debug.LogWarning($"Object {poolObject.name} does not have a Health component.");
        }
    }

    public override void OnObjectSpawned(GameObject poolObject)
    {
        Health health = poolObject.GetComponent<Health>();

        if (health != null)
        {
            health.ResetHealth();
        }
        else
        {
            Debug.LogWarning($"Object {poolObject.name} does not have a Health component.");
        }
    }

}
