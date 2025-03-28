using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    public GameObject spikePrefab;
    public Transform player;
    public float spawnRadius = 5f;
    public int minSpikes = 3;
    public int maxSpikes = 8;
    public float minScale = 0.8f;
    public float maxScale = 1.5f;
    public float spawnDepth = 2f; // 从地面下多深生成
    public AudioClip spawnSFX;
    public float maxTiltAngle = 15f;

    public void SpawnSpikes()
    {
        int spikeCount = Random.Range(minSpikes, maxSpikes + 1);

        for (int i = 0; i < spikeCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 basePos = new Vector3(
                player.position.x + offset2D.x,
                player.position.y,
                player.position.z + offset2D.y
            );

            // 生成位置偏移到地下
            Vector3 spawnPos = basePos + Vector3.down * spawnDepth;

            // 随机旋转
            float rx = Random.Range(-maxTiltAngle, maxTiltAngle);
            float ry = Random.Range(0f, 360f);
            float rz = Random.Range(-maxTiltAngle, maxTiltAngle);
            Quaternion rot = Quaternion.Euler(rx, ry, rz);

            GameObject spike = Instantiate(spikePrefab, spawnPos, rot);

            float scale = Random.Range(minScale, maxScale);
            spike.transform.localScale = new Vector3(scale, scale, scale);

            // 播放音效（挂 AudioSource 临时播放）
            if (spawnSFX)
            {
                AudioSource.PlayClipAtPoint(spawnSFX, basePos);
            }
        }
    }
}
