using UnityEngine;
using System.Collections;
using OculusSampleFramework;

public class WaterOrbTrigger : MonoBehaviour
{
    public bool isWaterOrbPlaced = false;

    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterOrb"))
        {
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WaterOrb"))
        {
            // Water Orb has been removed from the altar
            Debug.Log("Water Orb has been removed from the Altar!");
            isWaterOrbPlaced = false;
        }
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
        {
            yield return null;
        }

        if (grabbable.CompareTag("WaterOrb"))
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

        isWaterOrbPlaced = true;
        Debug.Log("Water Orb has been placed on the Altar!");
    }
}
