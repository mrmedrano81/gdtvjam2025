using UnityEngine;

public class MinimapCameraScript : MonoBehaviour
{
    public SimpleRtsCamera mainCamera;
    
    [Header("Settings")]
    public bool updateRotation = true;

    private float zoomScale;

    private void Awake()
    {
        mainCamera = FindFirstObjectByType<SimpleRtsCamera>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updateRotation)
        {
            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        if (mainCamera != null)
        {
            transform.rotation = Quaternion.Euler(90f, mainCamera.transform.eulerAngles.y, 0f);
        }
    }
}
