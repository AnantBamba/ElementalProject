using UnityEngine;

public class FireTerrainChange : MonoBehaviour
{
    public Terrain terrain;   // Target Terrain
    public Transform fireOrb; // Fire Element Orb
    public float radius = 10f; // Effect radius
    public float restoreSpeed = 0.1f; // Speed at which terrain restores to snow

    private TerrainData terrainData;
    private int splatMapWidth, splatMapHeight;
    private float[,,] splatMapData;
    private float[,,] originalSplatMapData; // Backup of the original terrain texture

    private Vector2Int lastAffectedPosition = new Vector2Int(-1, -1); // Last affected terrain position

    void Start()
    {
        if (terrain == null || fireOrb == null)
        {
            Debug.LogError("🚨 Please assign both Terrain and FireOrb in FireTerrainChange component!");
            return;
        }

        terrainData = terrain.terrainData;
        splatMapWidth = terrainData.alphamapWidth;
        splatMapHeight = terrainData.alphamapHeight;

        splatMapData = terrainData.GetAlphamaps(0, 0, splatMapWidth, splatMapHeight);
        originalSplatMapData = terrainData.GetAlphamaps(0, 0, splatMapWidth, splatMapHeight);
    }

    void Update()
    {
        if (fireOrb != null)
        {
            MoveEffectWithFireOrb();
        }
    }

    void MoveEffectWithFireOrb()
    {
        // Get FireOrb's local position within the Terrain (ignore Y-axis)
        Vector3 localPos = fireOrb.localPosition;
        Vector3 terrainSize = terrainData.size;

        // 🚀 **Fix coordinate mapping: FireOrb X affects SplatMap Z, FireOrb Z affects SplatMap X**
        float normX = localPos.z / terrainSize.z; // FireOrb Z corresponds to SplatMap X
        float normZ = localPos.x / terrainSize.x; // FireOrb X corresponds to SplatMap Z

        // 🚀 Compute the corresponding SplatMap indices
        int x = Mathf.RoundToInt(normX * (splatMapWidth - 1));
        int z = Mathf.RoundToInt(normZ * (splatMapHeight - 1));
        int radiusInPixels = Mathf.RoundToInt(radius / terrainSize.x * splatMapWidth);

        // 🚨 Ensure the indices are within the valid range
        if (x < 0 || x >= splatMapWidth || z < 0 || z >= splatMapHeight)
        {
            Debug.LogWarning("🚨 FireOrb is out of SplatMap bounds!");
            return;
        }

        // If FireOrb has moved, restore the previous affected area
        if (lastAffectedPosition.x != -1 && lastAffectedPosition.y != -1)
        {
            RestorePreviousTerrain(lastAffectedPosition.x, lastAffectedPosition.y, radiusInPixels);
        }

        // Store the new affected position
        lastAffectedPosition = new Vector2Int(x, z);

        // Apply texture modification to the new position
        ModifyTerrainTexture(x, z, radiusInPixels);
    }

    void ModifyTerrainTexture(int x, int z, int radiusInPixels)
    {
        for (int i = -radiusInPixels; i <= radiusInPixels; i++)
        {
            for (int j = -radiusInPixels; j <= radiusInPixels; j++)
            {
                int newX = x + i;
                int newZ = z + j;

                if (newX >= 0 && newX < splatMapWidth && newZ >= 0 && newZ < splatMapHeight)
                {
                    float dist = Mathf.Sqrt(i * i + j * j);
                    if (dist < radiusInPixels)
                    {
                        float blendFactor = 1 - (dist / radiusInPixels); // Closer to the center → stronger grass effect

                        // Assume Snow = Index 0, Grass = Index 1
                        splatMapData[newX, newZ, 0] = Mathf.Lerp(splatMapData[newX, newZ, 0], 1 - blendFactor, Time.deltaTime * 5);
                        splatMapData[newX, newZ, 1] = Mathf.Lerp(splatMapData[newX, newZ, 1], blendFactor, Time.deltaTime * 5);
                    }
                }
            }
        }

        // Apply the texture modification
        terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    void RestorePreviousTerrain(int x, int z, int radiusInPixels)
    {
        for (int i = -radiusInPixels; i <= radiusInPixels; i++)
        {
            for (int j = -radiusInPixels; j <= radiusInPixels; j++)
            {
                int newX = x + i;
                int newZ = z + j;

                if (newX >= 0 && newX < splatMapWidth && newZ >= 0 && newZ < splatMapHeight)
                {
                    for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                    {
                        splatMapData[newX, newZ, layer] = Mathf.Lerp(splatMapData[newX, newZ, layer], originalSplatMapData[newX, newZ, layer], restoreSpeed * Time.deltaTime);
                    }
                }
            }
        }

        // Apply the restoration
        terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    // 🚀 **Automatically reset the terrain to full snow when exiting Play mode**
    void OnApplicationQuit()
    {
        Debug.Log("🛑 Exiting Play Mode: Resetting terrain to full snow!");

        for (int x = 0; x < splatMapWidth; x++)
        {
            for (int z = 0; z < splatMapHeight; z++)
            {
                splatMapData[x, z, 0] = 1f; // Set full snow
                splatMapData[x, z, 1] = 0f; // Remove grass
            }
        }

        terrainData.SetAlphamaps(0, 0, splatMapData);
    }
}
