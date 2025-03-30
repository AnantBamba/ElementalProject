using UnityEngine;
using System.Collections;
using Sydewa;
using OculusSampleFramework;

public class AirOrbTrigger : MonoBehaviour
{
    public bool isAirOrbPlaced = false;

    [Header("Fog Settings")]
    [SerializeField] private ParticleSystem fogParticleSystem;
    public float transitionSpeed = 1.0f;
    private ParticleSystem.MainModule fogMainModule;

    [Header("Wind Audio Settings")]
    [SerializeField] private AudioSource windAudioSource;
    [SerializeField] private float windDelay = 2f;
    [SerializeField] private float windFadeInDuration = 3f;
    [SerializeField] private float windFadeOutDuration = 2f;

    [Header("Orb Snap Settings")]
    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        if (fogParticleSystem == null)
        {
            Debug.LogError("Fog Particle System not assigned in the inspector!");
        }

        fogMainModule = fogParticleSystem.main;

        if (windAudioSource != null)
        {
            windAudioSource.volume = 0f;
            StartCoroutine(DelayedFadeInWind());
        }
        else
        {
            Debug.LogWarning("Wind AudioSource not assigned!");
        }
    }

    private IEnumerator DelayedFadeInWind()
    {
        yield return new WaitForSeconds(windDelay);
        windAudioSource.Play();

        float elapsed = 0f;
        while (elapsed < windFadeInDuration)
        {
            elapsed += Time.deltaTime;
            windAudioSource.volume = Mathf.Lerp(0f, 1f, elapsed / windFadeInDuration);
            yield return null;
        }

        windAudioSource.volume = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AirOrb"))
        {
            OVRGrabbable grabbable = other.GetComponent<OVRGrabbable>();

            if (grabbable != null && grabbable.isGrabbed)
            {
                StartCoroutine(WaitForRelease(grabbable));
            }
            else
            {
                SnapOrbToAltar(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AirOrb"))
        {
            FadeInFog();
        }
    }

    private void FadeInFog()
    {
        isAirOrbPlaced = false;
        Debug.Log("Air Orb removed from the altar.");
        StartCoroutine(SmoothFogTransition(true));
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
        {
            yield return null;
        }

        if (grabbable.CompareTag("AirOrb"))
        {
            SnapOrbToAltar(grabbable.gameObject);
        }
    }

    private void SnapOrbToAltar(GameObject orb)
    {
        Vector3 worldSnapPosition = transform.TransformPoint(localSnapPosition);
        Quaternion worldSnapRotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        orb.transform.position = worldSnapPosition;
        orb.transform.rotation = worldSnapRotation;

        Rigidbody orbRb = orb.GetComponent<Rigidbody>();
        if (orbRb != null)
        {
            orbRb.isKinematic = true;
        }

        isAirOrbPlaced = true;
        Debug.Log("Air Orb placed on the altar.");

        // Fade out wind sound
        StartCoroutine(FadeOutWindAudio());

        // Fade out fog
        StartCoroutine(SmoothFogTransition(false));
    }

    private IEnumerator SmoothFogTransition(bool fadeIn)
    {
        float targetSize = fadeIn ? 1f : 0f;
        float startTime = Time.time;
        float startSize = fogMainModule.startSize.constant;

        if (fadeIn)
        {
            fogParticleSystem.Play();
        }
        else
        {
            fogParticleSystem.Stop();
        }

        while (Mathf.Abs(fogMainModule.startSize.constant - targetSize) > 0.01f)
        {
            float t = (Time.time - startTime) * transitionSpeed;
            float size = Mathf.Lerp(startSize, targetSize, t);
            fogMainModule.startSize = new ParticleSystem.MinMaxCurve(size);
            yield return null;
        }

        fogMainModule.startSize = new ParticleSystem.MinMaxCurve(targetSize);
    }

    private IEnumerator FadeOutWindAudio()
    {
        float startVolume = windAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < windFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            windAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / windFadeOutDuration);
            yield return null;
        }

        windAudioSource.Stop();
        windAudioSource.volume = 0f;
    }
}
