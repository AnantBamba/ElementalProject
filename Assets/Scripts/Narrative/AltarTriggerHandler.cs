using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AltarTriggerHandler : MonoBehaviour
{
    public GameObject wispPrefab;  
    public float moveSpeed = 2.0f;  
    private HashSet<Collider> triggeredColliders = new HashSet<Collider>();  

    public GameObject airAltar;
    public GameObject fireAltar;
    public GameObject waterAltar;
    public GameObject earthAltar;

    private FireOrbTrigger fireAltarScript;
    private AirOrbTrigger airAltarScript;
    private WaterOrbTrigger waterAltarScript;
    private EarthOrbTrigger earthAltarScript;

    private bool hasPlayedSound = false;  

    public Vector3 targetPosition;  

    public AudioSource backgroundMusic;  // Background music AudioSource
    public AudioSource dialogueAudio;    // Dialogue AudioSource

    void Start()
    {
        if (wispPrefab == null)
        {
            Debug.LogError("Wisp Prefab not assigned!");
        }

        airAltarScript = airAltar.GetComponent<AirOrbTrigger>();
        fireAltarScript = fireAltar.GetComponent<FireOrbTrigger>();
        waterAltarScript = waterAltar.GetComponent<WaterOrbTrigger>();
        earthAltarScript = earthAltar.GetComponent<EarthOrbTrigger>();

        if (airAltarScript == null || fireAltarScript == null || waterAltarScript == null || earthAltarScript == null)
        {
            Debug.LogError("Missing Orb Trigger script on one of the altars!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            Debug.Log("Player entered the trigger: " + other.gameObject.name);

            if (!triggeredColliders.Contains(other))
            {
                triggeredColliders.Add(other);
                StartCoroutine(MoveWisp(targetPosition));
            }
        }
    }

    IEnumerator MoveWisp(Vector3 targetPosition)
    {
        while (Vector3.Distance(wispPrefab.transform.position, targetPosition) > 0.1f)
        {
            wispPrefab.transform.position = Vector3.MoveTowards(
                wispPrefab.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            Vector3 direction = targetPosition - wispPrefab.transform.position;
            if (direction != Vector3.zero)  
            {
                Quaternion rotation = Quaternion.LookRotation(direction);  
                wispPrefab.transform.rotation = Quaternion.Slerp(wispPrefab.transform.rotation, rotation, Time.deltaTime * moveSpeed);
            }

            yield return null;  
        }

        wispPrefab.transform.position = targetPosition;
        Debug.Log("Wisp reached target: " + targetPosition);
    }

    bool CheckAllOrbsPlaced()
    {
        return airAltarScript.isAirOrbPlaced &&
               fireAltarScript.isFireOrbPlaced &&
               waterAltarScript.isWaterOrbPlaced &&
               earthAltarScript.isEarthOrbPlaced;
    }

    void Update()
    {
        if (CheckAllOrbsPlaced() && !hasPlayedSound)
        {
            StartCoroutine(PlayAudioAndQuit());
            hasPlayedSound = true;
        }
    }

    IEnumerator PlayAudioAndQuit()
    {
        Debug.Log("All orbs are placed! Playing sounds before quitting...");

        if (dialogueAudio != null && backgroundMusic != null && dialogueAudio.clip != null)
        {
            dialogueAudio.Play();
            backgroundMusic.Play();

            float dialogueLength = dialogueAudio.clip.length;
            float elapsedTime = 0f;

            while (elapsedTime < dialogueLength)
            {
                yield return null;
                elapsedTime += Time.deltaTime;

                if (!backgroundMusic.isPlaying)  
                {
                    backgroundMusic.Play();  
                }
                if (!dialogueAudio.isPlaying)  
                {
                    dialogueAudio.Play();  
                }
            }
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or AudioClip!");
        }

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}