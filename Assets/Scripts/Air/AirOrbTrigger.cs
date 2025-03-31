using UnityEngine;
using System.Collections;
using OculusSampleFramework;

public class AirOrbTrigger : MonoBehaviour
{
    public bool isAirOrbPlaced = false;

    [Header("Orb Snapping")]
    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    [Header("Particle System and Audio Settings")]
    public ParticleSystem airEffect; // Particle system for air orb effect
    public AudioSource airSound; // Audio source to modify the volume
    public float fadeDuration = 1.5f; // Time for the fade transitions

    private bool hasActivated = false;
    private Transform orbTransform;
    private float originalAudioVolume;

    private void Start()
    {
        if (airSound != null)
        {
            originalAudioVolume = airSound.volume;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AirOrb") && !hasActivated)
        {
            OVRGrabbable grabbable = other.GetComponent<OVRGrabbable>();
            if (grabbable != null && grabbable.isGrabbed)
                StartCoroutine(WaitForRelease(grabbable));
            else
                SnapOrbToAltar(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AirOrb"))
        {
            Debug.Log("Air Orb has been removed from the Altar!");
            isAirOrbPlaced = false;
            hasActivated = false; // Reset hasActivated to allow snapping again
            StartCoroutine(FadeInEffects()); // Fade in the particle system and restore audio volume
        }
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed) yield return null;
        if (grabbable.CompareTag("AirOrb")) SnapOrbToAltar(grabbable.gameObject);
    }

    private void SnapOrbToAltar(GameObject orb)
    {
        orb.transform.position = transform.TransformPoint(localSnapPosition);
        orb.transform.rotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        var rb = orb.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isAirOrbPlaced = true;
        hasActivated = true; // Set hasActivated to true to prevent re-snapping
        orbTransform = orb.transform;

        StartCoroutine(FadeOutEffects()); // Fade out the particle system and audio when the orb is placed
    }

    private IEnumerator FadeOutEffects()
    {
        // Fade the particle system
        if (airEffect != null)
        {
            var main = airEffect.main;
            float startTime = Time.time;

            while (Time.time - startTime < fadeDuration)
            {
                float t = (Time.time - startTime) / fadeDuration;
                main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }

            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 0f);
            airEffect.Stop();
        }

        // Fade the audio
        if (airSound != null)
        {
            float startVolume = airSound.volume;
            float startTime = Time.time;

            while (Time.time - startTime < fadeDuration)
            {
                float t = (Time.time - startTime) / fadeDuration;
                airSound.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            airSound.volume = 0f;
        }
    }

    private IEnumerator FadeInEffects()
    {
        // Fade the particle system
        if (airEffect != null)
        {
            var main = airEffect.main;
            float startTime = Time.time;

            while (Time.time - startTime < fadeDuration)
            {
                float t = (Time.time - startTime) / fadeDuration;
                main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Lerp(0f, 1f, t));
                yield return null;
            }

            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 1f);
            airEffect.Play();
        }

        // Fade the audio
        if (airSound != null)
        {
            float startVolume = airSound.volume;
            float startTime = Time.time;

            while (Time.time - startTime < fadeDuration)
            {
                float t = (Time.time - startTime) / fadeDuration;
                airSound.volume = Mathf.Lerp(startVolume, originalAudioVolume, t);
                yield return null;
            }

            airSound.volume = originalAudioVolume;
        }
    }
}
