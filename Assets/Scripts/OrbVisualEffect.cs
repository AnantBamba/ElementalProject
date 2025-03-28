using UnityEngine;

public class OrbVisualEffect : MonoBehaviour
{
    public Renderer orbRenderer;
    private Material orbMaterial;
    private Color originalColor;

    void Start()
    {
        orbMaterial = orbRenderer.material;
        originalColor = orbMaterial.GetColor("_EmissionColor");
    }

    public void SetOrbGlow(bool isActive)
    {
        Color glowColor = isActive ? Color.cyan * 2f : originalColor;
        orbMaterial.SetColor("_EmissionColor", glowColor);
    }
}