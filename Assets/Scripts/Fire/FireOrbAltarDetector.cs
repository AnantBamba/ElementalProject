using UnityEngine;

public class FireOrbAltarDetector : MonoBehaviour
{
    public LavaController lavaController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireOrb"))
        {
            lavaController.SetLavaLevel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FireOrb"))
        {
            lavaController.SetLavaLevel(false);
        }
    }
}
