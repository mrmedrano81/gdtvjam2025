using UnityEngine;

public class EnemyGruntDeadPool : ObjectPool
{
    public override void OnNewObjectCreated(GameObject poolObject)
    {
        poolObject.GetComponent<PoolObjectHandler>().objectPool = this;
    }

    public override void OnObjectGet(GameObject poolObject)
    {
        base.OnObjectGet(poolObject);

        poolObject.GetComponent<PoolObjectHandler>().SpawnObject();
    }

    public override void OnObjectReturned(GameObject poolObject)
    {
        base.OnObjectReturned(poolObject);
    }
}
