using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;  // Assign the FadeScreen UI Image
    public float fadeDuration = 1.5f;
    private string sceneToLoad;

    void Start()
    {
        StartCoroutine(FadeIn()); // Fade in at the start of the scene
    }

    public void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        yield return StartCoroutine(FadeOut()); // Smooth fade to black

        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false; // Prevent sudden activation

        // Wait until the scene is almost loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate the scene after fading out
        asyncLoad.allowSceneActivation = true;

        // Give Unity a short delay to stabilize memory before unloading
        yield return new WaitForSeconds(0.5f);

        // Unload the previous scene AFTER the new scene is fully loaded
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(FadeIn()); // Smooth fade in
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
