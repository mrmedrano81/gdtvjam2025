using UnityEngine;

public class SelectionUI : MonoBehaviour
{
    public GameObject selectionUI;
    public LayerMask selectionUIMask;

    InputManager inputManager;

    Vector3 selectedPosition;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene.");
            return;
        }

        inputManager.OnLeftClick += OnSelect;
    }

    private void OnSelect()
    {
        int mask = selectionUIMask.value;

        Vector3 mousePosInput = inputManager.GetMousePosition();

        mousePosInput.z = Camera.main.nearClipPlane;

        Ray ray = Camera.main.ScreenPointToRay(mousePosInput);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            Debug.Log("Clicked on: " + hit.collider.gameObject.name);

            selectedPosition = hit.point;
        }
        else
        {
            Debug.Log("Nothing hit");
        }
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
