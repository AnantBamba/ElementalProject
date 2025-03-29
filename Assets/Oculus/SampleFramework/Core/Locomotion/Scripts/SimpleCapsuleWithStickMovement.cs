using System;
using UnityEngine;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
    public bool EnableLinearMovement = true;
    public bool EnableRotation = false;
    public bool RotationEitherThumbstick = false; // Restored variable to prevent errors
    public float Speed = 10.0f;
    public OVRCameraRig CameraRig;
    public OVRPlayerController PlayerController;

    public event Action CameraUpdated;

    private void Awake()
    {
        if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
        if (PlayerController == null) PlayerController = GetComponent<OVRPlayerController>();
    }

    private void Update()
    {
        CameraUpdated?.Invoke();

        if (EnableLinearMovement) StickMovement();
    }

    void StickMovement()
    {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Transform centerEye = CameraRig.centerEyeAnchor;

        Vector3 forward = centerEye.forward;
        forward.y = 0; // Ignore vertical tilt
        forward.Normalize();

        Vector3 right = centerEye.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDir = (forward * primaryAxis.y + right * primaryAxis.x) * Speed * Time.deltaTime;
        PlayerController.transform.position += moveDir;
    }
}
