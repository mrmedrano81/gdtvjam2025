using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public StructureManager structureManager;
    public GridData structureData;

   [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

    private void Awake()
    {
        structureManager = FindFirstObjectByType<StructureManager>();
    }

    public int PlaceObject(GameObject prefab, EStructureType structureType, Vector3 position, Vector3Int gridPosition) 
    {
        GameObject newObject = Instantiate(prefab);

        newObject.transform.position = position;

        placedGameObjects.Add(newObject);

        structureManager.SetupStructure(newObject, structureType, structureData, position, gridPosition);

        return placedGameObjects.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex, EStructureType structureType)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }

        GameObject structureObjectToBeRemoved = placedGameObjects[gameObjectIndex];

        structureManager.OnStructureRemoved(structureObjectToBeRemoved, structureType);

        Destroy(placedGameObjects[gameObjectIndex]);

        placedGameObjects[gameObjectIndex] = null;
    }

    public GameObject GetObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return null;
        }

        return placedGameObjects[gameObjectIndex];
    }
}
