using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeController : MonoBehaviour
{
    public Image blackOverlay;  // Reference to the black screen overlay image
    private SimpleCapsuleWithStickMovement movementScript;  // Reference to the movement script
    public GameObject wisp;  // Reference to the wisp GameObject
    public Vector3 initialPosition = new Vector3(0, 0, 2);  // Initial position of the wisp relative to CenterEyeAnchor (local)
    public Vector3 targetPosition = new Vector3(0, 0, 5);  // Target position for the wisp relative to CenterEyeAnchor (local)
    public float fadeDuration = 3f;  // Duration of fade-in effect
    public float initialBlackTime = 2f;  // How long the screen stays black before fading
    public float wispMoveDuration = 3f;  // Duration for the wisp to move to the target position

    private Transform centerEyeAnchor;  // Reference to the CenterEyeAnchor transform

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

        // Ensure the wisp is positioned at the initial position relative to the CenterEyeAnchor (local position)
        if (wisp != null && centerEyeAnchor != null)
        {
            // Set the wisp position relative to the CenterEyeAnchor
            wisp.transform.localPosition = initialPosition;
            Renderer wispRenderer = wisp.GetComponent<Renderer>();
            if (wispRenderer != null)
            {
                wispRenderer.enabled = true; // Make sure the wisp is visible
            }
        }

        // Start the fade-in process
        StartCoroutine(FadeIn());
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

    private IEnumerator FadeIn()
    {
        // Wait for the initial black screen duration
        yield return new WaitForSeconds(initialBlackTime);

        // Enable movement script just before fading starts
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        // Smooth fade effect for the black screen
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            blackOverlay.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the black overlay is fully transparent at the end
        blackOverlay.color = new Color(0, 0, 0, 0);
        blackOverlay.gameObject.SetActive(false);

        // Start the wisp movement after the black screen transition
        StartCoroutine(MoveWispToTarget());
    }

    private IEnumerator MoveWispToTarget()
    {
        // Smoothly move the wisp from the initial position to the target position
        float moveElapsedTime = 0f;
        Vector3 initialPos = wisp.transform.localPosition;

        while (moveElapsedTime < wispMoveDuration)
        {
            wisp.transform.localPosition = Vector3.Lerp(initialPos, targetPosition, moveElapsedTime / wispMoveDuration);
            moveElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the wisp reaches the target position exactly
        wisp.transform.localPosition = targetPosition;
    }
}
