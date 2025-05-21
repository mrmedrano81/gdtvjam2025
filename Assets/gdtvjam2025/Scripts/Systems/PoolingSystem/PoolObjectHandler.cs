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
        Invoke("ReturnToPool", lifeTime);
    }

    private void ReturnToPool()
    {
        //Debug.Log(gameObject.name);

        gameObject.transform.SetParent(objectPool.transform); // Set the parent to the pool's transform

        objectPool.ReturnObject(gameObject);
    }
}
