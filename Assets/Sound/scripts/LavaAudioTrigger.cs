using UnityEngine;

public class LavaAudioTrigger : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource backgroundAudioToStop;    // The ambient audio to stop
    public LavaSoundController lavaSoundController; // Reference to lava sound sequence controller

    [Header("Optional Delay")]
    public float delayBeforeStart = 0f; // If you want a slight pause before lava sound begins

    [Header("Playback Duration")]
    public float lavaSoundDuration = 5f; // Duration lava sounds should play

    private bool hasTriggered = false;

    // This method should be called by LavaController
    public void TriggerLavaAudioSequence()
    {
        if (hasTriggered) return; // Prevent repeat triggers
        hasTriggered = true;
        StartCoroutine(PlayLavaSequence());
    }

    private System.Collections.IEnumerator PlayLavaSequence()
    {
        // Step 1: Stop background audio
        if (backgroundAudioToStop != null && backgroundAudioToStop.isPlaying)
        {
            backgroundAudioToStop.Stop();
        }

        // Step 2: Optional delay before starting lava sound
        if (delayBeforeStart > 0f)
        {
            yield return new WaitForSeconds(delayBeforeStart);
        }

        // Step 3: Play lava sound sequence
        if (lavaSoundController != null)
        {
            lavaSoundController.StartLavaRetreatSequence();
        }

        // Step 4: Wait for duration (5s or any duration)
        yield return new WaitForSeconds(lavaSoundDuration);

        // (Optional) You can trigger follow-up actions here if needed
    }
}
