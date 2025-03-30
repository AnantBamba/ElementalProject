using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using OculusSampleFramework;
using Sydewa;

[ExecuteInEditMode]
public class FireOrbTrigger1 : MonoBehaviour
{
    public bool isFireOrbPlaced = false;

    [Header("Managers")]
    [SerializeField] private LightingManager lightingManager;
    [SerializeField] private Terrain terrain;

    [Header("Terrain Blending Settings")]
    [SerializeField] private int snowLayerIndex = 2;
    [SerializeField] private int grassLayerIndex = 5;
    [SerializeField] private float fadeSpeed = 0.05f;

    [Header("Mountain Material Swap Settings")]
    [SerializeField] private Renderer[] mountainRenderers;
    [SerializeField] private Material initialMountainMaterial;
    [SerializeField] private Material fireOrbMountainMaterial;
    private Material[] originalMaterials;

    [Header("Expansion Settings")]
    [SerializeField] private float expansionRadius = 0f;
    [SerializeField] private float maxExpansionRadius = 10f;
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private LayerMask affectedLayerMask;
    [SerializeField] private GameObject expansionVisualizer; // Optional: a semi-transparent sphere

    [Header("Orb Snap Settings")]
    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private Coroutine expansionCoroutine;
    private TerrainLayer[] originalLayers;

    private void Start()
    {
        if (!Application.isPlaying) return;

        if (lightingManager == null)
            lightingManager = FindObjectOfType<LightingManager>();

        if (terrain == null)
            terrain = FindObjectOfType<Terrain>();

        if (terrain != null)
            originalLayers = terrain.terrainData.terrainLayers.Clone() as TerrainLayer[];

        if (mountainRenderers != null && mountainRenderers.Length > 0)
        {
            originalMaterials = new Material[mountainRenderers.Length];
            for (int i = 0; i < mountainRenderers.Length; i++)
            {
                originalMaterials[i] = mountainRenderers[i].sharedMaterial;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireOrb"))
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
        if (other.CompareTag("FireOrb"))
        {
            ResetEnvironment();
        }
    }

    private void ResetEnvironment()
    {
        isFireOrbPlaced = false;
        Debug.Log("Fire Orb has been removed. Restoring environment.");
        if (lightingManager) lightingManager.TimeOfDay = lightingManager.StartTime;

        if (terrain && originalLayers != null)
            terrain.terrainData.terrainLayers = originalLayers;

        if (mountainRenderers != null && originalMaterials != null)
        {
            for (int i = 0; i < mountainRenderers.Length; i++)
            {
                mountainRenderers[i].sharedMaterial = originalMaterials[i];
            }
        }

        if (expansionVisualizer)
            expansionVisualizer.SetActive(false);
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
            yield return null;

        if (grabbable.CompareTag("FireOrb"))
            SnapOrbToAltar(grabbable.gameObject);
    }

    private void SnapOrbToAltar(GameObject orb)
    {
        Vector3 worldSnapPosition = transform.TransformPoint(localSnapPosition);
        Quaternion worldSnapRotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        orb.transform.position = worldSnapPosition;
        orb.transform.rotation = worldSnapRotation;

        Rigidbody rb = orb.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        isFireOrbPlaced = true;
        Debug.Log("Fire Orb placed on Altar.");

        if (lightingManager) lightingManager.TimeOfDay = 12f;

        if (expansionCoroutine != null) StopCoroutine(expansionCoroutine);
        expansionCoroutine = StartCoroutine(ExpansionEffect());
    }

    private IEnumerator ExpansionEffect()
    {
        if (expansionVisualizer)
        {
            expansionVisualizer.transform.position = transform.position;
            expansionVisualizer.SetActive(true);
        }

        expansionRadius = 0f;
        while (expansionRadius < maxExpansionRadius)
        {
            expansionRadius += Time.deltaTime * expansionSpeed;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, expansionRadius, affectedLayerMask);
            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Terrain") && terrain != null)
                {
                    ApplyTerrainGradient(col.transform.position);
                }
            }
            if (expansionVisualizer)
                expansionVisualizer.transform.localScale = Vector3.one * expansionRadius * 2f;

            yield return null;
        }

        if (mountainRenderers != null)
        {
            foreach (Renderer r in mountainRenderers)
            {
                if (fireOrbMountainMaterial)
                    r.sharedMaterial = fireOrbMountainMaterial;
            }
        }
    }

    private void ApplyTerrainGradient(Vector3 worldPos)
    {
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(worldPos);

        int mapX = Mathf.FloorToInt((terrainPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / terrainData.size.z) * terrainData.alphamapHeight);

        int size = 5;
        mapX = Mathf.Clamp(mapX, 0, terrainData.alphamapWidth - size);
        mapZ = Mathf.Clamp(mapZ, 0, terrainData.alphamapHeight - size);

        float[,,] alphamaps = terrainData.GetAlphamaps(mapX, mapZ, size, size);

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float snow = alphamaps[x, z, snowLayerIndex];
                float grass = alphamaps[x, z, grassLayerIndex];

                float delta = Mathf.Min(fadeSpeed, snow);
                alphamaps[x, z, snowLayerIndex] = snow - delta;
                alphamaps[x, z, grassLayerIndex] = grass + delta;

                // Optional: Normalize
                float total = 0f;
                for (int l = 0; l < terrainData.alphamapLayers; l++)
                    total += alphamaps[x, z, l];

                for (int l = 0; l < terrainData.alphamapLayers; l++)
                    alphamaps[x, z, l] /= total;
            }
        }

        terrainData.SetAlphamaps(mapX, mapZ, alphamaps);
        terrain.Flush();
    }
}
