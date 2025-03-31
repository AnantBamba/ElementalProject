using UnityEngine;
using System.Collections;
using Sydewa;
using OculusSampleFramework;

public class AirOrbTrigger : MonoBehaviour
{
    public bool isAirOrbPlaced = false;

    [SerializeField] private ParticleSystem fogParticleSystem;  // Reference to the fog particle system
    public float transitionSpeed = 1.0f;
    private Coroutine timeChangeCoroutine;
    private ParticleSystem.MainModule fogMainModule;

    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        if (fogParticleSystem == null)
        {
            Debug.LogError("Fog Particle System not assigned in the inspector!");
        }

        // Cache the main module of the fog particle system for performance optimization
        fogMainModule = fogParticleSystem.main;
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
            // Air Orb has fully left the altar, smoothly fade in the fog
            ReenableFogEffect();
        }
    }

    private void ReenableFogEffect()
    {
        isAirOrbPlaced = false;
        Debug.Log("Air Orb has been removed from the Altar!");

        // Smoothly fade in the fog effect
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
        Debug.Log("Air Orb has been placed on the Altar!");

        // Smoothly fade out the fog effect when the orb is placed
        StartCoroutine(SmoothFogTransition(false));
    }

    // Coroutine to handle smooth fog fade-in and fade-out
    private IEnumerator SmoothFogTransition(bool fadeIn)
    {
        float targetSize = fadeIn ? 1f : 0f;  // Target size of the fog
        float startTime = Time.time;
        float startSize = fogMainModule.startSize.constant;

        // If fading in, ensure the fog is not stopped completely
        if (fadeIn)
        {
            fogParticleSystem.Play();
        }
        else
        {
            fogParticleSystem.Stop();
        }

        // Smoothly transition the fog's size (or opacity if you modify other properties)
        while (Mathf.Abs(fogMainModule.startSize.constant - targetSize) > 0.01f)
        {
            float t = (Time.time - startTime) * transitionSpeed;
            float size = Mathf.Lerp(startSize, targetSize, t);

            fogMainModule.startSize = new ParticleSystem.MinMaxCurve(size);  // Update size or opacity
            yield return null;
        }

        // Finalize the size to ensure it is exactly at the target value
        fogMainModule.startSize = new ParticleSystem.MinMaxCurve(targetSize);
    }
}
