using UnityEngine;

public class PlatformSegmentController : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This public method will be called when the player hits a platform
    public void Explode(float explosionForce, float explosionRadius)
    {
        // Turn off "Is Kinematic" so this segment can be affected by physics.
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Find all colliders within the explosion radius around this segment.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Loop through all the colliders we found.
        foreach (Collider hit in colliders)
        {
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();

            // Check if the nearby object has a Rigidbody and is a platform segment.
            if (hitRb != null && hit.GetComponent<PlatformSegmentController>() != null)
            {
                // Make sure the nearby segment also becomes non-kinematic.
                hitRb.isKinematic = false;

                // Add an explosive force that pushes it up and away from this segment's position.
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
            }
        }
    }

    // This will be called by our spawner when we reuse the platforms.
    public void ResetSegment()
    {
        // Re-enable "Is Kinematic" to make the platform static again for its next use.
        if (rb != null)
        {
            rb.isKinematic = true;
            // Also reset its velocity and rotation from the explosion.
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
