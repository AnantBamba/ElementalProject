using UnityEngine;
using OculusSampleFramework;

public class OrbGrabHandler : MonoBehaviour
{
    private OVRGrabbable grabbable;
    public WaterController waterController;
    private bool isHeld = false;

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
    }

    void Update()
    {
        if (grabbable.isGrabbed && !isHeld)
        {
            isHeld = true;
            waterController.SetWaterLevel(true);
        }
        else if (!grabbable.isGrabbed && isHeld)
        {
            isHeld = false;
            waterController.SetWaterLevel(false);
        }
    }
}