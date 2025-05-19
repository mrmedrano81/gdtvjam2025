using UnityEngine;
using System.Collections.Generic;


public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO databaseSO;
    GridData floorData;
    GridData structureData;
    ObjectPlacer objectPlacer;
    EventManager eventManager;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO databaseSO,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          EventManager eventManager)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.databaseSO = databaseSO;
        this.floorData = floorData;
        this.structureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.eventManager = eventManager;

        selectedObjectIndex = databaseSO.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(databaseSO.objectsData[selectedObjectIndex].prefab,
                                                       databaseSO.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            Debug.LogError($"Invalid placement at {gridPosition}");
            return;
        }

        int index = objectPlacer.PlaceObject(databaseSO.objectsData[selectedObjectIndex].prefab, grid.CellToWorld(gridPosition));

        GridData selectedData = databaseSO.objectsData[selectedObjectIndex].ID == 0 ? floorData : structureData;

        selectedData.AddObjectAt(gridPosition,
                                 databaseSO.objectsData[selectedObjectIndex].Size,
                                 databaseSO.objectsData[selectedObjectIndex].ID,
                                 index);


        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);

        eventManager.OnStructurePlaced?.Invoke();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = databaseSO.objectsData[selectedObjectIndex].ID == 0 ? floorData : structureData;

        return selectedData.CanPlaceObjectAt(gridPosition, databaseSO.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
