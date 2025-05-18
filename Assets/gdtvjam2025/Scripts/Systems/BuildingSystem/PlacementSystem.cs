using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO databaseSO;
    private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;

    private GridData floorData, structureData;

    private List<GameObject> placedGameObjects = new List<GameObject>();

    [SerializeField] private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private void Start()
    {
        StopPlacement();

        floorData = new GridData();
        structureData = new GridData();
    }

    private void Update()
    {
        DisplayGridPlacementVisualization();
    }

    private void DisplayGridPlacementVisualization()
    {
        if (selectedObjectIndex < 0)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            mouseIndicator.transform.position = mousePosition;

            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);

            lastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();

        selectedObjectIndex = databaseSO.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        gridVisualization.SetActive(true);
        
        previewSystem.StartShowingPlacementPreview(databaseSO.objectsData[selectedObjectIndex].prefab, 
                                                   databaseSO.objectsData[selectedObjectIndex].Size);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            Debug.LogError($"Invalid placement at {gridPosition}");
            return;
        }

        GameObject newObject = Instantiate(databaseSO.objectsData[selectedObjectIndex].prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);

        placedGameObjects.Add(newObject);
        GridData selectedData = databaseSO.objectsData[selectedObjectIndex].ID == 0 ? floorData : structureData;

        selectedData.AddObjectAt(gridPosition, 
                                 databaseSO.objectsData[selectedObjectIndex].Size, 
                                 databaseSO.objectsData[selectedObjectIndex].ID,
                                 placedGameObjects.Count - 1);


        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = databaseSO.objectsData[selectedObjectIndex].ID == 0 ? floorData : structureData;

        return selectedData.CanPlaceObjectAt(gridPosition, databaseSO.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;

        gridVisualization.SetActive(false);

        previewSystem.StopShowingPlacementPreview();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
    }


}
