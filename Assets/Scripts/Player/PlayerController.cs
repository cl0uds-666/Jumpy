using UnityEngine;

[RequireComponent(typeof(AirborneRotator))]
public class PlayerController : MonoBehaviour
{
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Gameplay Toggles (Developer Default)")]
    public bool inheritAirborneMomentum = true;

    [Header("Explosion on Hit")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;

    // --- Private State ---
    private Rigidbody rb;
    private AirborneRotator airborneRotator;
    private ModelRotator modelRotator;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        airborneRotator = GetComponent<AirborneRotator>();
        modelRotator = GetComponentInChildren<ModelRotator>();

        airborneRotator.enabled = false;
    }

    void Update()
    {
        if (isGrounded && Input.GetMouseButtonDown(0))
        {
            isGrounded = false;

            float lastPlatformSpeed = 0;
            if (transform.parent != null)
            {
                RingRotator parentRotator = transform.parent.GetComponent<RingRotator>();
                if (parentRotator != null) { lastPlatformSpeed = parentRotator.rotationSpeed; }
            }

            // --- THE FIX: Use correct casing 'InheritAirborneMomentum' ---
            if (GameSettings.Instance.InheritAirborneMomentum)
            {
                airborneRotator.rotationSpeed = lastPlatformSpeed;
                airborneRotator.enabled = true;
            }

            if (modelRotator != null) { modelRotator.JumpedOffPlatform(lastPlatformSpeed); }

            transform.parent = null;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            AudioManager.Instance.PlayJumpSound();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.GetContact(0).normal.y < -0.5f)
        {
            HandleHeadCollision(collision);
            return;
        }

        if (collision.GetContact(0).normal.y > 0.5f)
        {
            HandleLanding(collision);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (isGrounded && transform.parent == null)
        {
            RingRotator rotator = collision.gameObject.GetComponentInParent<RingRotator>();
            if (rotator != null)
            {
                airborneRotator.enabled = false;
                transform.parent = rotator.transform;
            }
        }
    }

    private void HandleLanding(Collision collision)
    {
        if (!isGrounded)
        {
            isGrounded = true;
            AudioManager.Instance.PlayLandingSound();
            AudioManager.Instance.StopFallingWind();

            float landingPlatformSpeed = 0;
            RingRotator rotator = collision.gameObject.GetComponentInParent<RingRotator>();
            if (rotator != null)
            {
                landingPlatformSpeed = rotator.rotationSpeed;
            }
            if (modelRotator != null) { modelRotator.LandedOnPlatform(landingPlatformSpeed); }
        }
    }

    private void HandleHeadCollision(Collision collision)
    {
        PlatformSegmentController segment = collision.gameObject.GetComponent<PlatformSegmentController>();
        if (segment != null)
        {
            segment.Explode(explosionForce, explosionRadius);
            TimeManager.Instance.DoSlowmotion();
            AudioManager.Instance.PlayExplosionSound();
            transform.parent = null;
            isGrounded = false;
            // --- THE FIX: Use correct casing 'InheritAirborneMomentum' ---
            if (GameSettings.Instance.InheritAirborneMomentum) { airborneRotator.enabled = true; }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            AudioManager.Instance.StopFallingWind();
            AudioManager.Instance.PlayLandingSound();

            UIManager.Instance.ShowGameOver();
            this.enabled = false;
            rb.isKinematic = true;
            transform.parent = null;
            airborneRotator.enabled = false;
            if (modelRotator != null) { modelRotator.StopAllRotation(); }
        }
    }
}
