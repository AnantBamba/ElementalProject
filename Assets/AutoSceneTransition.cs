using UnityEngine;

public class AutoSceneTransition : MonoBehaviour
{
    public string nextSceneName = "Final"; // Change to your actual scene name
    public float delay = 3f; // Time before transitioning

    void Start()
    {
        Invoke("TriggerSceneTransition", delay);
    }

    void TriggerSceneTransition()
    {
        SceneTransition transition = FindObjectOfType<SceneTransition>();

        if (transition != null)
        {
            transition.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("SceneTransition script not found in the scene!");
        }
    }
}