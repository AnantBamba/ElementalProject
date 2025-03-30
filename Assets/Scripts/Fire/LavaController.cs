using UnityEngine;
using System.Collections;

public class LavaController : MonoBehaviour
{
    public Transform lavaPlane;
    public float targetHeight = -30f; // Maximum tide level
    public float normalHeight = -3f; // Default lava level
    public float speed = 0.5f; // Adjust speed for smooth transition

    public void SetLavaLevel(bool isActive)
    {
        StopAllCoroutines();
        StartCoroutine(AdjustLavaHeight(isActive ? targetHeight : normalHeight));
    }

    private IEnumerator AdjustLavaHeight(float height)
    {
        while (Mathf.Abs(lavaPlane.position.y - height) > 0.01f)
        {
            lavaPlane.position = new Vector3(
                lavaPlane.position.x,
                Mathf.Lerp(lavaPlane.position.y, height, Time.deltaTime * speed),
                lavaPlane.position.z
            );
            yield return null;
        }
    }
}