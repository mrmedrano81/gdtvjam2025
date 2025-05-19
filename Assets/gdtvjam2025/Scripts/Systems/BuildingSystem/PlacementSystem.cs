using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private GridData floorData, structureData;

    IBuildingState buildingState;

    private void Awake()
    {
        eventManager = FindFirstObjectByType<EventManager>();
    }

    private void Start()
    {
        StopPlacement();

        floorData = new GridData();
        structureData = new GridData();
    }

    private void Update()
    {
        UpdatePlacementSystem();
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
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID,
                                           grid,
                                           previewSystem,
                                           databaseSO,
                                           floorData,
                                           structureData,
                                           objectPlacer,
                                           eventManager);

        inputManager.OnClicked += PlaceStructure;
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

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        gridVisualization.SetActive(false);

        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }


}
