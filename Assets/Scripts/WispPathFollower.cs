using UnityEngine;
using System.Collections;

public class WispPathFollower : MonoBehaviour
{
    public Transform[] waypoints;  // Assign the WispPoints in the Inspector
    public Transform player;       // Assign the Player (OVRCameraRig or XR Rig)
    public float moveSpeed = 2f;   // Wisp movement speed
    public float waitTime = 1.5f;  // Pause duration at each waypoint
    public float stopRadius = 5f;  // Distance at which the wisp stops if the player is too far
    public float resumeRadius = 3f; // Distance at which the wisp resumes moving

    private int currentWaypoint = 0;
    private bool isWaiting = false;

    void Start()
    {
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        while (currentWaypoint < waypoints.Length)
        {
            // Move toward the next waypoint
            while (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) > 0.1f)
            {
                // Check if the player is too far
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer > stopRadius)
                {
                    StartCoroutine(WaitForPlayer()); // Stop & wait
                    yield return new WaitUntil(() => Vector3.Distance(transform.position, player.position) < resumeRadius);
                }

                // Rotate the wisp to have its back facing forward (on the Y-axis)
                Vector3 direction = waypoints[currentWaypoint].position - transform.position;
                direction.y = 0; // Ignore vertical movement (we only care about the horizontal plane)
                transform.rotation = Quaternion.LookRotation(-direction); // Face opposite direction for the back to face forward

                // Move the wisp toward the waypoint
                transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Pause before moving to the next waypoint
            yield return new WaitForSeconds(waitTime);
            currentWaypoint++;
        }

        Debug.Log("Wisp has reached the final destination!");
    }

    IEnumerator WaitForPlayer()
    {
        if (!isWaiting)
        {
            isWaiting = true;
            Debug.Log("Wisp: 'Follow me!'"); // Replace with voice, text, or particle effects
            yield return new WaitForSeconds(3f);
            isWaiting = false;
        }
    }
}
