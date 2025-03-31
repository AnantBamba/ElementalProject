using UnityEngine;
using System.Collections;
using OculusSampleFramework;
using Sydewa;

public class FireOrbTrigger1 : MonoBehaviour
{
    public bool isFireOrbPlaced = false;

    [Header("Managers")]
    [SerializeField] private LightingManager lightingManager;
    [SerializeField] private TerrainTextureExpander terrainExpander; // 引用新的脚本

    [Header("Mountain Material Swap Settings")]
    [SerializeField] private Renderer[] mountainRenderers;
    [SerializeField] private Material initialMountainMaterial;
    [SerializeField] private Material fireOrbMountainMaterial;
    private Material[] originalMaterials;

    [Header("Orb Snap Settings")]
    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        if (!Application.isPlaying) return;

        if (lightingManager == null)
            lightingManager = FindObjectOfType<LightingManager>();

        if (terrainExpander == null)
            terrainExpander = FindObjectOfType<TerrainTextureExpander>();

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
        if (!Application.isPlaying || other.CompareTag("FireOrb") == false) return;

        OVRGrabbable grabbable = other.GetComponent<OVRGrabbable>();
        if (grabbable != null && grabbable.isGrabbed)
            StartCoroutine(WaitForRelease(grabbable));
        else
            SnapOrbToAltar(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Application.isPlaying || other.CompareTag("FireOrb") == false) return;
        ResetEnvironment();
    }

    private void ResetEnvironment()
    {
        isFireOrbPlaced = false;
        Debug.Log("Fire Orb removed. Restoring environment.");
        if (lightingManager) lightingManager.TimeOfDay = lightingManager.StartTime;

        if (mountainRenderers != null && originalMaterials != null)
        {
            for (int i = 0; i < mountainRenderers.Length; i++)
                mountainRenderers[i].sharedMaterial = originalMaterials[i];
        }
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
        if (!Application.isPlaying) return;

        Vector3 worldSnapPosition = transform.TransformPoint(localSnapPosition);
        Quaternion worldSnapRotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        orb.transform.position = worldSnapPosition;
        orb.transform.rotation = worldSnapRotation;

        Rigidbody rb = orb.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        isFireOrbPlaced = true;
        Debug.Log("Fire Orb placed on Altar.");

        if (lightingManager) lightingManager.TimeOfDay = 12f;

        terrainExpander?.StartExpansion();

        foreach (Renderer r in mountainRenderers)
        {
            if (fireOrbMountainMaterial)
                r.sharedMaterial = fireOrbMountainMaterial;
        }
    }
}