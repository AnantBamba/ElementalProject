using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;  // Assign the FadeScreen UI Image
    public float fadeDuration = 1.5f;

    void Start()
    {
        StartCoroutine(FadeIn()); // Fade in at the start of the scene
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    IEnumerator Transition(string sceneName)
    {
        yield return StartCoroutine(FadeOut()); // Fade to black

        // Load the new scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null; // Wait until loading is complete
        }

        yield return new WaitForSeconds(0.5f); // Optional: Small delay before fading in

        yield return StartCoroutine(FadeIn()); // Fade back in

        // Unload the previous scene to free up memory
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}