using UnityEngine;
using System.Collections;

public class LavaController : MonoBehaviour
{
    [Header("Lava Movement")]
    public Transform lavaPlane;
    public float targetHeight = -30f; // Lowered lava level
    public float normalHeight = -3f;  // Default lava level
    public float speed = 0.5f;

    [Header("Audio Control")]
    public AudioSource backgroundAudioToStop;       // Background sound to stop
    public LavaSoundController lavaSoundController; // Handles lava audio sequence
    public float lavaSoundDuration = 5f;            // Duration lava audio plays

    private bool hasTriggeredSound = false;

    /// <summary>
    /// Call this to raise or lower the lava.
    /// </summary>
    /// <param name="isActive">true = lower lava; false = return to normal height</param>
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

    private IEnumerator AdjustLavaHeight(float targetY)
    {
        while (Mathf.Abs(lavaPlane.position.y - targetY) > 0.01f)
        {
            lavaPlane.position = new Vector3(
                lavaPlane.position.x,
                Mathf.Lerp(lavaPlane.position.y, targetY, Time.deltaTime * speed),
                lavaPlane.position.z
            );
            yield return null;
        }

        lavaPlane.position = new Vector3(
            lavaPlane.position.x,
            targetY,
            lavaPlane.position.z
        );
    }

    private IEnumerator HandleLavaAudioSequence()
    {
        // Step 1: Stop background audio
        if (backgroundAudioToStop != null && backgroundAudioToStop.isPlaying)
        {
            backgroundAudioToStop.Stop();
        }

        // Step 2: Start lava descent audio
        if (lavaSoundController != null)
        {
            lavaSoundController.StartLavaRetreatSequence(); // Your own method in LavaSoundController
        }

        // Step 3: Wait for duration
        yield return new WaitForSeconds(lavaSoundDuration);

        // Done
    }
}
