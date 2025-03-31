using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioSource footstepSource;   // Assign AudioSource
    public AudioClip[] footstepClips;    // Array of footstep sounds
    public float stepInterval = 0.5f;    // Time between steps

    private CharacterController characterController;
    private float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Get the VR player’s movement controller
    }

    void Update()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f) // Detect movement
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer if player stops
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            footstepSource.clip = footstepClips[Random.Range(0, footstepClips.Length)]; // Pick random footstep sound
            footstepSource.Play();
        }
    }
}