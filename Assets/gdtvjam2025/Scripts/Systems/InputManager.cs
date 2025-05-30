using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayerMask;

    public event Action OnLeftClick, OnLeftClickHold, OnExit, OnSpace;

    private PlayerInput playerInput;

    private InputAction mousePosInput;
    private InputAction mouseLeftClick;
    private InputAction escape;

    private InputAction space;
    private InputAction tab;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("General");


        mousePosInput = playerInput.actions.FindAction("Mouse Position");
        mouseLeftClick = playerInput.actions.FindAction("Left Click");
        escape = playerInput.actions.FindAction("Escape");
        space = playerInput.actions.FindAction("Space");
        tab = playerInput.actions.FindAction("Tab");
    }

    // Update is called once per frame
    void Update()
    {
        if (space.WasPressedThisFrame())
        {
            OnSpace?.Invoke();
        }
        if (mouseLeftClick.WasPressedThisFrame())
        {
            OnLeftClick?.Invoke();
        }
        if (mouseLeftClick.phase == InputActionPhase.Performed)
        {
            OnLeftClickHold?.Invoke();
        }
        if (tab.WasPressedThisFrame())
        {
            OnExit?.Invoke();
        }
        if (escape.WasPressedThisFrame())
        {
            GameState.Instance.PauseGame();
        }
    }

    public bool IsPointerOverUI()    
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    /// Get the position of the mouse in the world space
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = mousePosInput.ReadValue<Vector2>();

        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;

    }

    public Vector2 GetMousePosition()
    {
        return mousePosInput.ReadValue<Vector2>();
    }

    /// Get the position of the mouse in the world space
    public Vector3 GetMouseWorldPosition(int layerMask)
    {
        Vector3 mousePos = mousePosInput.ReadValue<Vector2>();

        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            return hit.point;
        }

        return lastPosition; // Return the last known position if no hit
    }
}
