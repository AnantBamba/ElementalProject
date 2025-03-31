using UnityEngine;
using System.Collections.Generic;

public class FootstepAudioSwitcherTerrain : MonoBehaviour
{
    [System.Serializable]
    public class TerrainFootstep
    {
        public int terrainLayerIndex;    // 地形图层编号，例如 2 = 雪，5 = 草
        public AudioClip footstepClip;   // 对应音效
    }

    public AudioSource audioSource;
    public CharacterController characterController;
    public Terrain terrain;

    [Header("Footstep Clips Mapping")]
    public TerrainFootstep[] footstepMappings;

    public float movementThreshold = 0.1f;
    private bool isMoving = false;
    private int currentLayerIndex = -1;
    private Dictionary<int, AudioClip> layerToClip;

    void Start()
    {
        // 构建映射字典
        layerToClip = new Dictionary<int, AudioClip>();
        foreach (var mapping in footstepMappings)
        {
            if (!layerToClip.ContainsKey(mapping.terrainLayerIndex))
                layerToClip.Add(mapping.terrainLayerIndex, mapping.footstepClip);
        }
    }

    void Update()
    {
        if (terrain == null || characterController == null || audioSource == null) return;

        bool movingNow = characterController.velocity.magnitude > movementThreshold;

        int terrainIndex = GetMainTextureIndexUnderFoot();

        // 切换音效
        if (terrainIndex != currentLayerIndex)
        {
            currentLayerIndex = terrainIndex;
            UpdateFootstepClip(terrainIndex);
        }

        if (movingNow && !isMoving)
        {
            if (audioSource.clip != null) audioSource.Play();
        }
        else if (!movingNow && isMoving)
        {
            audioSource.Stop();
        }

        isMoving = movingNow;
    }

    void UpdateFootstepClip(int terrainIndex)
    {
        if (layerToClip.ContainsKey(terrainIndex))
        {
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.clip = layerToClip[terrainIndex];
            audioSource.loop = true;
        }
        else
        {
            audioSource.Stop(); // 未知地形就别播放脚步声
            audioSource.clip = null;
        }
    }

    int GetMainTextureIndexUnderFoot()
    {
        Vector3 pos = transform.position;
        Vector3 terrainPos = terrain.transform.position;
        TerrainData tData = terrain.terrainData;

        int mapX = (int)(((pos.x - terrainPos.x) / tData.size.x) * tData.alphamapWidth);
        int mapZ = (int)(((pos.z - terrainPos.z) / tData.size.z) * tData.alphamapHeight);

        float[,,] splatmapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);

        int maxIndex = 0;
        float maxMix = 0f;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxMix = splatmapData[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}
