using UnityEngine;
using System.Collections;

public class LavaSoundController : MonoBehaviour
{
    public AudioSource lavaBubble;
    public AudioSource boilingMud;
    public AudioSource fireCrackle;
    public AudioSource windSuction;

    public float fadeDuration = 5f;

    public void StartLavaRetreatSequence()
    {
        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        float elapsed = 0f;

        
        lavaBubble.Play();
        boilingMud.Play();
        fireCrackle.Play();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            
            lavaBubble.volume = Mathf.Lerp(1f, 0f, t);
            boilingMud.volume = Mathf.Lerp(1f, 0f, t);
            fireCrackle.volume = Mathf.Lerp(1f, 0f, t);

            
            lavaBubble.pitch = Mathf.Lerp(1f, 0.7f, t);
            boilingMud.pitch = Mathf.Lerp(1f, 0.8f, t);

            yield return null;
        }

        lavaBubble.Stop();
        boilingMud.Stop();
        fireCrackle.Stop();

        
        windSuction.volume = 0f;
        windSuction.Play();

        float windTime = 3f;
        float windElapsed = 0f;

        while (windElapsed < windTime)
        {
            windElapsed += Time.deltaTime;
            float wt = windElapsed / windTime;
            windSuction.volume = Mathf.Lerp(1f, 0f, wt);
            yield return null;
        }

        windSuction.Stop();
    }
}
