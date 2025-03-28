using UnityEngine;
using System.Collections;
using Sydewa;
using OculusSampleFramework;

public class FireOrbTrigger : MonoBehaviour
{
    public bool isFireOrbPlaced = false;

    [SerializeField] private LightingManager lightingManager;
    public float transitionSpeed = 1.0f;
    private Coroutine timeChangeCoroutine;

    [SerializeField] private Vector3 localSnapPosition = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Vector3 localSnapRotation = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        if (lightingManager == null)
        {
            lightingManager = FindObjectOfType<LightingManager>();
            if (lightingManager == null)
            {
                Debug.LogError("LightingManager not found in the scene!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireOrb"))
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
        if (other.CompareTag("FireOrb"))
        {
            // Fire Orb has fully left the altar, transition back regardless of grab state
            ResetTime();
        }
    }

    private void ResetTime()
    {
        isFireOrbPlaced = false;
        Debug.Log("Fire Orb has been removed from the Altar!");
        StartTransition(lightingManager.StartTime);
    }

    private void StartTransition(float targetTime)
    {
        if (timeChangeCoroutine != null)
        {
            StopCoroutine(timeChangeCoroutine);
        }
        timeChangeCoroutine = StartCoroutine(ChangeTimeOfDay(targetTime));
    }

    private IEnumerator ChangeTimeOfDay(float targetTime)
    {
        if (lightingManager == null)
        {
            yield break;
        }

        while (Mathf.Abs(lightingManager.TimeOfDay - targetTime) > 0.01f)
        {
            lightingManager.TimeOfDay = Mathf.MoveTowards(lightingManager.TimeOfDay, targetTime, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        lightingManager.TimeOfDay = targetTime;
    }

    private IEnumerator WaitForRelease(OVRGrabbable grabbable)
    {
        while (grabbable.isGrabbed)
        {
            yield return null;
        }

        if (grabbable.CompareTag("FireOrb"))
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

        isFireOrbPlaced = true;
        Debug.Log("Fire Orb has been placed on the Altar!");
        StartTransition(12f);
    }
}
