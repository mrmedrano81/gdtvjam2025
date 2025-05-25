using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EStructureType
{
    HQ,
    Gatherer,
    NormalTower,
    HeavyTower,
    MissileTower,
    Wall
}

public class PlacementSystem : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO databaseSO;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private EventManager eventManager;

    [Header("Settings")]
    public Vector2 gridSize = new Vector2(30, 30);

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    public GridData floorData, structureData;

    IBuildingState buildingState;

    public bool IsBuildingActive => buildingState != null;

    private void Awake()
    {
        eventManager = FindFirstObjectByType<EventManager>();
    }

    private void Start()
    {
        StopPlacement();

        floorData = new GridData();
        structureData = new GridData();

        floorData.gridSize = gridSize;
        structureData.gridSize = gridSize;

        objectPlacer.structureData = structureData;

        InitializeHQPlacement();
    }

    private void Update()
    {
        if (GameState.Instance.CurrentGameState == EGameState.Paused)
        {
            return;
        }
        if (GameState.Instance.CurrentGameState == EGameState.Combat)
        {
            StopPlacement();
            return;
        }

        UpdatePlacementSystem();
    }
    
    public void InitializeHQPlacement()
    {
        Vector3Int gridPosition = new Vector3Int(-2, 0, -2);

        int index = objectPlacer.PlaceObject(databaseSO.objectsData[0].prefab,
                                     databaseSO.objectsData[0].StructureType,
                                     grid.CellToWorld(gridPosition),
                                     gridPosition);

        //GridData selectedData = databaseSO.objectsData[selectedObjectIndex].ID == 0 ? floorData : structureData;

        structureData.AddObjectAt(gridPosition,
                                 databaseSO.objectsData[0].Size,
                                 databaseSO.objectsData[0].StructureType,
                                 index);
    }

    private void UpdatePlacementSystem()
    {
        if (buildingState == null)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacement(int ID)
    {
        if (GameState.Instance.CurrentGameState == EGameState.Paused || GameState.Instance.CurrentGameState == EGameState.Combat)
        {
            return;
        }

        objectPlacer.structureData = structureData;

        EStructureType structureType = (EStructureType)ID;

        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(structureType,
                                           grid,
                                           previewSystem,
                                           databaseSO,
                                           floorData,
                                           structureData,
                                           objectPlacer,
                                           eventManager);

        if (structureType == EStructureType.Wall)
        {
            inputManager.OnLeftClickHold += PlaceStructure;
        }
        else
        {
            inputManager.OnLeftClick += PlaceStructure;
        }

        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new RemovingState(grid,
                                          previewSystem,
                                          databaseSO,
                                          floorData,
                                          structureData,
                                          objectPlacer,
                                          eventManager);

        inputManager.OnLeftClickHold += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (GameState.Instance.CurrentGameState == EGameState.Paused
            || GameState.Instance.CurrentGameState == EGameState.Combat)
        {
            return;
        }

        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    public void StopPlacement()
    {
        if (buildingState == null)
            return;

        gridVisualization.SetActive(false);

        buildingState.EndState();

        inputManager.OnLeftClick -= PlaceStructure;
        inputManager.OnLeftClickHold -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }


}
