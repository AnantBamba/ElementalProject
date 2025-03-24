using System.Collections;
using UnityEngine;

public class SpikeRiseWithEffects : MonoBehaviour
{
    [Header("Spike Movement")]
    public float riseHeight = 5f;
    public float riseDuration = 3f;
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 20f;
    public AnimationCurve riseCurve;

    [Header("Timing Controls")]
    public float delayBeforeAttach = 1.5f;
    public float shakeTime = 1.2f;
    public float delayBeforeDrop = 2.5f;

    [Header("References")]
    public Transform houseTransform;
    public CameraShake cameraShake;
    public Rigidbody[] detachableParts;
    public AudioSource spikeAudio;

    [Header("Camera Shake Settings")]
    public float camShakeDuration = 0.5f;
    public float camShakeMagnitude = 0.2f;

    private Vector3 startPos;
    private float elapsedTime = 0f;
    private bool rising = false;
    private bool hasTriggered = false;
    private bool hasAttachedHouse = false;
    private bool hasShaken = false;
    private bool partsDropped = false;
    private bool audioPlayed = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (rising)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / riseDuration);
            float curveValue = riseCurve.Evaluate(t);
            float newY = Mathf.Lerp(startPos.y, startPos.y + riseHeight, curveValue);

            float xShake = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
            float zShake = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

            transform.position = new Vector3(startPos.x + xShake, newY, startPos.z + zShake);

            if (!audioPlayed)
            {
                if (spikeAudio != null)
                {
                    spikeAudio.Play();
                }
                audioPlayed = true;
            }

            if (!hasAttachedHouse && elapsedTime >= delayBeforeAttach)
            {
                houseTransform.SetParent(this.transform);
                hasAttachedHouse = true;
            }

            if (!hasShaken && elapsedTime >= shakeTime)
            {
                if (cameraShake != null)
                {
                    cameraShake.TriggerShake(camShakeDuration, camShakeMagnitude);
                }
                hasShaken = true;
            }

            if (!partsDropped && elapsedTime >= delayBeforeDrop)
            {
                foreach (Rigidbody part in detachableParts)
                {
                    if (part != null)
                    {
                        part.isKinematic = false;
                        part.useGravity = true;
                        part.AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
                    }
                }
                partsDropped = true;
            }

            if (t >= 1f)
            {
                rising = false;
                transform.position = new Vector3(startPos.x, startPos.y + riseHeight, startPos.z);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            rising = true;
        }
    }
}
