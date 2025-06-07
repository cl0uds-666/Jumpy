using UnityEngine;

public class PlatformSegmentController : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This public method is called when the player hits a platform
    public void Explode(float explosionForce, float explosionRadius)
    {
        // --- THE REAL FIX ---
        // We scale the explosion force by the current Time.timeScale.
        // The TimeManager has already slowed time down *in the same frame*, so Time.timeScale is already low (e.g., 0.2f).
        // This ensures the initial "kick" from the explosion is also reduced, making it look correct in slow motion.
        float scaledExplosionForce = explosionForce * Time.timeScale;

        // Turn off "Is Kinematic" so this segment can be affected by physics.
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Find all colliders within the explosion radius.
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

                // Add the EXPLOSION with the NEW SCALED force.
                hitRb.AddExplosionForce(scaledExplosionForce, transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
            }
        }
    }

    // This is called by the LevelSpawner to reset the segment.
    public void ResetSegment()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
