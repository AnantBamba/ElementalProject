using UnityEngine;

public class OrbActivator : MonoBehaviour
{
    public string altarTag = "EarthAltar";
    public float snapSpeed = 5f;
    public float snapThreshold = 0.1f;

    private Transform altarTarget;
    private bool isSnapping = false;
    private bool hasActivated = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag(altarTag))
        {
            altarTarget = other.transform;
            isSnapping = true;
        }
    }

    void Update()
    {
        if (isSnapping && altarTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, altarTarget.position, Time.deltaTime * snapSpeed);

            if (Vector3.Distance(transform.position, altarTarget.position) < snapThreshold)
            {
                transform.position = altarTarget.position;
                isSnapping = false;

                TriggerNatureSpread();
                hasActivated = true;
            }
        }
    }

    void TriggerNatureSpread()
    {
        NatureSpreadWithTrigger spread = FindObjectOfType<NatureSpreadWithTrigger>();
        if (spread != null)
        {
            spread.TriggerSpread(this.transform);
        }
    }
}
