using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO databaseSO;
    private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;

    private void Start()
    {
        StopPlacement();
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;


        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        mouseIndicator.transform.position = mousePosition;

        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
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
        cellIndicator.SetActive(true);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        GameObject newObject = Instantiate(databaseSO.objectsData[selectedObjectIndex].prefab);

        newObject.transform.position = grid.CellToWorld(gridPosition);

    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;

        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }


}
