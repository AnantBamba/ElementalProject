using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlurryVisionEffect : MonoBehaviour
{
    public float blurDuration = 5f;  // Adjust for longer/shorter blur effect
    private Image blurImage;

    void Start()
    {
        blurImage = GetComponent<Image>();
        StartCoroutine(FadeBlur());
    }

    IEnumerator FadeBlur()
    {
        float elapsedTime = 0f;
        Color color = blurImage.color;

        while (elapsedTime < blurDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0.7f, 0, elapsedTime / blurDuration);
            blurImage.color = color;
            yield return null;
        }

        blurImage.gameObject.SetActive(false); // Remove effect after fading
    }
}