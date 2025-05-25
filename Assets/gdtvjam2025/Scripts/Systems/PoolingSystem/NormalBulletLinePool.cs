using UnityEngine;

public class NormalBulletLinePool : ObjectPool
{
    public override void OnNewObjectCreated(GameObject poolObject)
    {
        poolObject.GetComponent<PoolObjectHandler>().objectPool = this;
    }

    public override void OnObjectGet(GameObject poolObject)
    {
        poolObject.GetComponent<PoolObjectHandler>().OnSpawnObject();
    }
}
