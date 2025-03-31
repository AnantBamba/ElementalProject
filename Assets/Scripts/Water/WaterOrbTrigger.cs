using UnityEngine;
using System.Collections;
using OculusSampleFramework;

public class WaterOrbTrigger : MonoBehaviour
{
    public bool isWaterOrbPlaced = false;

    [Header("Orb Snapping")]
    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    [Header("Rain Effects")]
    public ParticleSystem rainEffect;
    public AudioSource rainAudio;
    public float rainDuration = 8f;

    [Header("Water Plane Rise")]
    public Transform waterPlane;
    public float riseDelay = 10f;
    public float riseTargetY = 5f;
    public float riseDuration = 3f;

    [Header("Target Object")]
    public GameObject targetObject; // The object to disable/enable with fade effect
    public float fadeSpeed = 1.0f;

    private bool hasActivated = false;
    private Transform orbTransform;
    private float originalAudioVolume;

    private void Start()
    {
        if (rainAudio != null)
        {
            originalAudioVolume = rainAudio.volume;
        }

        // Start rain at the beginning of the scene
        if (rainEffect != null)
        {
            rainEffect.Play();
            Debug.Log("Rain started at scene start.");
        }
        if (rainAudio != null)
        {
            rainAudio.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterOrb") && !hasActivated)
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
        if (other.CompareTag("WaterOrb"))
        {
            Debug.Log("Water Orb has been removed from the Altar!");
            isWaterOrbPlaced = false;
            hasActivated = false; // Allow snapping again
            ReenableTargetObject();
            StartCoroutine(FadeInEffects()); // Restart rain when orb is removed
        }
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed) yield return null;
        if (grabbable.CompareTag("WaterOrb")) SnapOrbToAltar(grabbable.gameObject);
    }

    private void SnapOrbToAltar(GameObject orb)
    {
        orb.transform.position = transform.TransformPoint(localSnapPosition);
        orb.transform.rotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        var rb = orb.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isWaterOrbPlaced = true;
        hasActivated = true;
        orbTransform = orb.transform;

        if (targetObject != null) targetObject.SetActive(false);

        StartCoroutine(FadeOutEffects()); // Stop rain when orb is placed
        StartCoroutine(HandleWaterSequence());
    }

    private void ReenableTargetObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    private IEnumerator FadeOutEffects()
    {
        if (rainEffect != null)
        {
            var main = rainEffect.main;
            float startTime = Time.time;

            while (Time.time - startTime < rainDuration)
            {
                float t = (Time.time - startTime) / rainDuration;
                main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }

            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 0f);
            rainEffect.Stop();
            Debug.Log("Rain stopped.");
        }

        if (rainAudio != null)
        {
            float startVolume = rainAudio.volume;
            float startTime = Time.time;

            while (Time.time - startTime < rainDuration)
            {
                float t = (Time.time - startTime) / rainDuration;
                rainAudio.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            rainAudio.volume = 0f;
            rainAudio.Stop();
        }
    }

    private IEnumerator FadeInEffects()
    {
        if (rainEffect != null)
        {
            var main = rainEffect.main;
            float startTime = Time.time;

            while (Time.time - startTime < rainDuration)
            {
                float t = (Time.time - startTime) / rainDuration;
                main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Lerp(0f, 1f, t));
                yield return null;
            }

            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 1f);
            rainEffect.Play();
            Debug.Log("Rain restarted.");
        }

        if (rainAudio != null)
        {
            float startVolume = rainAudio.volume;
            float startTime = Time.time;

            while (Time.time - startTime < rainDuration)
            {
                float t = (Time.time - startTime) / rainDuration;
                rainAudio.volume = Mathf.Lerp(startVolume, originalAudioVolume, t);
                yield return null;
            }

            rainAudio.volume = originalAudioVolume;
            rainAudio.Play();
        }
    }

    private IEnumerator HandleWaterSequence()
    {
        yield return new WaitForSeconds(riseDelay);

        if (waterPlane != null)
        {
            Vector3 start = waterPlane.position;
            Vector3 end = new Vector3(start.x, riseTargetY, start.z);
            float elapsed = 0f;
            while (elapsed < riseDuration)
            {
                elapsed += Time.deltaTime;
                waterPlane.position = Vector3.Lerp(start, end, elapsed / riseDuration);
                yield return null;
            }
            waterPlane.position = end;
        }
    }
}
