using UnityEngine;
using System.Collections;
using OculusSampleFramework; // 如果使用 OVRGrabbable

public class OrbActivator : MonoBehaviour
{
    public string orbTag = "EarthOrb";
    public Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    public Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private bool isOrbPlaced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isOrbPlaced || !other.CompareTag(orbTag)) return;

        OVRGrabbable grabbable = other.GetComponent<OVRGrabbable>();

        if (grabbable != null && grabbable.isGrabbed)
        {
            StartCoroutine(WaitForRelease(grabbable));
        }
        else
        {
            SnapOrbToAltar(other.gameObject);
        }
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
        {
            yield return null;
        }

        if (grabbable.CompareTag(orbTag))
        {
            SnapOrbToAltar(grabbable.gameObject);
        }
    }

    private void SnapOrbToAltar(GameObject orb)
    {
        Vector3 worldSnapPosition = transform.TransformPoint(localSnapPosition);
        Quaternion worldSnapRotation = transform.rotation * Quaternion.Euler(localSnapRotation);

        orb.transform.position = worldSnapPosition;
        orb.transform.rotation = worldSnapRotation;

        Rigidbody orbRb = orb.GetComponent<Rigidbody>();
        if (orbRb != null)
        {
            orbRb.isKinematic = true;
        }

        isOrbPlaced = true;
        Debug.Log("Earth Orb placed on altar!");

        TriggerNatureSpread(orb.transform);
    }

    private void TriggerNatureSpread(Transform orbTransform)
    {
        NatureSpreadWithTrigger spread = FindObjectOfType<NatureSpreadWithTrigger>();
        if (spread != null)
        {
            spread.TriggerSpread(orbTransform);
        }
    }
}
