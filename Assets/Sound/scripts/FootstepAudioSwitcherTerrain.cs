using UnityEngine;

public class FootstepAudioSwitcherTerrain : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Footstep Clips by Terrain Index")]
    public AudioClip[] footstepClips; // Index 0 = grass, 1 = snow, etc.

    [Header("References")]
    public CharacterController characterController;
    public Terrain terrain;

    public float movementThreshold = 0.1f;
    private bool isMoving = false;
    private int currentTerrainIndex = -1;

    void Update()
    {
        if (terrain == null || audioSource == null || characterController == null) return;

        bool movingNow = characterController.velocity.magnitude > movementThreshold;

        int terrainIndex = GetMainTextureIndexUnderFoot();
        if (terrainIndex != currentTerrainIndex)
        {
            currentTerrainIndex = terrainIndex;
            UpdateFootstepClip(terrainIndex);
        }

        if (movingNow && !isMoving)
        {
            audioSource.Play();
        }
        else if (!movingNow && isMoving)
        {
            audioSource.Stop();
        }

        isMoving = movingNow;
    }

    void UpdateFootstepClip(int index)
    {
        if (index < 0 || index >= footstepClips.Length) return;

        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = footstepClips[index];
        audioSource.loop = true;
    }

    int GetMainTextureIndexUnderFoot()
    {
        Vector3 playerPos = transform.position;
        Vector3 terrainPos = terrain.transform.position;
        TerrainData tData = terrain.terrainData;

        int mapX = (int)(((playerPos.x - terrainPos.x) / tData.size.x) * tData.alphamapWidth);
        int mapZ = (int)(((playerPos.z - terrainPos.z) / tData.size.z) * tData.alphamapHeight);

        float[,,] splatmapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);

        int maxIndex = 0;
        float maxMix = 0;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxIndex = i;
                maxMix = splatmapData[0, 0, i];
            }
        }

        return maxIndex;
    }
}
