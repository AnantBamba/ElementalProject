using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WispTriggerHandler : MonoBehaviour
{
    public GameObject wispPrefab;  // Wisp GameObject
    public float moveSpeed = 2.0f;  // Speed of wisp movement
    private HashSet<Collider> triggeredColliders = new HashSet<Collider>();  // Tracks triggered zones

    void Start()
    {
        if (wispPrefab == null)
        {
            Debug.LogError("Wisp Prefab not assigned!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))  // You can tag the player with "Player"
        {
            // Debug: Log what is entering the trigger
            Debug.Log("Player entered the trigger: " + other.gameObject.name);

            // Make sure we don't trigger the wisp movement multiple times
            if (!triggeredColliders.Contains(other))
            {
                triggeredColliders.Add(other);  // Mark this trigger as activated

                // Get the target position (where the wisp should go)
                Vector3 targetPosition = transform.position; // Trigger zone position
                Debug.Log("Wisp moving to: " + targetPosition);

                // Start moving the wisp smoothly
                StartCoroutine(MoveWisp(targetPosition));
            }
        }
    }

    IEnumerator MoveWisp(Vector3 targetPosition)
    {
        // Move the wisp smoothly towards the target
        while (Vector3.Distance(wispPrefab.transform.position, targetPosition) > 0.1f)
        {
            // Move the wisp
            wispPrefab.transform.position = Vector3.MoveTowards(
                wispPrefab.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Rotate the wisp to look forward towards the target
            Vector3 direction = targetPosition - wispPrefab.transform.position;  // Direction from wisp to target
            if (direction != Vector3.zero)  // Avoid issues when the direction is zero
            {
                Quaternion rotation = Quaternion.LookRotation(direction);  // Get the rotation towards the target
                wispPrefab.transform.rotation = Quaternion.Slerp(wispPrefab.transform.rotation, rotation, Time.deltaTime * moveSpeed);
            }

            yield return null;  // Wait for the next frame
        }

        // Ensure the wisp reaches the exact target position
        wispPrefab.transform.position = targetPosition;
        Debug.Log("Wisp reached target: " + targetPosition);

        // Optionally, play sound if the trigger zone has an AudioSource
        AudioSource triggerAudio = GetComponent<AudioSource>();
        if (triggerAudio != null && triggerAudio.clip != null)
        {
            triggerAudio.Play();  // Play sound after wisp arrives
        }
        else
        {
            Debug.LogWarning("No AudioSource or AudioClip found on " + gameObject.name);
        }
    }
}
