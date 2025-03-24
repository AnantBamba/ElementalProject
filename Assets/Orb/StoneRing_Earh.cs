using UnityEngine;

public class StoneRingController : MonoBehaviour
{
    [Header("基础设置")]
    public Transform targetSphere;         // 球体引用
    public GameObject stonePrefab;         // 小石头 prefab
    public int stoneCount = 12;            // 石头数量
    public float radius = 2f;              // 星环半径
    public Vector3 offset = Vector3.zero;  // 星环在球体上的偏移
    public Vector3 fixedEulerAngles = new Vector3(0, 0, 0); // 星环姿态
    public float rotationSpeed = 30f;      // 星环自转速度（度/秒）

    private Transform[] stones;            // 储存生成的石头引用
    private float currentAngle = 0f;       // 当前旋转角度

    void Start()
    {
        GenerateStoneRing();
    }

    void LateUpdate()
    {
        // 星环跟随球体的位置
        transform.position = targetSphere.position + offset;

        // 固定星环姿态（不跟随球体旋转）
        transform.rotation = Quaternion.Euler(fixedEulerAngles);

        // 累加旋转角度
        currentAngle += rotationSpeed * Time.deltaTime;

        // 让每颗石头重新计算在环上的位置
        for (int i = 0; i < stones.Length; i++)
        {
            float angleOffset = i * Mathf.PI * 2f / stoneCount;
            float totalAngle = currentAngle * Mathf.Deg2Rad + angleOffset;

            Vector3 localPos = new Vector3(Mathf.Cos(totalAngle), 0, Mathf.Sin(totalAngle)) * radius;
            stones[i].localPosition = localPos;
            stones[i].LookAt(transform.position); // 可选：朝向球体中心
        }
    }

    void GenerateStoneRing()
    {
        stones = new Transform[stoneCount];

        for (int i = 0; i < stoneCount; i++)
        {
            float angle = i * Mathf.PI * 2f / stoneCount;
            Vector3 localPos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            GameObject stone = Instantiate(stonePrefab, transform.position + localPos, Quaternion.identity, transform);
            stones[i] = stone.transform;
        }
    }
}
