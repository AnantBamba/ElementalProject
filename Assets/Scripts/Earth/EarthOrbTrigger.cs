using UnityEngine;
using System.Collections;
using OculusSampleFramework;

public class EarthOrbTrigger : MonoBehaviour
{
    public bool isEarthOrbPlaced = false;

    [Header("Orb Snapping")]
    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    [Header("Nature Spread with Trigger Settings")]
    public Terrain terrain;
    public Transform spreadCenterObject;
    public float spreadSpeed = 5f;
    public float maxRadius = 50f;
    public DetailLayerSettings[] detailLayers;

    private float currentRadius = 0f;
    private TerrainData terrainData;
    private int[][,] originalDetails;
    private bool initialized = false;
    private bool spreading = false;
    private bool hasActivated = false;
    private Transform orbTransform;

    private void Start()
    {
        if (terrain == null || !Application.isPlaying) return;

        terrainData = terrain.terrainData;
        int layerCount = terrainData.detailPrototypes.Length;
        int res = terrainData.detailResolution;
        originalDetails = new int[layerCount][,];

        for (int i = 0; i < layerCount; i++)
        {
            originalDetails[i] = terrainData.GetDetailLayer(0, 0, res, res, i);
        }

        initialized = true;
    }

    private void Update()
    {
        if (!Application.isPlaying || !spreading || !initialized || spreadCenterObject == null) return;

        if (currentRadius < maxRadius)
        {
            currentRadius += spreadSpeed * Time.deltaTime;
            UpdateAllDetails();
        }
    }

    private void UpdateAllDetails()
    {
        int detailRes = terrainData.detailResolution;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 spreadOrigin = spreadCenterObject.position;

        foreach (var layer in detailLayers)
        {
            if (layer.layerIndex >= terrainData.detailPrototypes.Length) continue;

            int[,] detailMap = terrainData.GetDetailLayer(0, 0, detailRes, detailRes, layer.layerIndex);

            for (int x = 0; x < detailRes; x++)
            {
                for (int y = 0; y < detailRes; y++)
                {
                    float worldX = ((float)x / detailRes) * terrainData.size.x + terrainPos.x;
                    float worldZ = ((float)y / detailRes) * terrainData.size.z + terrainPos.z;

                    if (Vector2.Distance(new Vector2(worldX, worldZ), new Vector2(spreadOrigin.x, spreadOrigin.z)) < currentRadius - layer.radiusOffset)
                    {
                        detailMap[y, x] = layer.density;
                    }
                }
            }

            terrainData.SetDetailLayer(0, 0, layer.layerIndex, detailMap);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EarthOrb") && !hasActivated)
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
        if (other.CompareTag("EarthOrb"))
        {
            Debug.Log("Earth Orb has been removed from the Altar!");
            isEarthOrbPlaced = false;
            hasActivated = false; // Allow the orb to resnap again
            if (spreading)
            {
                StopSpreading();
            }
        }
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed) yield return null;
        if (grabbable.CompareTag("EarthOrb"))
        {
            SnapOrbToAltar(grabbable.gameObject);
        }
    }

    private void SnapOrbToAltar(GameObject orb)
    {
        orb.transform.position = transform.TransformPoint(localSnapPosition);
        orb.transform.rotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        var rb = orb.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isEarthOrbPlaced = true;
        hasActivated = true;
        orbTransform = orb.transform;

        // Start spreading nature when orb is placed
        StartSpreading(orbTransform);
    }

    private void StartSpreading(Transform orbTransform)
    {
        spreadCenterObject = orbTransform;
        spreading = true;
    }

    private void StopSpreading()
    {
        spreading = false;
        currentRadius = 0f; // Reset radius
        // Optionally, you could reset terrain detail layers to their original state
        ResetTerrainDetails();
    }

    private void ResetTerrainDetails()
    {
        if (!Application.isPlaying || !initialized) return;

        int res = terrainData.detailResolution;
        for (int i = 0; i < originalDetails.Length; i++)
        {
            terrainData.SetDetailLayer(0, 0, i, originalDetails[i]);
        }
    }

    private void OnDisable()
    {
        ResetTerrainDetails();
    }

    private void OnDrawGizmos()
    {
        if (spreadCenterObject == null || !spreading) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawSphere(spreadCenterObject.position, currentRadius);
    }
}
