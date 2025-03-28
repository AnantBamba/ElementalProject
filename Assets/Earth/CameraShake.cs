<<<<<<< HEAD
using System.Collections;
=======
>>>>>>> Max
using UnityEngine;

public class CameraShake : MonoBehaviour
{
<<<<<<< HEAD
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
=======
    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0.2f;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = originalPos + shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
>>>>>>> Max
    }
}
