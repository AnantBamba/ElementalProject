using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AltarTriggerHandler : MonoBehaviour
{
    public GameObject wispPrefab;  // Wisp GameObject
    public float moveSpeed = 2.0f;  // Speed of wisp movement
    private HashSet<Collider> triggeredColliders = new HashSet<Collider>();  // Tracks triggered zones

    // References to the altar GameObjects (Air, Fire, Water, Earth)
    public GameObject airAltar;
    public GameObject fireAltar;
    public GameObject waterAltar;
    public GameObject earthAltar;

    // References to the script that contains the orb booleans
    private FireOrbTrigger fireAltarScript;
    private AirOrbTrigger airAltarScript;
    private WaterOrbTrigger waterAltarScript;
    private EarthOrbTrigger earthAltarScript;

    private bool hasPlayedSound = false;  // To ensure the sound only plays once

    // Expose a public Vector3 variable for the target position (can be set in Inspector)
    public Vector3 targetPosition;  // Position to move the wisp to

    void Start()
    {
        if (wispPrefab == null)
        {
            Debug.LogError("Wisp Prefab not assigned!");
        }

        // Get references to the orb trigger scripts attached to the altar GameObjects
        airAltarScript = airAltar.GetComponent<AirOrbTrigger>();
        fireAltarScript = fireAltar.GetComponent<FireOrbTrigger>();
        waterAltarScript = waterAltar.GetComponent<WaterOrbTrigger>();
        earthAltarScript = earthAltar.GetComponent<EarthOrbTrigger>();

        // Check if orb trigger scripts are missing
        if (airAltarScript == null || fireAltarScript == null || waterAltarScript == null || earthAltarScript == null)
        {
            Debug.LogError("Missing Orb Trigger script on one of the altars!");
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

                // Start moving the wisp smoothly to the target position
                StartCoroutine(MoveWisp(targetPosition));
            }
        }
    }

    IEnumerator MoveWisp(Vector3 targetPosition)
    {
        // Move the wisp smoothly towards the target
        while (Vector3.Distance(wispPrefab.transform.position, targetPosition) > 0.1f)
        {
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
    }

    // Check if all the orb booleans are set to true
    bool CheckAllOrbsPlaced()
    {
        return airAltarScript.isAirOrbPlaced &&
               fireAltarScript.isFireOrbPlaced &&
               waterAltarScript.isWaterOrbPlaced &&
               earthAltarScript.isEarthOrbPlaced;
    }

    void Update()
    {
        // Check if all the orbs are placed and the sound hasn't played yet
        if (CheckAllOrbsPlaced() && !hasPlayedSound)
        {
            // Play sound after all orbs are placed
            AudioSource triggerAudio = GetComponent<AudioSource>();
            if (triggerAudio != null && triggerAudio.clip != null)
            {
                triggerAudio.Play();  // Play sound once all orbs are placed
                hasPlayedSound = true;  // Set flag to prevent sound from playing again
            }
            else
            {
                Debug.LogWarning("No AudioSource or AudioClip found on " + gameObject.name);
            }

            // Stop the scene after sound is played
            StopScene();
        }
    }

    // Stop the scene (for example, you could disable the game or load a new scene, etc.)
    void StopScene()
    {
        Debug.Log("All orbs are placed! Stopping the scene...");
        // You can either stop time, disable gameplay, or load a new scene here.
        Time.timeScale = 0;  // This will stop time, pausing the game.
        // Alternatively, you can load another scene here using SceneManager.LoadScene() if necessary.
    }
}
