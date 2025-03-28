using UnityEngine;

public class CameraShake : MonoBehaviour
{
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
    }
}
