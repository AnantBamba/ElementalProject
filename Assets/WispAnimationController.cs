using UnityEngine;

public class WispAnimationController : MonoBehaviour
{
    public Animator animator;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate movement speed based on position change
        float speed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // Activate animation if speed is above a threshold
        animator.SetBool("isMoving", speed > 0.05f);
    }
}