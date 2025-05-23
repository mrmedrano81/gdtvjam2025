using UnityEngine;


public class EnemyGruntObjectPool : ObjectPool
{
    public override void OnNewObjectCreated(GameObject poolObject)
    {
        poolObject.GetComponent<EnemyAIController>().enemyPool = this;
        poolObject.GetComponent<Health>().ResetHealth();
        poolObject.GetComponent<UIHealthBar>().UpdateHealthbar();

        //set map center point
        poolObject.GetComponent<EnemyAIController>().mapCenterPoint = GameObject.FindGameObjectWithTag("HQ").transform;

        currentPoolSize++;
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

        poolObject.GetComponent<EnemyAIController>().Initialize();
        poolObject.GetComponent<UIHealthBar>().UpdateHealthbar();
    }

    public override void OnObjectGet(GameObject poolObject)
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
