using UnityEngine;

[System.Serializable]
public class DetailLayerSetting
{
    public int layerIndex;
    public float radiusOffset = 0f;
    public int density = 1;
}

public class TerrainDetailExpander : MonoBehaviour
{
    [Header("Terrain Setup")]
    public Terrain terrain;
    public Transform centerObject;

    [Header("Expansion Settings")]
    public float spreadSpeed = 5f;
    public float maxRadius = 50f;
    public DetailLayerSetting[] detailLayers;

    private float currentRadius = 0f;
    private TerrainData terrainData;
    private int[][,] originalDetailData;
    private bool initialized = false;
    private bool isExpanding = false;

    void Start()
    {
        if (terrain == null) return;

        terrainData = terrain.terrainData;
        int layerCount = terrainData.detailPrototypes.Length;
        int res = terrainData.detailResolution;
        originalDetailData = new int[layerCount][,];

        for (int i = 0; i < layerCount; i++)
        {
            originalDetailData[i] = terrainData.GetDetailLayer(0, 0, res, res, i);
        }

        initialized = true;
    }

    void Update()
    {
        if (!isExpanding || !initialized || centerObject == null) return;

        if (currentRadius < maxRadius)
        {
            currentRadius += spreadSpeed * Time.deltaTime;
            UpdateDetailLayers();
        }
    }

    void UpdateDetailLayers()
    {
        int res = terrainData.detailResolution;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 origin = centerObject.position;

        foreach (var layer in detailLayers)
        {
            if (layer.layerIndex >= terrainData.detailPrototypes.Length) continue;

            int[,] detailMap = terrainData.GetDetailLayer(0, 0, res, res, layer.layerIndex);

            for (int x = 0; x < res; x++)
            {
                for (int y = 0; y < res; y++)
                {
                    float worldX = ((float)x / res) * terrainData.size.x + terrainPos.x;
                    float worldZ = ((float)y / res) * terrainData.size.z + terrainPos.z;

                    float dist = Vector2.Distance(new Vector2(worldX, worldZ), new Vector2(origin.x, origin.z));
                    if (dist < currentRadius - layer.radiusOffset)
                    {
                        detailMap[y, x] = layer.density;
                    }
                }
            }

            terrainData.SetDetailLayer(0, 0, layer.layerIndex, detailMap);
        }
    }

    public void StartDetailExpansion(Transform center = null)
    {
        if (center != null) centerObject = center;
        currentRadius = 0f;
        isExpanding = true;
    }

    void OnDisable()
    {
        if (!Application.isPlaying || !initialized) return;

        int res = terrainData.detailResolution;
        for (int i = 0; i < originalDetailData.Length; i++)
        {
            terrainData.SetDetailLayer(0, 0, i, originalDetailData[i]);
        }
    }

    void OnDrawGizmos()
    {
        if (!isExpanding || centerObject == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawSphere(centerObject.position, currentRadius);
    }
}
