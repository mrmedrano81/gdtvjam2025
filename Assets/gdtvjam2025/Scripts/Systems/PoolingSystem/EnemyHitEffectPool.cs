using UnityEngine;

public class EnemyHitEffectPool : ObjectPool
{
    public override void OnNewObjectCreated(GameObject poolObject)
    {
        poolObject.GetComponent<PoolObjectHandler>().objectPool = this;
    }

    public override void OnObjectGet(GameObject poolObject)
    {

    }

    public override void OnObjectReturned(GameObject poolObject)
    {

    }
}
