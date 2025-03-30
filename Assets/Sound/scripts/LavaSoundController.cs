using UnityEngine;
using System.Collections;

public class LavaSoundController : MonoBehaviour
{
    [Header("Lava Audio Sources")]
    public AudioSource lavaBubble;
    public AudioSource boilingMud;
    public AudioSource fireCrackle;
    public AudioSource windSuction;

    [Header("Timing Settings")]
    public float totalDuration = 5f;
    public float fadeOutDuration = 1.5f;
    public float windSuctionStartTime = 4f;

    private bool hasPlayed = false;

    public void StartLavaRetreatSequence()
    {
        if (hasPlayed) return;
        hasPlayed = true;
        StartCoroutine(PlayLavaSequence());
    }

    private IEnumerator PlayLavaSequence()
    {
        // Step 1: Play initial lava sounds
        if (lavaBubble != null) lavaBubble.Play();
        if (boilingMud != null) boilingMud.Play();
        if (fireCrackle != null) fireCrackle.Play();

        // Step 2: Wait to start wind suction (near the end)
        yield return new WaitForSeconds(windSuctionStartTime);

        if (windSuction != null)
        {
            windSuction.volume = 1f;
            windSuction.Play();
        }

        // Step 3: Wait until end then fade out all
        float remaining = totalDuration - windSuctionStartTime;
        yield return new WaitForSeconds(remaining);

        StartCoroutine(FadeOutAndStop(lavaBubble));
        StartCoroutine(FadeOutAndStop(boilingMud));
        StartCoroutine(FadeOutAndStop(fireCrackle));
        StartCoroutine(FadeOutAndStop(windSuction));

        yield return new WaitForSeconds(fadeOutDuration);
        hasPlayed = false; // allow retrigger if needed
    }

    private IEnumerator FadeOutAndStop(AudioSource source)
    {
        if (source == null || !source.isPlaying) yield break;

        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
