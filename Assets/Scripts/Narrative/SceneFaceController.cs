using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeController : MonoBehaviour
{
    public Image blackOverlay;
    private SimpleCapsuleWithStickMovement movementScript;
    public GameObject wisp;
    public GameObject altarCollision;
    public Vector3 initialPosition = new Vector3(0, 0, 2);
    public Vector3 targetPosition = new Vector3(0, 0, 5);
    public float fadeDuration = 3f;
    public AudioClip audioClip;

    private Transform centerEyeAnchor;
    private AudioSource audioSource;

    private void Start()
    {
        centerEyeAnchor = Camera.main.transform;
        ResetRotation();

        movementScript = FindObjectOfType<SimpleCapsuleWithStickMovement>();

        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            blackOverlay.color = Color.black;
        }

        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        if (wisp != null)
        {
            wisp.transform.position = initialPosition;
            Renderer wispRenderer = wisp.GetComponent<Renderer>();
            if (wispRenderer != null)
            {
                wispRenderer.enabled = true;
            }
        }

        if (altarCollision != null)
        {
            altarCollision.SetActive(false);  // Disable AltarCollision at the start
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;

        StartCoroutine(PlayAudioAndStartTransition());
    }

    private void ResetRotation()
    {
        if (centerEyeAnchor != null)
        {
            Vector3 rotation = centerEyeAnchor.rotation.eulerAngles;
            rotation.y = 0;
            centerEyeAnchor.rotation = Quaternion.Euler(rotation);
        }
    }

    private IEnumerator PlayAudioAndStartTransition()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);  // Wait for the audio to finish

        if (movementScript != null)
        {
            movementScript.enabled = true;  // Enable movement after audio
        }

        StartCoroutine(FadeInAndMoveWisp());  // Start fade and wisp movement at the same time
    }

    private IEnumerator FadeInAndMoveWisp()
    {
        float elapsedTime = 0f;
        Vector3 initialPos = wisp.transform.position;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            blackOverlay.color = new Color(0, 0, 0, alpha);

            wisp.transform.position = Vector3.Lerp(initialPos, targetPosition, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackOverlay.color = new Color(0, 0, 0, 0);
        blackOverlay.gameObject.SetActive(false);
        wisp.transform.position = targetPosition;

        if (altarCollision != null)
        {
            altarCollision.SetActive(true);  // Re-enable AltarCollision after transition
        }
    }
}
