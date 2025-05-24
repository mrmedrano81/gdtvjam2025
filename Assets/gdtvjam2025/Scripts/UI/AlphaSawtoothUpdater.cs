using UnityEngine;

// This would be part of a MonoBehaviour script attached to your GameObject
public class AlphaSawtoothUpdater : MonoBehaviour
{
    private Renderer _renderer;
    private Material _material;

    // Parameters for the sawtooth alpha effect
    public float minAlpha = 0.0f;
    public float maxAlpha = 1.0f;
    public float flickerSpeed = 2.0f;
    public float timeOffset = 0.0f;
    public float smoothingFactor = 0.5f; // Range 0-1

    [Tooltip("The name of the float property in your shader that controls alpha (e.g., '_Alpha', '_Opacity').")]
    public string shaderAlphaPropertyName = "_BaseColor"; // <--- SET THIS IN INSPECTOR

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material; // Gets the material instance
    }

    // Call this function every frame (e.g., from Update())
    public void UpdateSawtoothShaderAlphaFloat()
    {
        // Calculate a raw oscillating value (0 to 1 and back)
        float rawPingPongValue = Mathf.PingPong(Time.time * flickerSpeed + timeOffset, 1.0f);

        // Apply smoothing
        float smoothedValue;
        if (rawPingPongValue <= 0.5f)
        {
            float t = rawPingPongValue * 2f;
            smoothedValue = Mathf.Lerp(t, Mathf.SmoothStep(0f, 1f, t), smoothingFactor);
        }
        else
        {
            float t = (1f - rawPingPongValue) * 2f;
            smoothedValue = Mathf.Lerp(t, Mathf.SmoothStep(0f, 1f, t), smoothingFactor);
        }

        // Map the smoothed value (0-1) to the desired alpha range
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, smoothedValue);

        // Set the alpha property in the shader
        _material.SetFloat(shaderAlphaPropertyName, currentAlpha);
    }

    // Optional: Reset alpha when disabled
    void OnDisable()
    {
        // Set to maxAlpha or minAlpha depending on your desired default state
        if (_material != null)
        {
            _material.SetFloat(shaderAlphaPropertyName, maxAlpha);
        }
    }
}