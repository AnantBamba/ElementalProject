using UnityEngine;

public class LoopingFootstepAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;       // Should have the 5s footstep loop assigned
    public float movementThreshold = 0.1f;

    [Header("Movement Settings")]
    public CharacterController characterController;

    private bool wasMoving = false;

    void Update()
    {
        bool isMoving = IsPlayerMoving();

        if (isMoving && !wasMoving)
        {
            audioSource.Play();
        }
        else if (!isMoving && wasMoving)
        {
            audioSource.Stop();
        }

        wasMoving = isMoving;
    }

    bool IsPlayerMoving()
    {
        return characterController != null && characterController.velocity.magnitude > movementThreshold;
    }
}
