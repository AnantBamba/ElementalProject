using UnityEngine;

public class AutoSceneTransition : MonoBehaviour
{
    public string nextSceneName = "Final";  // Change this to your actual next scene

    void Start()
    {
        Invoke("TriggerSceneTransition", 3f); // Wait for 10 seconds, then transition
    }

    void TriggerSceneTransition()
    {
        FindObjectOfType<SceneTransition>().LoadScene(nextSceneName);
    }
}