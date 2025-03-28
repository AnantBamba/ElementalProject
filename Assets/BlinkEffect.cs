using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkEffect : MonoBehaviour
{
    public Image blinkImage;
    public int blinkCount = 3;  // Number of blinks
    public float blinkSpeed = 0.3f;  // Time per blink

    void Start()
    {
        blinkImage = GetComponent<Image>();
        StartCoroutine(BlinkSequence());
    }

    IEnumerator BlinkSequence()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            yield return StartCoroutine(BlinkOnce());
        }

        // Ensure it's fully faded at the end
        blinkImage.gameObject.SetActive(false);
    }

    IEnumerator BlinkOnce()
    {
        float elapsedTime = 0f;
        Color color = blinkImage.color;

        // Close eyes (fade to black)
        while (elapsedTime < blinkSpeed)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsedTime / blinkSpeed);
            blinkImage.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); // Pause at full black

        elapsedTime = 0f;

        // Open eyes (fade to clear)
        while (elapsedTime < blinkSpeed)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / blinkSpeed);
            blinkImage.color = color;
            yield return null;
        }
    }
}
