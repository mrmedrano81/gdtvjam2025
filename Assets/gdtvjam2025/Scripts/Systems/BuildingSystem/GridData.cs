using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();
    public Vector2 gridSize = new Vector2(30, 30);

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, EStructureType structureType, int placedObjectIndex)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);

        PlacementData data = new PlacementData(positionsToOccupy, structureType, placedObjectIndex);

        foreach (Vector3Int position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                //throw new Exception($"Position {position} is already occupied by ID {placedObjects[position].ID}");
                throw new Exception($"Position {position} is already occupied by {placedObjects[position].structureType}");
            }

            placedObjects[position] = data;
        }
    }

    //public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    //{
    //    List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);

    //    PlacementData data = new PlacementData(positionsToOccupy, ID, placedObjectIndex);

    //    foreach (Vector3Int position in positionsToOccupy)
    //    {
    //        if (placedObjects.ContainsKey(position))
    //        {
    //            throw new Exception($"Position {position} is already occupied by ID {placedObjects[position].ID}");
    //        }

    //        placedObjects[position] = data;
    //    }
    //}

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();

        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                Vector3Int position = new Vector3Int(gridPosition.x + x, gridPosition.y, gridPosition.z + y);
                returnVal.Add(position);
            }
        }


        //int halfObjectSizeX = Mathf.RoundToInt(objectSize.x / 2);
        //int halfObjectSizeY = Mathf.RoundToInt(objectSize.y / 2);

        ////if (objectSize.x % 2 != 0)
        ////{
        ////    halfObjectSizeX = Mathf.RoundToInt((objectSize.x +1)/ 2);
        ////}

        ////if (objectSize.y % 2 != 0)
        ////{
        ////    halfObjectSizeY = Mathf.RoundToInt((objectSize.y +1)/ 2);
        ////}

        //for (int x = -halfObjectSizeX; x < halfObjectSizeX; x++)
        //{
        //    for (int y = -halfObjectSizeY; y < halfObjectSizeY; y++)
        //    {
        //        Vector3Int position = new Vector3Int(gridPosition.x + x, gridPosition.y, gridPosition.z + y) + Vector3Int.one;
        //        returnVal.Add(position);
        //    }
        //}

        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);

        foreach (Vector3Int position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                return false;
            }
        }

        // check if the object fits within the grid size
        if (gridPosition.x + objectSize.x > gridSize.x || gridPosition.z + objectSize.y > gridSize.y)
        {
            return false;
        }

        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }

        return placedObjects[gridPosition].placedObjectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    public PlacementData GetPlacementDataOfObjectAt(Vector3Int gridPosition)
    {
        PlacementData returnVal = null;
        if (placedObjects.ContainsKey(gridPosition))
        {
            returnVal = placedObjects[gridPosition];

            Debug.Log($"Found object at {gridPosition} with ID {returnVal.occupiedPositions}");
        }

        return returnVal;
    }

    public int GetPlacementIndex(Vector3Int gridPosition)
    {
        int returnVal = -1;

        if (placedObjects.ContainsKey(gridPosition))
        {
            returnVal = placedObjects[gridPosition].placedObjectIndex;

            Debug.Log($"Found object at {gridPosition} with ID {placedObjects[gridPosition].placedObjectIndex}");
        }

        return returnVal;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    //public int ID { get; private set; }
    public EStructureType structureType { get; private set; }
    public int placedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, EStructureType structureType, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.structureType = structureType;
        this.placedObjectIndex = placedObjectIndex;
    }
}