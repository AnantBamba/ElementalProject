using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light wispLight;
    public float minIntensity = 1.5f;
    public float maxIntensity = 2.5f;
    public float flickerSpeed = 0.2f;

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        wispLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}