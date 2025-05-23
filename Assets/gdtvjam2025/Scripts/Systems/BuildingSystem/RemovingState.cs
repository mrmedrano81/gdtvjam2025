using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;

    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO databaseSO;
    GridData floorData;
    GridData structureData;
    ObjectPlacer objectPlacer;
    EventManager eventManager;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         ObjectsDatabaseSO databaseSO,
                         GridData floorData,
                         GridData structureData,
                         ObjectPlacer objectPlacer,
                         EventManager eventManager)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.databaseSO = databaseSO;
        this.floorData = floorData;
        this.structureData = structureData;
        this.objectPlacer = objectPlacer;
        this.eventManager = eventManager;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;

        if (structureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = structureData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if (selectedData == null)
        {
            // play sound that nothing can be removed
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);

            if (gameObjectIndex == -1)
            {
                return;
            }

            PlacementData placementData = selectedData.GetPlacementDataOfObjectAt(gridPosition);

            selectedData.RemoveObjectAt(gridPosition);

            objectPlacer.RemoveObjectAt(gameObjectIndex, placementData.structureType);
        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);

        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));

        eventManager.OnStructureRemoved?.Invoke();
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(structureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
               floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
