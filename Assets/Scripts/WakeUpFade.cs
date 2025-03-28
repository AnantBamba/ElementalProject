using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakeUpFade : MonoBehaviour
{
    public float fadeDuration = 3f;  // Adjust for slower/faster fade
    private Image fadeImage;

    void Start()
    {
        fadeImage = GetComponent<Image>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // Hide after fading
    }
}