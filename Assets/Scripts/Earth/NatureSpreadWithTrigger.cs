using UnityEngine;

[System.Serializable]
public class DetailLayerSettings
{
    public int layerIndex;
    public float radiusOffset = 0;
    public int density = 1;
}

public class NatureSpreadWithTrigger : MonoBehaviour
{
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

    void Start()
    {
        if (!Application.isPlaying || terrain == null) return;

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

    void Update()
    {
        if (!Application.isPlaying || !spreading || !initialized || spreadCenterObject == null) return;

        if (currentRadius < maxRadius)
        {
            currentRadius += spreadSpeed * Time.deltaTime;
            UpdateAllDetails();
        }
    }

    void UpdateAllDetails()
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

    public void TriggerSpread(Transform orbTransform)
    {
        spreadCenterObject = orbTransform;
        spreading = true;
    }

    void OnDisable()
    {
        if (!Application.isPlaying || !initialized) return;

        int res = terrainData.detailResolution;
        for (int i = 0; i < originalDetails.Length; i++)
        {
            terrainData.SetDetailLayer(0, 0, i, originalDetails[i]);
        }
    }

    void OnDrawGizmos()
    {
        if (spreadCenterObject == null || !spreading) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawSphere(spreadCenterObject.position, currentRadius);
    }
}
