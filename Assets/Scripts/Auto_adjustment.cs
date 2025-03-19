using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_adjustment : MonoBehaviour
{
    public float radius = 0.5f; // The radius of the bounding sphere for triggering the collision event
    public GameObject specificObject; // Reference to the specific object
    public ParticleSystem particleSystem;
    private Vector3 targetPosition;

    private void Start()
    {
        // Add a sphere collider with the specified radius to this object
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = radius;
        sphereCollider.isTrigger = true;

        particleSystem.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When another object enters the collider attached to this object, move to its center
        if (other.gameObject == specificObject)
        {
            BoxCollider boxCollider = other.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                targetPosition = boxCollider.bounds.center;
                Debug.Log("yes");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move the sphere towards the target position only when targetPosition is set
        if (targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5f);

            // If the sphere is close enough to the target, stop moving, turn off the particle system, and reset targetPosition
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                particleSystem.gameObject.SetActive(false);
                targetPosition = Vector3.zero;
            }
        }
    }
}