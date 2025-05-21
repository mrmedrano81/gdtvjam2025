using UnityEngine;

public class LineFade : MonoBehaviour
{
    public Color color;
    public Color originalColor;
    public float speed = 10f;

    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        originalColor = lineRenderer.startColor;
        color = lineRenderer.startColor;
    }

    private void Update()
    {
        color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * speed);

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void OnEnable()
    {
        color = originalColor;
    }
}
