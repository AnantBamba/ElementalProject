using UnityEngine;

[ExecuteInEditMode]
public class TerrainDebugTool : MonoBehaviour
{
    public Terrain terrain;
    public int sampleX = 0;
    public int sampleZ = 0;

    void Start()
    {
        if (!terrain) terrain = GetComponent<Terrain>();

        DebugTerrainLayers();
        DebugAlphamap(sampleX, sampleZ);
    }

    public void DebugTerrainLayers()
    {
        if (!terrain) return;

        var layers = terrain.terrainData.terrainLayers;
        for (int i = 0; i < layers.Length; i++)
        {
            string texName = layers[i].diffuseTexture ? layers[i].diffuseTexture.name : "(null)";
            Debug.Log($"[Layer {i}] Name: {layers[i].name}, Diffuse: {texName}");
        }
    }

    public void DebugAlphamap(int x, int z)
    {
        if (!terrain) return;

        int width = terrain.terrainData.alphamapWidth;
        int height = terrain.terrainData.alphamapHeight;
        int layers = terrain.terrainData.alphamapLayers;

        if (x < 0 || x >= width || z < 0 || z >= height)
        {
            Debug.LogWarning("Sample position out of range!");
            return;
        }

        float[,,] map = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

        Debug.Log($"\n--- Alphamap at ({x},{z}) ---");
        for (int i = 0; i < layers; i++)
        {
            Debug.Log($"Layer {i} Weight: {map[0, 0, i]:F3}");
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 180, 30), "Print Layer Info"))
        {
            DebugTerrainLayers();
        }

        if (GUI.Button(new Rect(10, 50, 180, 30), $"Sample Alpha ({sampleX},{sampleZ})"))
        {
            DebugAlphamap(sampleX, sampleZ);
        }
    }
#endif
}
