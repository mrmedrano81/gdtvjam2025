using System.Collections.Generic;
using UnityEngine;

public enum EWallPlacementStatus
{
    Placing,
    Removing
}

public class WallScript : MonoBehaviour
{
    [Header("Wall Settings")]
    public float health = 100f;
    [SerializeField] private EStructureType structureType;

    [Header("References")]
    public GameObject MiddleWall;
    public GameObject TopWallConnector;
    public GameObject BottomWallConnector;
    public GameObject RightWallConnector;
    public GameObject LeftWallConnector;

    [HideInInspector] public GridData structureData;
    [HideInInspector] public Vector3Int gridPosition;

    private ObjectPlacer objectPlacer;

    [Header("Debugging")]
    public GameObject topWall;
    public GameObject rightWall;
    public GameObject leftWall;
    public GameObject bottomWall;

    private Dictionary<int, GameObject> adjacentWallDictionary = new();


    private void Awake()
    {
        objectPlacer = FindFirstObjectByType<ObjectPlacer>();
    }

    private void Start()
    {

    }

    public void SetupWall(EWallPlacementStatus wallPlacementStatus)
    {
        ResetWall();
        UpdateAdjacentWalls(wallPlacementStatus);
        UpdateWallConnectors();
    }

    private void ResetWall()
    {
        topWall = null;
        rightWall = null;
        leftWall = null;
        bottomWall = null;

        adjacentWallDictionary.Clear();

        adjacentWallDictionary.Add(0, null);
        adjacentWallDictionary.Add(1, null);
        adjacentWallDictionary.Add(2, null);
        adjacentWallDictionary.Add(3, null);
    }

    private void UpdateWallConnectors()
    {
        foreach (var key in adjacentWallDictionary.Keys)
        {
            GameObject adjacentWall = adjacentWallDictionary[key];

            if (adjacentWall != null)
            {
                switch (key)
                {
                    case 1: BottomWallConnector.SetActive(true); break; // Bottom
                    case 0: TopWallConnector.SetActive(true); break;    // Top
                    case 3: RightWallConnector.SetActive(true); break;  // Right
                    case 2: LeftWallConnector.SetActive(true); break;   // Left
                }
            }
            else
            {
                switch (key)
                {
                    case 1: BottomWallConnector.SetActive(false); break; // Bottom
                    case 0: TopWallConnector.SetActive(false); break;    // Top
                    case 3: RightWallConnector.SetActive(false); break;  // Right
                    case 2: LeftWallConnector.SetActive(false); break;   // Left
                }
            }
        }
    }

    private void UpdateAdjacentWalls(EWallPlacementStatus status)
    {
        Vector3Int direction = Vector3Int.zero;
        for (int i = 0; i < 4; i++)
        {
            int adjWallIndex = -1;
            switch (i)
            {
                case 0:
                    adjWallIndex = 1; // Top
                    direction = Vector3Int.forward; 
                    break;    // Top
                case 1:
                    adjWallIndex = 0; // Bottom
                    direction = Vector3Int.back; 
                    break;  // Bottom
                case 2:
                    adjWallIndex = 3; // Left
                    direction = Vector3Int.left; 
                    break;  // Left
                case 3: 
                    adjWallIndex = 2; // Right
                    direction = Vector3Int.right; 
                    break; // Right
            }

            if (adjWallIndex == -1)
            {
                Debug.LogError("Invalid wall index");
                return;
            }

            Vector3Int adjacentPosition = gridPosition + direction;

            PlacementData adjacentData = structureData.GetPlacementDataOfObjectAt(adjacentPosition);

            //Debug.Log($"Adjacent wall at {adjacentPosition} with index {adjacentData.placedObjectIndex} in direction {i}.");

            if (adjacentData != null)
            {
                WallScript adjacentWall = objectPlacer.GetObjectAt(adjacentData.placedObjectIndex).GetComponent<WallScript>();

                if (adjacentWall != null)
                {
                    if (status == EWallPlacementStatus.Placing)
                    {
                        // Store the adjacent wall if it is not already stored
                        if (adjacentWallDictionary[i] == null)
                        {
                            adjacentWallDictionary[i] = adjacentWall.gameObject;
                        }
                        else
                        {
                            Debug.LogError($"Adjacent wall at {i} is already stored in this wall.");
                        }

                        // Store this wall as an adjacent wall for the current adjacent wall if it is not already stored
                        if (adjacentWall.adjacentWallDictionary[adjWallIndex] == null)
                        {
                            adjacentWall.adjacentWallDictionary[adjWallIndex] = gameObject;
                        }
                        else
                        {
                            Debug.LogError($"This wall is already stored at at {i} of the adjacent wall.");
                        }

                    }

                    if (status == EWallPlacementStatus.Removing)
                    {
                        // Store the adjacent wall if it is not already stored
                        if (adjacentWallDictionary[i] != null)
                        {
                            adjacentWallDictionary[i] = null;
                        }
                        // Store this wall as an adjacent wall for the current adjacent wall if it is not already stored
                        if (adjacentWall.adjacentWallDictionary[adjWallIndex] != null)
                        {
                            adjacentWall.adjacentWallDictionary[adjWallIndex] = null;
                        }
                    }

                    adjacentWall.UpdateWallConnectors();
                }
            }
            else
            {
                Debug.Log($"No adjacent wall at {i}");
            }
        }
    }



    private void OnDrawGizmos()
    {

    }
}
