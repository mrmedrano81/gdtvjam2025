using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayerMask;

    private PlayerInput playerInput;

    private InputAction mousePosInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("General");
        mousePosInput = playerInput.actions.FindAction("Mouse Position");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// Get the position of the mouse in the world space
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = mousePosInput.ReadValue<Vector2>();

        Debug.Log(mousePos);

        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;

    }
}
