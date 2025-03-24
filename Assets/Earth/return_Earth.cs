using System.Collections;
using UnityEngine;

public class RitualTrigger : MonoBehaviour
{
    public Transform sphere;
    public Transform targetPoint;
    public float snapDistance = 1.5f;
    public float snapSpeed = 3f;

    public Renderer spikeRenderer; // 新增：尖刺的材质渲染器
    public float dissolveSpeed = 1.5f; // 溶解速度

    public Transform house;
    public float houseTargetY = 0f;
    public float houseFallSpeed = 2f;

    private bool snapping = false;
    private bool snapped = false;
    private bool ritualStarted = false;

    void Update()
    {
        if (!snapped && Vector3.Distance(sphere.position, targetPoint.position) < snapDistance)
        {
            snapping = true;
        }

        if (snapping && !snapped)
        {
            sphere.position = Vector3.Lerp(sphere.position, targetPoint.position, Time.deltaTime * snapSpeed);

            if (Vector3.Distance(sphere.position, targetPoint.position) < 0.1f)
            {
                snapped = true;
                snapping = false;
                StartCoroutine(TriggerRitualEvent());
            }
        }
    }

    IEnumerator TriggerRitualEvent()
    {
        if (spikeRenderer != null)
        {
            float dissolve = 0f;
            Material mat = spikeRenderer.material;

            while (dissolve < 1f)
            {
                dissolve += Time.deltaTime * dissolveSpeed;
                mat.SetFloat("_Dissolve", dissolve);
                yield return null;
            }

            // 可选：让尖刺完全透明后禁用它
            spikeRenderer.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.8f);

        if (house != null)
        {
            house.SetParent(null);

            Vector3 start = house.position;
            Vector3 target = new Vector3(start.x, houseTargetY, start.z);

            while (Vector3.Distance(house.position, target) > 0.1f)
            {
                house.position = Vector3.MoveTowards(house.position, target, Time.deltaTime * houseFallSpeed);
                yield return null;
            }
        }
    }
}
