using UnityEngine;

public class TerrainTextureBlender : MonoBehaviour
{
    public Terrain terrain;
    public int snowLayerIndex = 2;
    public int grassLayerIndex = 5;
    public float fadeSpeed = 0.01f;
    public float updateInterval = 0.1f;

    [Header("Gradient Spread Settings")]
    public Vector3 worldCenterPosition = Vector3.zero;
    public float maxRadius = 30f;
    public float radiusGrowSpeed = 5f;

    private float currentRadius = 0f;
    private float[,,] originalAlphamaps;
    private int alphamapWidth;
    private int alphamapHeight;
    private int alphamapLayers;

    private bool isBlending = false;

    void Start()
    {
        if (!terrain) terrain = GetComponent<Terrain>();

        alphamapWidth = terrain.terrainData.alphamapWidth;
        alphamapHeight = terrain.terrainData.alphamapHeight;
        alphamapLayers = terrain.terrainData.alphamapLayers;

        originalAlphamaps = terrain.terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        EnsureGrassLayerPresence();
    }

    public void StartGrassBlend()
    {
        if (!isBlending)
        {
            currentRadius = 0f;
            isBlending = true;
            InvokeRepeating("BlendToGrassWithRadius", 0f, updateInterval);
        }
    }

    public void ResetToSnow()
    {
        if (isBlending)
        {
            CancelInvoke("BlendToGrassWithRadius");
            isBlending = false;
        }
        terrain.terrainData.SetAlphamaps(0, 0, originalAlphamaps);
        terrain.Flush();
    }

    private void BlendToGrassWithRadius()
    {
        float[,,] alphamaps = terrain.terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        Vector3 terrainSize = terrain.terrainData.size;

        currentRadius += radiusGrowSpeed * updateInterval;
        bool finished = currentRadius >= maxRadius;

        for (int x = 0; x < alphamapWidth; x++)
        {
            for (int z = 0; z < alphamapHeight; z++)
            {
                float worldX = (float)x / (float)alphamapWidth * terrainSize.x + terrain.transform.position.x;
                float worldZ = (float)z / (float)alphamapHeight * terrainSize.z + terrain.transform.position.z;

                float dist = Vector2.Distance(new Vector2(worldX, worldZ), new Vector2(worldCenterPosition.x, worldCenterPosition.z));
                if (dist <= currentRadius)
                {
                    float snow = alphamaps[x, z, snowLayerIndex];
                    float grass = alphamaps[x, z, grassLayerIndex];

                    if (snow > 0f)
                    {
                        float delta = Mathf.Min(fadeSpeed, snow);
                        alphamaps[x, z, snowLayerIndex] -= delta;
                        alphamaps[x, z, grassLayerIndex] += delta;
                    }

                    float total = 0f;
                    for (int l = 0; l < alphamapLayers; l++)
                        total += alphamaps[x, z, l];
                    for (int l = 0; l < alphamapLayers; l++)
                        alphamaps[x, z, l] /= total;
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
        terrain.Flush();

        if (finished)
        {
            CancelInvoke("BlendToGrassWithRadius");
            isBlending = false;
        }
    }

    private void EnsureGrassLayerPresence()
    {
        float[,,] map = terrain.terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        for (int x = 0; x < alphamapWidth; x++)
        {
            for (int z = 0; z < alphamapHeight; z++)
            {
                if (map[x, z, grassLayerIndex] <= 0f)
                {
                    map[x, z, grassLayerIndex] = 0.001f;
                    float total = 0f;
                    for (int l = 0; l < alphamapLayers; l++)
                        total += map[x, z, l];
                    for (int l = 0; l < alphamapLayers; l++)
                        map[x, z, l] /= total;
                }
            }
        }
        terrain.terrainData.SetAlphamaps(0, 0, map);
        terrain.Flush();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 180, 30), "Start Grass Blend"))
        {
            StartGrassBlend();
        }

        if (GUI.Button(new Rect(10, 50, 180, 30), "Reset to Snow"))
        {
            ResetToSnow();
        }
    }
#endif
}