using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public StructureManager structureManager;
   [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

    private void Awake()
    {
        structureManager = FindFirstObjectByType<StructureManager>();
    }

    public int PlaceObject(GameObject prefab, EStructureType structureType, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);

        newObject.transform.position = position;

        structureManager.SetupStructure(newObject, structureType);

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
