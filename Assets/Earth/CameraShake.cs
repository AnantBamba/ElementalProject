using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;

    [Header("Default Settings")]
    public float defaultDuration = 0.3f;
    public float defaultIntensity = 0.2f;

    void Start()
    {
        originalPosition = transform.localPosition;
    }


    public void TriggerShake(float duration = -1f, float intensity = -1f)
    {
        float d = (duration > 0) ? duration : defaultDuration;
        float i = (intensity > 0) ? intensity : defaultIntensity;

        StopAllCoroutines();
        StartCoroutine(DoShake(d, i));
    }

    private IEnumerator DoShake(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            transform.localPosition = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
