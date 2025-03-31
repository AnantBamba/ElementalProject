using UnityEngine;
using System.Collections;

public class TerrainTextureExpander : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private Transform centerTarget;
    [SerializeField] private int snowLayerIndex = 2;
    [SerializeField] private int grassLayerIndex = 5;
    [SerializeField] private float fadeSpeed = 0.05f;
    [SerializeField] private float maxExpansionRadius = 10f;
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private GameObject expansionVisualizer;

    private Coroutine expansionCoroutine;

    public void StartExpansion()
    {
        if (expansionCoroutine != null) StopCoroutine(expansionCoroutine);
        expansionCoroutine = StartCoroutine(ExpansionEffect());
    }

    private IEnumerator ExpansionEffect()
    {
        float radius = 0f;
        Vector3 center = centerTarget ? centerTarget.position : transform.position;

        if (expansionVisualizer)
        {
            expansionVisualizer.transform.position = center;
            expansionVisualizer.SetActive(true);
        }

        while (radius < maxExpansionRadius)
        {
            radius += Time.deltaTime * expansionSpeed;
            ApplyTerrainGradient(center, radius);

            if (expansionVisualizer)
                expansionVisualizer.transform.localScale = Vector3.one * radius * 2f;

            yield return null;
        }
    }

    private void ApplyTerrainGradient(Vector3 center, float radius)
    {
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float worldX = terrainPos.x + x / (float)width * terrainData.size.x;
                float worldZ = terrainPos.z + z / (float)height * terrainData.size.z;

                if (Vector2.Distance(new Vector2(center.x, center.z), new Vector2(worldX, worldZ)) <= radius)
                {
                    float snow = alphamaps[x, z, snowLayerIndex];
                    float grass = alphamaps[x, z, grassLayerIndex];
                    float delta = Mathf.Min(fadeSpeed, snow);

                    alphamaps[x, z, snowLayerIndex] = snow - delta;
                    alphamaps[x, z, grassLayerIndex] = grass + delta;

                    float total = 0f;
                    for (int l = 0; l < terrainData.alphamapLayers; l++)
                        total += alphamaps[x, z, l];
                    for (int l = 0; l < terrainData.alphamapLayers; l++)
                        alphamaps[x, z, l] /= total;
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamaps);
        terrain.Flush();
    }
}
