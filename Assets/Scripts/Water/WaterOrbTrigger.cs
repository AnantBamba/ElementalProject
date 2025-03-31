// WaterOrbTrigger.cs
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

    [Header("Terrain Detail Expansion")]
    public TerrainDetailExpander terrainDetailExpander;
    public float terrainTriggerDelay = 15f;

    private bool hasActivated = false;
    private Transform orbTransform;

    private void Start()
    {
        if (rainEffect != null && rainEffect.isPlaying) rainEffect.Stop();
        if (rainAudio != null && rainAudio.isPlaying) rainAudio.Stop();
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

        StartCoroutine(HandleWaterSequence());
    }

    private IEnumerator HandleWaterSequence()
    {
        float rainStartTime = Time.time;
        if (rainEffect != null) rainEffect.Play();
        if (rainAudio != null) rainAudio.Play();

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

        float timeSinceRainStart = Time.time - rainStartTime;
        yield return new WaitForSeconds(Mathf.Max(0, terrainTriggerDelay - timeSinceRainStart));

        if (terrainDetailExpander != null && orbTransform != null)
            terrainDetailExpander.StartDetailExpansion(orbTransform);

        float totalTime = Time.time - rainStartTime;
        yield return new WaitForSeconds(Mathf.Max(0, rainDuration - totalTime));

        if (rainEffect != null) rainEffect.Stop();
        if (rainAudio != null) rainAudio.Stop();
    }
}
