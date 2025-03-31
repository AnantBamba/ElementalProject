using UnityEngine;
using System.Collections;
using Sydewa;
using OculusSampleFramework;

public class FireOrbTrigger : MonoBehaviour
{
    public bool isFireOrbPlaced = false;

    [SerializeField] private LightingManager lightingManager;
    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainLayer initialTerrainLayer; // Snow terrain layer
    [SerializeField] private TerrainLayer fireTerrainLayer; // Fire terrain layer
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

        // Ensure the terrain layers are set up correctly at the start
        SetUpInitialTerrainLayers();
    }

    private void SetUpInitialTerrainLayers()
    {
        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;
            TerrainLayer[] layers = terrainData.terrainLayers;

            // Ensure there are at least 7 layers, and swap snow and fire layers
            if (layers.Length < 7) // We need at least 7 layers to have fire at index 6
            {
                Debug.LogError("Terrain does not have enough layers. Adding missing layers...");
                TerrainLayer[] newLayers = new TerrainLayer[7];
                newLayers[2] = initialTerrainLayer; // Snow layer at index 2
                newLayers[6] = fireTerrainLayer; // Fire layer at index 6
                terrainData.terrainLayers = newLayers;
            }
            else
            {
                // Ensure fire layer is at index 6 and snow layer is at index 2
                if (layers[6] != fireTerrainLayer)
                {
                    layers[6] = fireTerrainLayer;
                    Debug.Log("Fire terrain layer set at index 6.");
                }

                if (layers[2] != initialTerrainLayer)
                {
                    layers[2] = initialTerrainLayer;
                    Debug.Log("Snow terrain layer set at index 2.");
                }

                terrainData.terrainLayers = layers;
                terrain.terrainData.RefreshPrototypes(); // Refresh texture prototypes
                terrain.Flush(); // Force terrain to update

                Debug.Log("Terrain layers initialized.");
            }
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
            SwapTerrainLayers();  // Swap the terrain layers when orb is removed
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
        SwapTerrainLayers();
        StartColorTransition(fireOrbMountainColor);
    }

    private void SwapTerrainLayers()
    {
        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;
            TerrainLayer[] layers = terrainData.terrainLayers;

            if (layers.Length > 6) // Ensure there are at least 7 layers
            {
                // Swap the snow layer (index 2) and fire layer (index 6)
                TerrainLayer tempLayer = layers[2];
                layers[2] = layers[6];
                layers[6] = tempLayer;

                terrainData.terrainLayers = layers;
                terrain.terrainData.RefreshPrototypes(); // Refresh texture prototypes
                terrain.Flush(); // Force terrain to update

                Debug.Log("Terrain layers successfully swapped between snow (index 2) and fire (index 6).");
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
}
