using UnityEngine;

public class WispSparkleReaction : MonoBehaviour
{
    public ParticleSystem sparkleParticles;
    public float speedMultiplier = 20f;  // Controls how much emission changes based on speed
    public float maxEmission = 100f;     // Max particles emitted per second

    private Rigidbody rb;
    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get Rigidbody (if used for movement)
        emissionModule = sparkleParticles.emission;
    }

    void Update()
    {
        // Calculate speed of the wisp
        float speed = rb ? rb.velocity.magnitude : (Vector3.Magnitude(transform.position - lastPosition) / Time.deltaTime);
        lastPosition = transform.position;

        // Adjust emission rate based on speed
        emissionModule.rateOverTime = Mathf.Clamp(speed * speedMultiplier, 10, maxEmission);
    }

    private Vector3 lastPosition;
}