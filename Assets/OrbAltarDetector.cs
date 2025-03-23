using UnityEngine;

public class OrbAltarDetector : MonoBehaviour
{
    public WaterController waterController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            waterController.SetWaterLevel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            waterController.SetWaterLevel(false);
        }
    }
}
