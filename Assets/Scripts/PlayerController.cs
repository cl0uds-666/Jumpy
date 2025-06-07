using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Explosion on Hit")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;

    private Rigidbody rb;
    private bool isGrounded; // We'll add a proper ground check later

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Simple jump for now
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // This function is called by Unity's physics engine when a collision occurs.
    void OnCollisionEnter(Collision collision)
    {
        // We check the direction of the collision to see if we hit the underside of a platform.
        // A "normal" is a vector that points directly away from the surface of the collision.
        // If we hit something from below, the normal will point downwards.
        if (collision.GetContact(0).normal.y < -0.5f)
        {
            // Try to get the PlatformSegmentController from the object we hit.
            PlatformSegmentController segment = collision.gameObject.GetComponent<PlatformSegmentController>();

            // If the object we hit has the script, it's a platform segment.
            if (segment != null)
            {
                // --- Start the chain reaction! ---
                // 1. Tell the segment to explode itself and its neighbours.
                segment.Explode(explosionForce, explosionRadius);

                // 2. Tell the TimeManager to start the slow-motion effect.
                TimeManager.Instance.DoSlowmotion();
            }
        }
    }
}
