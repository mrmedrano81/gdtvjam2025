using UnityEngine;

public class WorldUIModifier : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] private bool faceCamera = true;

    private Camera mainCamera;
    //private RectTransform UITransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        //UITransform = GetComponent<RectTransform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (faceCamera)
        {
            FaceCamera();
        }
    }

    private void FaceCamera()
    { 
        if (mainCamera != null)
        {
            Vector3 viewDirection = mainCamera.transform.position - transform.position;

            transform.rotation = Quaternion.LookRotation(viewDirection, mainCamera.transform.up);

            //transform.LookAt(mainCamera.transform.position);
        }
    }
}
