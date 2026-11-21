using UnityEngine;

[RequireComponent(typeof(Light))]
public class PulsingLight : MonoBehaviour
{
    public enum Mode { Steady, Pulse, Flicker }

    [SerializeField] private Mode mode = Mode.Pulse;
    [SerializeField] private Color color = new Color(0f, 0.9f, 1f);
    [SerializeField] private float baseIntensity = 2f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField, Range(0f, 1f)] private float pulseDepth = 0.5f;     // 0 = no change, 1 = fades fully out

    [Header("Flicker")]
    [SerializeField] private float flickerSpeed = 12f;
    [SerializeField, Range(0f, 1f)] private float flickerAmount = 0.6f;

    [Header("Optional: a glowing mesh that follows the light (an LED strip, a panel)")]
    [SerializeField] private Renderer emissiveRenderer;
    [SerializeField, ColorUsage(false, true)] private Color emissiveColor = new Color(0f, 1.5f, 1.5f);

    private Light lightComponent;
    private Material emissiveMaterial;
    private float offset;

    private void Awake()
    {
        lightComponent = GetComponent<Light>();
        offset = Random.value * 100f;          // so lights don't pulse in perfect sync

        if (emissiveRenderer != null)
        {
            emissiveMaterial = emissiveRenderer.material;      // its own copy
            emissiveMaterial.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        float factor = 1f;

        if (mode == Mode.Pulse)
        {
            float s = (Mathf.Sin((Time.time + offset) * pulseSpeed) + 1f) * 0.5f;
            factor = Mathf.Lerp(1f - pulseDepth, 1f, s);
        }
        else if (mode == Mode.Flicker)
        {
            float n = Mathf.PerlinNoise((Time.time + offset) * flickerSpeed, offset);
            factor = Mathf.Lerp(1f - flickerAmount, 1f, n);
        }

        lightComponent.color = color;
        lightComponent.intensity = baseIntensity * factor;

        if (emissiveMaterial != null)
            emissiveMaterial.SetColor("_EmissionColor", emissiveColor * factor);
    }

    private void OnDestroy()
    {
        if (emissiveMaterial != null) Destroy(emissiveMaterial);
    }
}