using UnityEngine;
using System.Collections;
using Sydewa;
using OculusSampleFramework;

public class FireOrbTrigger : MonoBehaviour
{
    public bool isFireOrbPlaced = false;

    [SerializeField] private LightingManager lightingManager;
    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainLayer initialTerrainLayer; // Terrain at the start and after orb removal
    [SerializeField] private TerrainLayer fireTerrainLayer; // Terrain when orb is placed
    [SerializeField] private Material mountainMaterial;
    [SerializeField] private Color initialMountainColor = Color.gray;
    [SerializeField] private Color fireOrbMountainColor = Color.red;

    public float transitionSpeed = 2.0f;
    private Coroutine timeChangeCoroutine;
    private Coroutine colorChangeCoroutine;

    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        if (lightingManager == null)
        {
            lightingManager = FindObjectOfType<LightingManager>();
            if (lightingManager == null)
            {
                Debug.LogError("LightingManager not found in the scene!");
            }
        }

        if (terrain == null)
        {
            terrain = FindObjectOfType<Terrain>();
            if (terrain == null)
            {
                Debug.LogError("Terrain not found in the scene!");
            }
        }

        if (mountainMaterial != null)
        {
            mountainMaterial.color = initialMountainColor;
        }

        // Ensure the terrain layer is the initial layer at the start of the scene
        if (!IsTerrainLayerMatching(initialTerrainLayer))
        {
            SwitchTerrainLayerToInitial();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireOrb"))
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
        if (other.CompareTag("FireOrb"))
        {
            ResetTime();
            SwitchTerrainLayerToInitial();  // Switch back to the initial terrain layer
            StartColorTransition(initialMountainColor);   // Smoothly transition back to the initial mountain color
        }
    }

    private void ResetTime()
    {
        isFireOrbPlaced = false;
        Debug.Log("Fire Orb has been removed from the Altar!");
        StartTransition(lightingManager.StartTime);
    }

    private void StartTransition(float targetTime)
    {
        if (timeChangeCoroutine != null)
        {
            StopCoroutine(timeChangeCoroutine);
        }
        timeChangeCoroutine = StartCoroutine(ChangeTimeOfDay(targetTime));
    }

    private IEnumerator ChangeTimeOfDay(float targetTime)
    {
        if (lightingManager == null)
        {
            yield break;
        }

        while (Mathf.Abs(lightingManager.TimeOfDay - targetTime) > 0.01f)
        {
            lightingManager.TimeOfDay = Mathf.MoveTowards(lightingManager.TimeOfDay, targetTime, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        lightingManager.TimeOfDay = targetTime;
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
        {
            yield return null;
        }

        if (grabbable.CompareTag("FireOrb"))
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

        isFireOrbPlaced = true;
        Debug.Log("Fire Orb has been placed on the Altar!");
        StartTransition(12f);

        // Switch to the fire terrain layer and the fire orb color when orb is placed
        SwitchTerrainLayerToFire();
        StartColorTransition(fireOrbMountainColor);
    }

    private void SwitchTerrainLayerToFire()
    {
        if (terrain != null)
        {
            TerrainLayer[] layers = terrain.terrainData.terrainLayers;
            if (layers.Length > 0)
            {
                layers[0] = fireTerrainLayer;
                terrain.terrainData.terrainLayers = layers;

                Debug.Log("Terrain layer switched to fire terrain layer.");
            }
            else
            {
                Debug.LogError("Terrain has no layers assigned.");
            }
        }
    }

    private void SwitchTerrainLayerToInitial()
    {
        if (terrain != null)
        {
            TerrainLayer[] layers = terrain.terrainData.terrainLayers;
            if (layers.Length > 0)
            {
                layers[0] = initialTerrainLayer;
                terrain.terrainData.terrainLayers = layers;

                Debug.Log("Terrain layer switched to initial terrain layer.");
            }
            else
            {
                Debug.LogError("Terrain has no layers assigned.");
            }
        }
    }

    private void StartColorTransition(Color targetColor)
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
        }
        colorChangeCoroutine = StartCoroutine(SmoothColorTransition(targetColor));
    }

    private IEnumerator SmoothColorTransition(Color targetColor)
    {
        Color currentColor = mountainMaterial.color;

        // Smoothly interpolate the color change
        float timeElapsed = 0f;
        while (timeElapsed < transitionSpeed)
        {
            mountainMaterial.color = Color.Lerp(currentColor, targetColor, timeElapsed / transitionSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mountainMaterial.color = targetColor;
    }

    private bool IsTerrainLayerMatching(TerrainLayer targetLayer)
    {
        if (terrain != null && terrain.terrainData.terrainLayers.Length > 0)
        {
            TerrainLayer currentLayer = terrain.terrainData.terrainLayers[0];
            return currentLayer == targetLayer;
        }
        return false;
    }
}
