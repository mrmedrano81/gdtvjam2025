using UnityEngine;

public class SelectionUI : MonoBehaviour
{
    public GameObject selectionUI;
    public LayerMask selectionUIMask;

    InputManager inputManager;
    PlacementSystem placementSystem;

    Vector3 selectedPosition;

    GameObject selectedObject;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        placementSystem = FindFirstObjectByType<PlacementSystem>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene.");
            return;
        }

        inputManager.OnLeftClick += OnSelect;
    }

    private void OnSelect()
    {
        if (placementSystem.IsBuildingActive)
        {
            Debug.Log("Placement in progress, cannot select object.");
            DeselectCurrentObject();
            return; // Prevent selection while placing an object
        }

        int mask = selectionUIMask.value;

        Vector3 mousePosInput = inputManager.GetMousePosition();

        mousePosInput.z = Camera.main.nearClipPlane;

        Ray ray = Camera.main.ScreenPointToRay(mousePosInput);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            Debug.Log("Clicked on: " + hit.collider.gameObject.name);

            selectedPosition = hit.point;

            if (hit.collider.gameObject == selectedObject)
            {
                Debug.Log("Already selected: " + hit.collider.gameObject.name);

                SetupSelectedObject();
                return; // Already selected, do nothing
            }

            if (selectedObject != null)
            {
                DeselectCurrentObject();
            }

            selectedObject = hit.collider.gameObject;

            SetupSelectedObject();

            
        }
        else
        {
            if (selectedObject != null)
            {
                DeselectCurrentObject();
            }

            Debug.Log("No object selected, raycast did not hit any collider.");
        }
    }

    private void SetupSelectedObject()
    {
        selectedObject.GetComponentInParent<MaterialFlasher>().enabled = true;
    }

    private void DeselectCurrentObject()
    {
        if (selectedObject == null)
        {
            return; // Nothing to deselect
        }

        selectedObject.GetComponentInParent<MaterialFlasher>().enabled = false;

        selectedObject = null;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (selectedPosition != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(selectedPosition, 0.5f); // Draw a sphere at the selected position
        }
    }
}
