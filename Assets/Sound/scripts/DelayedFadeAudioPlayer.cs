using UnityEngine;

public class DelayedFadeAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public float delayInSeconds = 2f;
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 2f;

    private bool isFadingOut = false;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = 0f;
            StartCoroutine(DelayedStart());
        }
        else
        {
            Debug.LogWarning("AudioSource Î´°ó¶¨£¡");
        }
    }

    private System.Collections.IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(delayInSeconds);
        audioSource.Play();
        StartCoroutine(FadeInAudio());
    }

    private System.Collections.IEnumerator FadeInAudio()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        audioSource.volume = 1f;
    }

    
    public void TriggerFadeOutAndStop()
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndStop());
        }
    }

    private System.Collections.IEnumerator FadeOutAndStop()
    {
        isFadingOut = true;
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0f;
        isFadingOut = false;
    }
}
