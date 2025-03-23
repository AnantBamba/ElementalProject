using UnityEngine;
using Sydewa; // Ensure this is included to access LightingManager

public class FireOrbTrigger : MonoBehaviour
{
    public bool isFireOrbPlaced = false;

    [SerializeField] private LightingManager lightingManager;
    public float transitionSpeed = 1.0f;

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
            isFireOrbPlaced = true;
            Debug.Log("Fire Orb has been placed on the Altar!");
            StopAllCoroutines(); // Stop any ongoing time transition
            StartCoroutine(ChangeTimeOfDay(12f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FireOrb"))
        {
            isFireOrbPlaced = false;
            Debug.Log("Fire Orb has been removed from the Altar!");
            StopAllCoroutines(); // Stop any ongoing time transition
            StartCoroutine(ChangeTimeOfDay(lightingManager.StartTime));
        }
    }

    private System.Collections.IEnumerator ChangeTimeOfDay(float targetTime)
    {
        if (lightingManager == null)
        {
            yield break;
        }

        while (Mathf.Abs(lightingManager.TimeOfDay - targetTime) > 0.01f)
        {
            lightingManager.TimeOfDay = Mathf.Lerp(lightingManager.TimeOfDay, targetTime, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        lightingManager.TimeOfDay = targetTime;
    }
}
