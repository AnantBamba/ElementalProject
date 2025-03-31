using UnityEngine;
using System.Collections;

public class WaterController : MonoBehaviour
{
    public Transform waterPlane;
    public float targetHeight = 0f; // Maximum tide level
    public float normalHeight = 0f; // Default water level
    public float speed = 2f; // Adjust speed for smooth transition

    public void SetWaterLevel(bool isActive)
    {
        StopAllCoroutines();
        StartCoroutine(AdjustWaterHeight(isActive ? targetHeight : normalHeight));
    }

    private IEnumerator AdjustWaterHeight(float height)
    {
        while (Mathf.Abs(waterPlane.position.y - height) > 0.01f)
        {
            waterPlane.position = new Vector3(
                waterPlane.position.x,
                Mathf.Lerp(waterPlane.position.y, height, Time.deltaTime * speed),
                waterPlane.position.z
            );
            yield return null;
        }
    }
}