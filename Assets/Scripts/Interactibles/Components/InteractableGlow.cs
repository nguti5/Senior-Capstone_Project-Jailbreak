
using UnityEngine;

public class InteractableGlow : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private float glowIntensity = 2f;

    private Material[] materials;
    private Color[] originalEmission;
    private bool[] hadEmission;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogWarning($"{name}: No Renderer found for glow.");
            return;
        }

        materials = targetRenderer.materials;
        originalEmission = new Color[materials.Length];
        hadEmission = new bool[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];

            if (mat.HasProperty("_EmissionColor"))
            {
                hadEmission[i] = true;
                originalEmission[i] = mat.GetColor("_EmissionColor");
            }
        }
    }

    public void SetGlow(bool enabled)
    {
        if (materials == null) return;

        for (int i = 0; i < materials.Length; i++)
        {
            if (!hadEmission[i]) continue;

            Material mat = materials[i];

            if (enabled)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor(
                    "_EmissionColor",
                    glowColor * glowIntensity
                );
            }
            else
            {
                mat.SetColor(
                    "_EmissionColor",
                    originalEmission[i]
                );
            }
        }
    }

    private void OnDisable()
    {
        SetGlow(false);
    }

    private void OnDestroy()
    {
        if (materials == null) return;

        foreach (Material mat in materials)
        {
            if (mat != null)
                Destroy(mat);
        }
    }
}
