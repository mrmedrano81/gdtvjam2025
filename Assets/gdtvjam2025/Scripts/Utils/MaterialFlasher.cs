using UnityEngine;
using System.Collections.Generic;

public class MaterialFlasher : MonoBehaviour
{
    [Tooltip("The color to flash to.")]
    public Color flashColor = Color.white;
    [Tooltip("Speed of flashing in cycles per second.")]
    public float flashSpeed = 0.6f;

    private Renderer[] renderers;
    private List<Material> materials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private float time = 0f;

    private static MaterialFlasher materialFlasherInstance;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                materials.Add(mat);
                originalColors.Add(mat.color);
            }
        }

        materialFlasherInstance = this;

        materialFlasherInstance.enabled = false;
    }

    void Update()
    {
        time += Time.deltaTime;
        float t = (Mathf.Sin(time * flashSpeed * 2 * Mathf.PI) + 1f) * 0.5f;

        for (int i = 0; i < materials.Count; i++)
            materials[i].color = Color.Lerp(originalColors[i], flashColor, t);
    }

    void OnDisable()  // Reset colors when disabled
    {
        for (int i = 0; i < materials.Count; i++)
            materials[i].color = originalColors[i];
    }

    private void OnEnable()
    {
        time = 0f;
        for (int i = 0; i < materials.Count; i++)
            materials[i].color = flashColor;
    }
}
