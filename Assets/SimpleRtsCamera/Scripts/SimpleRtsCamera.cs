using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleRtsCamera : MonoBehaviour
{
	[Header("DEBUGGING")]
	public bool disableCursorMove = false;

	[Header("RTS Camera Settings")]
	[Header("Move")]
	[SerializeField] private float _moveSpeed = 200;
	[SerializeField] private float _edgeThreshold = 5;
	[SerializeField] private float _rightMouseSpeedMultiplier = 10;
	[SerializeField] private Vector2 cameraXZBounds;

	[Header("Zoom")]
	[SerializeField] private float _zoomSpeed = 200;
	[SerializeField] private float _minZoomDistance = 200;
	[SerializeField] private float _maxZoomDistance = 200;
	[SerializeField] private float _currentZoomDistance = 0;

	[Header("Rotate")]
	[SerializeField] private float _rotateSpeed = 0.5f;
	[SerializeField] private float _rotateFallback = 1000f;

	private PlayerInput _playerInput;
	private Vector2 _moveInput;
	private Vector2 _mousePositionInput;
	private float _rightMouseInput;
	private Vector2 _initialMousePosition;
	private float _scrollMouseInput;
	private float _middleMouseInput;

	public Vector3 mousePosition;

	Quaternion _initialRotation;
	private Vector3 _initialPosition;
    private Vector3 _initialLookAtPoint; // Store the initial look-at point
    private Vector3 _initialPositionOffsetFromLookAt; // Store the initial offset

    private bool resetCameraRotation = true;
    private bool resetCameraPosition = true;


    private void Awake()
    {
        _playerInput = FindAnyObjectByType<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("General");
        _initialRotation = transform.rotation;
        _initialPosition = transform.position;

        // Calculate and store the initial look-at point and offset
        _initialLookAtPoint = GetCameraLookAtPoint();
        _initialPositionOffsetFromLookAt = transform.position - _initialLookAtPoint;
    }

    private void OnEnable()
	{
		_playerInput.actions["CameraMove"].performed += MoveHandler;
		_playerInput.actions["CameraMove"].canceled += MoveHandler;

		_playerInput.actions["Mouse Position"].performed += MousePositionHandler;
		_playerInput.actions["Mouse Position"].canceled += MousePositionHandler;

		_playerInput.actions["RightMouse"].started += InitialMousePositionHandler;
		_playerInput.actions["RightMouse"].performed += RightMouseHandler;
		_playerInput.actions["RightMouse"].canceled += RightMouseHandler;

		_playerInput.actions["ScrollMouse"].performed += ScrollMouseHandler;
		_playerInput.actions["ScrollMouse"].canceled += ScrollMouseHandler;

		_playerInput.actions["MiddleMouse"].started += InitialMousePositionHandler;
		_playerInput.actions["MiddleMouse"].performed += MiddleMouseHandler;
		_playerInput.actions["MiddleMouse"].canceled += MiddleMouseHandler;
	}

	private void LateUpdate()
	{

		if (!disableCursorMove)
		{
            MoveCamera();
            MoveCameraWithCursor();
        }

		MoveCameraWithRightMouse();
		ZoomCamera();
		RotateCamera();
		ClampCameraPosition();
	}

	private void OnDisable()
	{
		if (!_playerInput) return;

		_playerInput.actions["CameraMove"].performed -= MoveHandler;
		_playerInput.actions["CameraMove"].canceled -= MoveHandler;

		_playerInput.actions["Mouse Position"].performed -= MousePositionHandler;
		_playerInput.actions["Mouse Position"].canceled -= MousePositionHandler;

		_playerInput.actions["RightMouse"].started -= InitialMousePositionHandler;
		_playerInput.actions["RightMouse"].performed -= RightMouseHandler;
		_playerInput.actions["RightMouse"].canceled -= RightMouseHandler;

		_playerInput.actions["ScrollMouse"].performed -= ScrollMouseHandler;
		_playerInput.actions["ScrollMouse"].canceled -= ScrollMouseHandler;

		_playerInput.actions["MiddleMouse"].started -= InitialMousePositionHandler;
		_playerInput.actions["MiddleMouse"].performed -= MiddleMouseHandler;
		_playerInput.actions["MiddleMouse"].canceled -= MiddleMouseHandler;
	}

	private void MoveHandler(InputAction.CallbackContext callbackContext) =>
		_moveInput = callbackContext.ReadValue<Vector2>();

	private void MousePositionHandler(InputAction.CallbackContext callbackContext)
	{
        _mousePositionInput = callbackContext.ReadValue<Vector2>();
        mousePosition = callbackContext.ReadValue<Vector2>();
    }

    private void InitialMousePositionHandler(InputAction.CallbackContext callbackContext) =>
		_initialMousePosition = _mousePositionInput;

	private void RightMouseHandler(InputAction.CallbackContext callbackContext) =>
		_rightMouseInput = callbackContext.ReadValue<float>();

	private void ScrollMouseHandler(InputAction.CallbackContext callbackContext) =>
		_scrollMouseInput = callbackContext.ReadValue<float>();

	private void MiddleMouseHandler(InputAction.CallbackContext callbackContext) =>
		_middleMouseInput = callbackContext.ReadValue<float>();

	private void MoveCamera()
	{
		var moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y) * (_moveSpeed * Time.deltaTime);
		transform.position += moveDirection;
	}

	public bool RightMouseClicked()
	{
		return _playerInput.actions.FindAction("RightMouse").phase == InputActionPhase.Performed;
    }

	private void MoveCameraWithCursor()
	{
		if (_mousePositionInput.x < _edgeThreshold)
		{
			transform.position += -transform.right * (_moveSpeed * Time.deltaTime);
		}
		else if (_mousePositionInput.x > Screen.width - _edgeThreshold)
		{
			transform.position += transform.right * (_moveSpeed * Time.deltaTime);
		}

		if (_mousePositionInput.y < _edgeThreshold)
		{
			transform.position += -Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * (_moveSpeed * Time.deltaTime);
		}
		else if (_mousePositionInput.y > Screen.height - _edgeThreshold)
		{
			transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * (_moveSpeed * Time.deltaTime);
		}
	}

	private void MoveCameraWithRightMouse()
	{
		if (Mathf.Approximately(_rightMouseInput, 0) || _initialMousePosition == Vector2.zero) return;

		var mouseDelta = _mousePositionInput - _initialMousePosition;

		var moveX = mouseDelta.x * (_moveSpeed * _rightMouseSpeedMultiplier / Screen.width) * Time.deltaTime;
		var moveY = mouseDelta.y * (_moveSpeed * _rightMouseSpeedMultiplier / Screen.height) * Time.deltaTime;

		var moveDirection = new Vector3(moveX, 0, moveY);
        transform.position += moveDirection;
	}

    private void ZoomCamera()
    {
        if (Mathf.Approximately(_scrollMouseInput, 0)) return;

        Vector3 zoomDirection = transform.forward;

        // Get current distance from camera to look-at point
        Vector3 lookAtPoint = GetCameraLookAtPoint();
        float currentDistance = Vector3.Distance(transform.position, lookAtPoint);

        // Calculate the proposed zoom delta
        float zoomDelta = _scrollMouseInput * _zoomSpeed * Time.deltaTime;

        // Calculate proposed new distance
        float newDistance = currentDistance - zoomDelta;

        // Clamp the new distance
        newDistance = Mathf.Clamp(newDistance, _minZoomDistance, _maxZoomDistance);

        // Calculate the actual zoom movement to apply
        float actualZoom = currentDistance - newDistance;

        // Move the camera
        transform.position += zoomDirection * actualZoom;

        // Optionally update _currentZoomDistance
        _currentZoomDistance = Vector3.Distance(transform.position, lookAtPoint);
    }

    //private void ZoomCamera()
    //{
    //	if (Mathf.Approximately(_scrollMouseInput, 0)) return;

    //	var zoomDirection = transform.forward;


    //	float dist = Vector3.Distance(GetCameraLookAtPoint(), transform.position);

    //	Debug.Log($"Distance: {dist}");

    //	// clamp zoom distance

    //	transform.position += zoomDirection * (_scrollMouseInput * _zoomSpeed * Time.deltaTime);

    //	_currentZoomDistance = transform.position.y;
    //}


    private void RotateCamera()
	{
		if (Mathf.Approximately(_middleMouseInput, 0)) return;

		var lookAtPoint = GetCameraLookAtPoint();
		var mouseDelta = _mousePositionInput - _initialMousePosition;
		transform.RotateAround(lookAtPoint, Vector3.up, mouseDelta.x * _rotateSpeed * Time.deltaTime);
	}

    #region Camera UI Settings
    public void ToggleCameraResetRotation(bool toggle)
    {
        resetCameraRotation = toggle;
    }

    public void ToggleCameraResetPosition(bool toggle)
    {
        resetCameraPosition = toggle;
    }

    public void ResetCameraRotation()
    {
        if (resetCameraRotation)
        {
            transform.rotation = _initialRotation;
        }

        if (resetCameraPosition)
        {
            // Reset position relative to the initial look-at point
            transform.position = _initialLookAtPoint + _initialPositionOffsetFromLookAt;
        }
    }
    #endregion

    private Vector3 GetCameraLookAtPoint()
	{
		var ray = new Ray(transform.position, transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
		{
			return hit.point;
		}

		var groundPlane = new Plane(Vector3.up, Vector3.zero);
		if (groundPlane.Raycast(ray, out float enter))
		{
			return ray.GetPoint(enter);
		}

		return transform.position + transform.forward * _rotateFallback;
	}

	private void ClampCameraPosition()
	{
        //Vector3 lookAtPoint = GetCameraLookAtPoint();
        //lookAtPoint.y = 0;

        //float distX = transform.position.x - lookAtPoint.x;
        //float distZ = transform.position.z - lookAtPoint.z;

        //         float cameraBoundsX = cameraXZBounds.x + distX;
        //float cameraBoundsZ = cameraXZBounds.y + distZ;

        //var clampedX = Mathf.Clamp(transform.position.x, -cameraBoundsX, cameraBoundsX);
        //         var clampedZ = Mathf.Clamp(transform.position.z, -cameraBoundsZ, cameraBoundsZ);			


        // 1. Get the current look-at point
        Vector3 lookDir = transform.forward;
        float t = -transform.position.y / lookDir.y; // ray-plane intersection at y=0
        Vector3 lookAtPoint = transform.position + lookDir * t;
        // 2. Clamp the look-at point within bounds
        Vector3 clampedLookAt = lookAtPoint;
        clampedLookAt.x = Mathf.Clamp(lookAtPoint.x, -cameraXZBounds.x, cameraXZBounds.x);
        clampedLookAt.z = Mathf.Clamp(lookAtPoint.z, -cameraXZBounds.y, cameraXZBounds.y);
        clampedLookAt.y = 0;

        // 3. Maintain the distance and direction from camera to look-at point
        Vector3 cameraToLook = transform.position - lookAtPoint;
        transform.position = clampedLookAt + cameraToLook;

			



        //var clampedX = Mathf.Clamp(transform.position.x, -cameraXZBounds.x, cameraXZBounds.x);
        //var clampedZ = Mathf.Clamp(transform.position.z, -cameraXZBounds.y, cameraXZBounds.y);

        //transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
    }
}
