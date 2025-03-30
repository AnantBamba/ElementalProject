using UnityEngine;

public class TerrainReplaceToGrass : MonoBehaviour
{
    public Terrain terrain;
    public int targetLayer = 5; // 草地层索引
    public int fromLayer = 2;   // 雪地层索引
    public float fadeSpeed = 0.01f;
    public float updateInterval = 0.05f;
    public float expansionSpeed = 5f;
    public float maxDistance = 100f;
    public Transform centerTarget; // 可指定 Terrain 子物体作为扩散中心

    private Vector3 worldCenter;
    private float[,,] originalAlphamaps;
    private float[,,] workingAlphamaps;
    private int width;
    private int height;
    private int layers;

    private float currentRadius = 0f;
    private float terrainWidth;
    private float terrainHeight;
    private Vector3 terrainPos;

    void Start()
    {
        if (!terrain) terrain = GetComponent<Terrain>();

        width = terrain.terrainData.alphamapWidth;
        height = terrain.terrainData.alphamapHeight;
        layers = terrain.terrainData.alphamapLayers;

        terrainWidth = terrain.terrainData.size.x;
        terrainHeight = terrain.terrainData.size.z;
        terrainPos = terrain.transform.position;

        originalAlphamaps = terrain.terrainData.GetAlphamaps(0, 0, width, height);
        workingAlphamaps = (float[,,])originalAlphamaps.Clone();

        // 设置扩散中心
        if (centerTarget != null)
        {
            worldCenter = centerTarget.position;
        }

        currentRadius = 0f;
        InvokeRepeating(nameof(StepFadeToGrass), 0f, updateInterval);
    }

    void StepFadeToGrass()
    {
        currentRadius += expansionSpeed;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(
                    x / (float)width * terrainWidth + terrainPos.x,
                    0,
                    y / (float)height * terrainHeight + terrainPos.z);

                float distance = Vector3.Distance(worldCenter, worldPos);
                if (distance > currentRadius || distance > maxDistance)
                    continue;

                float snow = workingAlphamaps[x, y, fromLayer];
                if (snow > 0f)
                {
                    float delta = Mathf.Min(fadeSpeed, snow);
                    workingAlphamaps[x, y, fromLayer] -= delta;
                    workingAlphamaps[x, y, targetLayer] += delta;

                    float total = 0f;
                    for (int l = 0; l < layers; l++) total += workingAlphamaps[x, y, l];
                    for (int l = 0; l < layers; l++) workingAlphamaps[x, y, l] /= total;
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, workingAlphamaps);
        terrain.Flush();

        if (currentRadius > maxDistance)
        {
            CancelInvoke(nameof(StepFadeToGrass));
        }
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && originalAlphamaps != null)
            return;
#endif
        ResetToOriginal();
    }

    void OnApplicationQuit()
    {
        ResetToOriginal();
    }

    void ResetToOriginal()
    {
        if (originalAlphamaps != null && terrain != null)
        {
            terrain.terrainData.SetAlphamaps(0, 0, originalAlphamaps);
            terrain.Flush();
        }
    }
}
