using UnityEngine;
using System.Collections;

public class LavaController : MonoBehaviour
{
    [Header("Lava Movement")]
    public Transform lavaPlane;
    public float targetHeight = -30f; // Maximum tide level
    public float normalHeight = -3f;  // Default lava level
    public float speed = 0.5f;

    [Header("Audio Settings")]
    public AudioSource backgroundAudioToStop;       // Background sound to stop
    public LavaSoundController lavaSoundController; // Lava sound sequence manager
    public float lavaSoundDuration = 5f;            // Total duration of lava sound

    private bool hasTriggeredSound = false;

    public void SetLavaLevel(bool isActive)
    {
        StopAllCoroutines();
        StartCoroutine(AdjustLavaHeight(isActive ? targetHeight : normalHeight));

        if (isActive && !hasTriggeredSound)
        {
            hasTriggeredSound = true;
            StartCoroutine(HandleLavaAudioSequence());
        }
    }

    private IEnumerator AdjustLavaHeight(float height)
    {
        while (Mathf.Abs(lavaPlane.position.y - height) > 0.01f)
        {
            lavaPlane.position = new Vector3(
                lavaPlane.position.x,
                Mathf.Lerp(lavaPlane.position.y, height, Time.deltaTime * speed),
                lavaPlane.position.z
            );
            yield return null;
        }
    }

    private IEnumerator HandleLavaAudioSequence()
    {
        // Stop background audio
        if (backgroundAudioToStop != null && backgroundAudioToStop.isPlaying)
        {
            backgroundAudioToStop.Stop();
        }

        // Start lava sound sequence
        if (lavaSoundController != null)
        {
            lavaSoundController.StartLavaRetreatSequence();
        }

        // Wait for duration (5 seconds)
        yield return new WaitForSeconds(lavaSoundDuration);

        // Optional: stop the lava sound here if needed
        // (Most of it should already stop via LavaSoundController itself)
    }
}
