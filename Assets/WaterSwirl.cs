using UnityEngine;

public class WaterSwirl : MonoBehaviour
{
    public Material orbMaterial;
    public float speed = 0.1f;

    void Update()
    {
        float offset = Time.time * speed;
        orbMaterial.SetTextureOffset("_MainTex", new Vector2(offset, offset));
    }
}