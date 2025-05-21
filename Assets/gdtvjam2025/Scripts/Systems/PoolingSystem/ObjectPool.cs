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
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolObject = Instantiate(objectPrefab);

            OnCreatePool(poolObject);

            poolObject.SetActive(false);

            OobjectPoolQueue.Enqueue(poolObject);
        }
    }

    private void Update()
    {
        currentPoolSize = OobjectPoolQueue.Count;
    }

    public GameObject GetObject()
    {
        if (OobjectPoolQueue.Count > 0)
        {
            GameObject poolObject = OobjectPoolQueue.Dequeue();

            OnObjectGet(poolObject);

            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab);

            OnNewObjectCreated(newPoolObject);

            OnObjectGet(newPoolObject);

            return newPoolObject;
        }
    }

    public GameObject SpawnObjectAt(Vector3 position)
    {
        if (OobjectPoolQueue.Count > 0)
        {
            GameObject poolObject = OobjectPoolQueue.Dequeue();

            poolObject.transform.position = position;

            OnObjectGet(poolObject);

            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab, position, Quaternion.identity);

            OnObjectGet(newPoolObject);

            OnNewObjectCreated(newPoolObject);

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

            OnObjectGet(poolObject);

            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab, position, rotation);

            OnNewObjectCreated(newPoolObject);

            newPoolObject.transform.position = position;
            newPoolObject.transform.rotation = rotation;

            OnObjectGet(newPoolObject);

            return newPoolObject;
        }
    }

    public GameObject SpawnObjectAt(Vector3 position, Quaternion rotation, Transform parentTransform)
    {
        if (OobjectPoolQueue.Count > 0)
        {
            GameObject poolObject = OobjectPoolQueue.Dequeue();

            poolObject.transform.position = position;
            poolObject.transform.rotation = rotation;

            OnObjectGet(poolObject);

            poolObject.transform.SetParent(parentTransform, false);

            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = Instantiate(objectPrefab, position, rotation);

            OnNewObjectCreated(newPoolObject);

            newPoolObject.transform.position = position;
            newPoolObject.transform.rotation = rotation;

            OnObjectGet(newPoolObject);

            newPoolObject.transform.SetParent(parentTransform, false);

            return newPoolObject;
        }
    }

    public void ReturnObject(GameObject poolObject)
    {
        OnObjectReturned(poolObject);

        poolObject.SetActive(false);
        OobjectPoolQueue.Enqueue(poolObject);
    }


    /// <summary>
    /// Override this function if you want to perform any setup after the pool is created but before any objects are spawned or returned.
    /// </summary>
    public virtual void OnCreatePool(GameObject poolObject)
    {
        OnNewObjectCreated(poolObject);

        // This method can be overridden in derived classes to perform additional setup after the pool is created
        // but before any objects are spawned or returned.
    }

    /// <summary>
    ///  This method can be overridden in derived classes to perform additional setup after an object is spawned
    ///  but before it is returned to the pool.
    /// </summary>
    /// <param name="poolObject"></param>
    public virtual void OnObjectGet(GameObject poolObject)
    {

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

    /// <summary>
    /// This method can be overridden in derived classes to perform additional setup after a new object is created.
    /// For example, you might want to set up references or initialize the object's state.
    /// </summary>
    /// <param name="poolObject"></param>
    public virtual void OnNewObjectCreated(GameObject poolObject)
    {

    }
}