using UnityEngine;
using System.Collections.Generic;
using System;

public class PreviewSystem : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float transparencyAlpha = 0.5f;
    [SerializeField] private float previewYOffset = 0.06f;

    [SerializeField] private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField] private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    public LayerMask selectionUIMask;
    InputManager inputManager;

    private GameObject selectedObject;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);

        if (previewObject != null) Destroy(previewObject);

        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color color = validity ? Color.white : Color.red;

        color.a = transparencyAlpha;

        previewMaterialInstance.color = color;

    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color color = validity ? Color.white : Color.red;

        color.a = transparencyAlpha;

        cellIndicatorRenderer.material.color = color;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);

        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    public void ShowObjectToBeRemoved()
    {
        int mask = selectionUIMask.value;

        Vector3 mousePosInput = inputManager.GetMousePosition();

        mousePosInput.z = Camera.main.nearClipPlane;

        Ray ray = Camera.main.ScreenPointToRay(mousePosInput);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            if (hit.collider.gameObject.CompareTag("HQ"))
            {
                return;
            }

            if (hit.collider.gameObject == selectedObject)
            {
                selectedObject.GetComponentInParent<MaterialFlasher>().enabled = true;
                selectedObject.GetComponentInParent<MaterialFlasher>().flashColor = Color.red;
                return; // Already selected, do nothing
            }

            if (selectedObject != null)
            {
                selectedObject.GetComponentInParent<MaterialFlasher>().enabled = false;
                selectedObject.GetComponentInParent<MaterialFlasher>().flashColor = Color.white;
                selectedObject = null;
            }

            selectedObject = hit.collider.gameObject;

            selectedObject.GetComponentInParent<MaterialFlasher>().enabled = true;
            selectedObject.GetComponentInParent<MaterialFlasher>().flashColor = Color.red;



        }
        else
        {
            if (selectedObject != null)
            {
                selectedObject.GetComponentInParent<MaterialFlasher>().enabled = false;
                selectedObject.GetComponentInParent<MaterialFlasher>().flashColor = Color.white;
                selectedObject = null;
            }

            Debug.Log("No object selected, raycast did not hit any collider.");
        }
    }
}
