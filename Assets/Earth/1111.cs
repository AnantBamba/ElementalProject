using UnityEngine;

public class SpikeRise : MonoBehaviour
{
    public float riseHeight = 2f;     // 要升高的距离
    public float riseDuration = 0.5f; // 升起用的时间

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer = 0f;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;
    }

    void Update()
    {
        if (timer < riseDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / riseDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
        }
    }
}
