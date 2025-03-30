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
    public float riseDelay = 10f;         // ⏱️ seconds after rain starts
    public float riseTargetY = 5f;
    public float riseDuration = 3f;

    [Header("Terrain Detail Expansion")]
    public TerrainDetailExpander terrainDetailExpander;
    public float terrainTriggerDelay = 15f; // ⏱️ seconds after rain starts

    private bool hasActivated = false;
    private Transform orbTransform;

    private void Start()
    {
        if (rainEffect != null && rainEffect.isPlaying)
        {
            rainEffect.Stop();
        }
        if (rainAudio != null && rainAudio.isPlaying)
        {
            rainAudio.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterOrb") && !hasActivated)
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
        if (other.CompareTag("WaterOrb"))
        {
            Debug.Log("Water Orb has been removed from the Altar!");
            isWaterOrbPlaced = false;
        }
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
        {
            yield return null;
        }

        if (grabbable.CompareTag("WaterOrb"))
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

        isWaterOrbPlaced = true;
        hasActivated = true;
        orbTransform = orb.transform; // Save for terrain spreading

        Debug.Log("Water Orb has been placed on the Altar!");

        StartCoroutine(HandleWaterSequence());
    }

    private IEnumerator HandleWaterSequence()
    {
        float rainStartTime = Time.time;

        // Step 1: Start rain
        if (rainEffect != null) rainEffect.Play();
        if (rainAudio != null) rainAudio.Play();

        // Step 2: Wait for 10s then raise water
        yield return new WaitForSeconds(riseDelay);

        if (waterPlane != null)
        {
            Vector3 startPos = waterPlane.position;
            Vector3 endPos = new Vector3(startPos.x, riseTargetY, startPos.z);

            float elapsed = 0f;
            while (elapsed < riseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / riseDuration;
                waterPlane.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            waterPlane.position = endPos;
        }

        // Step 3: Wait until 15s since rain started, then trigger terrain spread
        float timeSinceRainStarted = Time.time - rainStartTime;
        float waitMore = Mathf.Max(0, terrainTriggerDelay - timeSinceRainStarted);
        yield return new WaitForSeconds(waitMore);

        if (terrainDetailExpander != null && orbTransform != null)
        {
            terrainDetailExpander.StartDetailExpansion(orbTransform);
        }

        // Step 4: Wait until rainDuration passed since rain started, then stop rain
        float rainTotalTime = Time.time - rainStartTime;
        float remainingRain = Mathf.Max(0, rainDuration - rainTotalTime);
        yield return new WaitForSeconds(remainingRain);

        if (rainEffect != null) rainEffect.Stop();
        if (rainAudio != null) rainAudio.Stop();
    }
}
