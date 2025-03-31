using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WispTriggerHandler : MonoBehaviour
{
    public GameObject wispPrefab;  // Wisp GameObject
    public float moveSpeed = 2.0f;  // Speed of wisp movement
    public AudioClip voiceLine;  // Assign different voice lines in Unity

    private AudioSource wispAudioSource;  // Wisp's AudioSource
    private HashSet<Collider> triggeredColliders = new HashSet<Collider>();  // Track triggered zones

    void Start()
    {
        if (wispPrefab == null)
        {
            Debug.LogError("Wisp Prefab not assigned!");
            return;
        }

        wispAudioSource = wispPrefab.GetComponent<AudioSource>();

        if (wispAudioSource == null)
        {
            Debug.LogError("No AudioSource found on Wisp Prefab!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger: " + other.gameObject.name);

            if (!triggeredColliders.Contains(other))
            {
                triggeredColliders.Add(other);  // Mark this trigger as activated

                Vector3 targetPosition = transform.position; // Move wisp to this trigger zone
                Debug.Log("Wisp moving to: " + targetPosition);

                StartCoroutine(MoveWisp(targetPosition));
            }
        }
    }

    IEnumerator MoveWisp(Vector3 targetPosition)
    {
        // Move the wisp smoothly
        while (Vector3.Distance(wispPrefab.transform.position, targetPosition) > 0.1f)
        {
            wispPrefab.transform.position = Vector3.MoveTowards(
                wispPrefab.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Rotate the wisp toward the target
            Vector3 direction = targetPosition - wispPrefab.transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                wispPrefab.transform.rotation = Quaternion.Slerp(
                    wispPrefab.transform.rotation, rotation, Time.deltaTime * moveSpeed
                );
            }

            yield return null;
        }

        // Snap wisp to exact position
        wispPrefab.transform.position = targetPosition;
        Debug.Log("Wisp reached target: " + targetPosition);

        // **Now Play the Voice Line from the Wisp's Position**
        if (voiceLine != null && wispAudioSource != null)
        {
            yield return new WaitForSeconds(0.5f); // Small delay before speaking
            wispAudioSource.clip = voiceLine;
            wispAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No voice line assigned or missing AudioSource on Wisp.");
        }
    }
}