using UnityEngine;

public class GerstnerWaves : MonoBehaviour
{
    public int waveCount = 4; // Number of wave layers
    public float waveSpeed = 2f;
    public float waveHeight = 0.5f;
    public float waveLength = 5f;
    public Vector2[] waveDirections;

    private MeshFilter meshFilter;
    private Vector3[] originalVertices;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;
        if (waveDirections.Length == 0)
        {
            waveDirections = new Vector2[waveCount];
            for (int i = 0; i < waveCount; i++)
                waveDirections[i] = Random.insideUnitCircle.normalized; // Random wave directions
        }
    }

    void Update()
    {
        ApplyWaves();
    }

    void ApplyWaves()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vert = originalVertices[i];
            float yOffset = 0;

            for (int j = 0; j < waveCount; j++)
            {
                float frequency = 2 * Mathf.PI / waveLength;
                float phase = waveSpeed * Time.time;
                yOffset += waveHeight / waveCount * Mathf.Sin(Vector2.Dot(waveDirections[j], new Vector2(vert.x, vert.z)) * frequency + phase);
            }

            vertices[i] = new Vector3(vert.x, yOffset, vert.z);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
