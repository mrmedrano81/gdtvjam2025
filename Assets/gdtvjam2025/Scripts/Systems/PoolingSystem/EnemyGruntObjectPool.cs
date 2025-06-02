using UnityEngine;


public class EnemyGruntObjectPool : ObjectPool
{
    public EnemyGruntDeadPool enemyGruntDeadPool;

    public override void OnNewObjectCreated(GameObject poolObject)
    {
        poolObject.GetComponent<EnemyAIController>().enemyGruntPool = this;

        poolObject.GetComponent<EnemyAIController>().enemyGruntDeadPool = enemyGruntDeadPool;

        poolObject.GetComponent<EnemyAIController>().OnObjectGet();

        //set map center point
        currentPoolSize++;
    }

    public override void OnObjectReturned(GameObject poolObject)
    {
        poolObject.GetComponent<EnemyAIController>().OnObjectReturned();
    }

    public override void OnObjectGet(GameObject poolObject)
    {
        if (GameState.Instance.hardMode)
        {
            poolObject.GetComponent<EnemyHealth>().maxHealth = 350f;
            poolObject.GetComponent<EnemyAIController>().damage = 4f;
        }
        else
        {
            poolObject.GetComponent<EnemyHealth>().maxHealth = 150f;
            poolObject.GetComponent<EnemyAIController>().damage = 2f;
        }

        poolObject.GetComponent<EnemyAIController>().OnObjectGet();
    }

}
