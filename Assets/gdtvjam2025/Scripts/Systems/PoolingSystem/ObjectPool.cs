using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public int poolSize = 50;
    public GameObject objectPrefab;

    [Header("DEBUGGING")]
    public int currentPoolSize = 0;

    private Queue<GameObject> OobjectPoolQueue = new Queue<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolObject = Instantiate(objectPrefab);

            OnPoolAwake(poolObject);

            poolObject.SetActive(false);

            OobjectPoolQueue.Enqueue(poolObject);
        }
    }

    private void Update()
    {
        currentPoolSize = OobjectPoolQueue.Count;
    }

    /// <summary>
    /// Override this function if you want to perform any setup after the pool is created but before any objects are spawned or returned.
    /// </summary>
    public virtual void OnPoolAwake(GameObject poolObject)
    {
        // This method can be overridden in derived classes to perform additional setup after the pool is created
        // but before any objects are spawned or returned.
    }

    public GameObject GetObject()
    {
        if (OobjectPoolQueue.Count > 0)
        {
            GameObject poolObject = OobjectPoolQueue.Dequeue();
            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab);
            return newPoolObject;
        }
    }

    /// <summary>
    ///  This method can be overridden in derived classes to perform additional setup after an object is spawned
    ///  but before it is returned to the pool.
    /// </summary>
    /// <param name="poolObject"></param>
    public virtual void OnObjectSpawned(GameObject poolObject)
    {

    }

    public GameObject SpawnObjectAt(Vector3 position)
    {
        if (OobjectPoolQueue.Count > 0)
        {
            GameObject poolObject = OobjectPoolQueue.Dequeue();

            poolObject.transform.position = position;

            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab, position, Quaternion.identity);

            newPoolObject.transform.position = position;

            return newPoolObject;
        }
    }

    public GameObject SpawnObjectAt(Vector3 position, Quaternion rotation)
    {
        if (OobjectPoolQueue.Count > 0)
        {
            GameObject poolObject = OobjectPoolQueue.Dequeue();

            poolObject.transform.position = position;
            poolObject.transform.rotation = rotation;

            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab, position, rotation);

            newPoolObject.transform.position = position;
            newPoolObject.transform.rotation = rotation;

            return newPoolObject;
        }
    }

    /// <summary>
    /// Override this function if you want to perform any changes before the object is returned.
    /// </summary>
    /// <param name="poolObject"></param>
    public virtual void OnObjectReturned(GameObject poolObject)
    {
        // This method can be overridden in derived classes to perform additional setup after an object is returned to the pool.
        // For example, you might want to reset the object's state or remove any references to it.
    }

    public void ReturnObject(GameObject poolObject)
    {
        OnObjectReturned(poolObject);
        poolObject.SetActive(false);
        OobjectPoolQueue.Enqueue(poolObject);
    }
}