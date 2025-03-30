using UnityEngine;

public class TripleOrbitRing : MonoBehaviour
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;
    public int orbsPerRing = 24;
    public float radiusHorizontal = 3f;
    public float radiusTiltRight = 3f;
    public float radiusTiltLeft = 3f;

    [Header("Rotation Settings")]
    public float ringRotationSpeed = 20f;
    public float orbSelfRotationSpeed = 60f;

    private GameObject[] ringParents = new GameObject[3];

    void Start()
    {
        CreateRing(0, radiusHorizontal, Quaternion.identity);  
        CreateRing(1, radiusTiltRight, Quaternion.Euler(0, 0, -15));  
        CreateRing(2, radiusTiltLeft, Quaternion.Euler(0, 0, 15));      
    }

    void Update()
    {
        // 让每个环绕球体自转
        foreach (GameObject ring in ringParents)
        {
            ring.transform.Rotate(Vector3.up, ringRotationSpeed * Time.deltaTime, Space.World);
        }
    }

    void CreateRing(int index, float radius, Quaternion tilt)
    {
        GameObject ringParent = new GameObject($"Ring_{index}");
        ringParent.transform.SetParent(transform);
        ringParent.transform.localPosition = Vector3.zero;
        ringParent.transform.localRotation = tilt;
        ringParents[index] = ringParent;

        for (int i = 0; i < orbsPerRing; i++)
        {
            float angle = (360f / orbsPerRing) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 localPos = new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);
            GameObject orb = Instantiate(orbPrefab, ringParent.transform);
            orb.transform.localPosition = localPos;
            orb.transform.LookAt(ringParent.transform.position); 
            orb.AddComponent<SelfRotator>().rotationSpeed = orbSelfRotationSpeed;
        }
    }
}

public class SelfRotator : MonoBehaviour
{
    public float rotationSpeed = 60f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
