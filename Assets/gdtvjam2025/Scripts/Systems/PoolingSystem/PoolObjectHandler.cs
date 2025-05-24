using UnityEngine;

/// <summary>
/// A component that handles the returning of a pooled object to the pool after a condition or set of conditions.
/// </summary>
public class PoolObjectHandler : MonoBehaviour
{
    public ObjectPool objectPool;
    public float lifeTime = 1f;
    public bool returnAfterTime = true;

    private void OnEnable()
    {
        OnObjectSpawned();

        if (objectPool != null)
        {
            Invoke("ReturnToPool", lifeTime);
        }
    }

    private void ReturnToPool()
    {
        //Debug.Log(gameObject.name);

        gameObject.transform.SetParent(objectPool.transform); // Set the parent to the pool's transform

        OnReturnToPool();

        objectPool.ReturnObject(gameObject);
    }

    public virtual void OnReturnToPool()
    {
        // This method can be overridden in derived classes to perform additional actions when returning to the pool.
        // For example, resetting properties or states of the object.
    }

    public virtual void OnObjectSpawned()
    {
        // This method can be overridden in derived classes to perform actions when the object is spawned from the pool.
        // For example, initializing properties or states of the object.
    }
}
