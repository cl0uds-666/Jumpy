using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Explosion on Hit")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;

    private Rigidbody rb;
    private bool wasAirborne = false; // Tracks if we were recently in the air

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Using FixedUpdate for consistent physics checks
    void FixedUpdate()
    {
        // If our vertical velocity is negative, it means we are falling.
        if (rb.velocity.y < -0.1f)
        {
            wasAirborne = true;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            AudioManager.Instance.PlayJumpSound();
            wasAirborne = true; // Jumping immediately makes us airborne
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the underside of a platform
        if (collision.GetContact(0).normal.y < -0.5f)
        {
            PlatformSegmentController segment = collision.gameObject.GetComponent<PlatformSegmentController>();
            if (segment != null)
            {
                segment.Explode(explosionForce, explosionRadius);
                TimeManager.Instance.DoSlowmotion();
                AudioManager.Instance.PlayExplosionSound();
            }
        }

        // Check if we landed on top of a surface
        // We also check 'wasAirborne' to ensure this only fires once on landing, not while sliding.
        if (wasAirborne && collision.GetContact(0).normal.y > 0.5f)
        {
            AudioManager.Instance.PlayLandingSound();
            wasAirborne = false; // We are no longer airborne, so reset the flag
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            UIManager.Instance.ShowGameOver();
            this.enabled = false;
            rb.isKinematic = true;
        }
    }
}
