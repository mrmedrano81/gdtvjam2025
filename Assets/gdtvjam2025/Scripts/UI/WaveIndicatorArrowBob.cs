using UnityEngine;

public class WaveIndicatorArrowBob : MonoBehaviour
{
    [Tooltip("The total distance the arrow will travel forward and backward.")]
    [SerializeField] private float travelDistance = 1.0f;

    [Tooltip("The speed at which the arrow moves.")]
    [SerializeField] private float movementSpeed = 2.0f;

    [Tooltip("The direction the arrow moves relative to its own forward axis.")]
    [SerializeField] private Vector3 localMovementDirection = Vector3.forward;

    [Tooltip("Offset applied to the time calculation, useful for staggering multiple arrows.")]
    [SerializeField] private float timeOffset = 0.0f;

    [Tooltip("Controls how smooth the transitions are at the ends of the movement (0 = sharp, 1 = very smooth).")]
    [Range(0f, 1f)]
    [SerializeField] private float smoothingFactor = 0.5f;

    private Vector3 _initialLocalPosition;

    private void OnDisable()
    {
        transform.localPosition = _initialLocalPosition;
    }

    void Awake()
    {
        _initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate a raw oscillating value (0 to 1 and back)
        float rawPingPongValue = Mathf.PingPong(Time.time * movementSpeed + timeOffset, 1.0f);

        // Apply smoothing based on the smoothingFactor
        float smoothedValue;
        if (rawPingPongValue <= 0.5f)
        {
            // Moving forward (0 to 0.5 scaled to 0-1)
            float t = rawPingPongValue * 2f;
            smoothedValue = Mathf.Lerp(t, Mathf.SmoothStep(0f, 1f, t), smoothingFactor);
        }
        else
        {
            // Moving backward (0.5 to 1 scaled to 0-1, reversed)
            float t = (1f - rawPingPongValue) * 2f;
            smoothedValue = Mathf.Lerp(t, Mathf.SmoothStep(0f, 1f, t), smoothingFactor);
        }

        // Calculate the arrow's new position
        Vector3 displacement = localMovementDirection.normalized * (smoothedValue * travelDistance);
        transform.localPosition = _initialLocalPosition + displacement;
    }
}