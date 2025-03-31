using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeController : MonoBehaviour
{
    public Image blackOverlay;  // Reference to the black screen overlay image
    private SimpleCapsuleWithStickMovement movementScript;  // Reference to the movement script
    public GameObject wisp;  // Reference to the wisp GameObject
    public GameObject altarCollision;  // Reference to the AltarCollision GameObject
    public Vector3 initialPosition = new Vector3(0, 0, 2);  // Initial position of the wisp in world space
    public Vector3 targetPosition = new Vector3(0, 0, 5);  // Target position for the wisp in world space
    public float fadeDuration = 3f;  // Duration of fade-in effect and wisp movement (same duration for both)
    public AudioClip audioClip;  // The audio clip to play at the start

    private Transform centerEyeAnchor;  // Reference to the CenterEyeAnchor transform
    private AudioSource audioSource;  // Reference to the AudioSource component
    private float initialBlackTime;  // Duration the screen stays black before fading

    private void Start()
    {
        // Get the CenterEyeAnchor transform from the OVRPlayerController
        centerEyeAnchor = Camera.main.transform;  // Assuming Camera.main is the CenterEyeAnchor

        // Fix the rotation issue by setting the CenterEyeAnchor's Y rotation to 0
        ResetRotation();

        // Get the movement script
        movementScript = FindObjectOfType<SimpleCapsuleWithStickMovement>();

        // Ensure the black screen overlay is fully visible at the start
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            blackOverlay.color = Color.black; // Ensure the screen starts black
        }

        // Disable movement script initially
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Ensure the wisp is positioned at the initial position in world space
        if (wisp != null)
        {
            wisp.transform.position = initialPosition;  // Set the wisp position in world space
            Renderer wispRenderer = wisp.GetComponent<Renderer>();
            if (wispRenderer != null)
            {
                wispRenderer.enabled = true; // Make sure the wisp is visible
            }
        }

        // Add and configure the AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;

        // Set the initial black screen time to the length of the audio clip
        initialBlackTime = audioClip.length;

        // Start the audio and wait for it to finish before starting the transition
        StartCoroutine(PlayAudioAndStartTransition());
    }

    private void ResetRotation()
    {
        // Reset the Y rotation of the camera to 0 to avoid the 180-degree offset
        if (centerEyeAnchor != null)
        {
            Vector3 rotation = centerEyeAnchor.rotation.eulerAngles;
            rotation.y = 0;  // Set Y rotation to 0
            centerEyeAnchor.rotation = Quaternion.Euler(rotation);  // Apply the corrected rotation
        }
    }

    private IEnumerator PlayAudioAndStartTransition()
    {
        // Play the audio
        audioSource.Play();

        // Wait for the audio to finish before starting the transition
        yield return new WaitForSeconds(audioSource.clip.length);

        // Start the fade-in process
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // Disable the AltarCollision GameObject before the transition
        if (altarCollision != null)
        {
            altarCollision.SetActive(false);
        }

        // Wait for the initial black screen duration (this now matches the audio length)
        yield return new WaitForSeconds(initialBlackTime);

        // Enable movement script just before fading starts
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        // Smooth fade effect for the black screen and wisp movement (both happen simultaneously)
        float elapsedTime = 0f;
        Vector3 initialPos = wisp.transform.position;

        while (elapsedTime < fadeDuration)
        {
            // Lerp the black screen alpha from opaque to transparent
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            blackOverlay.color = new Color(0, 0, 0, alpha);

            // Lerp the wisp position from the initial position to the target position
            wisp.transform.position = Vector3.Lerp(initialPos, targetPosition, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the black overlay is fully transparent at the end
        blackOverlay.color = new Color(0, 0, 0, 0);
        blackOverlay.gameObject.SetActive(false);

        // Ensure the wisp reaches the target position exactly
        wisp.transform.position = targetPosition;

        // Re-enable the AltarCollision GameObject after the transition
        if (altarCollision != null)
        {
            altarCollision.SetActive(true);
        }
    }
}
