using UnityEngine;

[RequireComponent(typeof(AirborneRotator))]
public class PlayerController : MonoBehaviour
{
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Gameplay Toggles (Developer Default)")]
    [Tooltip("Sets the default value for the airborne momentum feature at the start of the game.")]
    public bool inheritAirborneMomentum = true; // MUST BE PUBLIC

    [Header("Explosion on Hit")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;

    private Rigidbody rb;
    private AirborneRotator airborneRotator;
    private bool wasAirborne = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        airborneRotator = GetComponent<AirborneRotator>();
        airborneRotator.enabled = false;
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < -0.1f) { wasAirborne = true; }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameSettings.Instance.InheritAirborneMomentum)
            {
                if (transform.parent != null)
                {
                    RingRotator parentRotator = transform.parent.GetComponent<RingRotator>();
                    if (parentRotator != null) { airborneRotator.rotationSpeed = parentRotator.rotationSpeed; }
                }
                airborneRotator.enabled = true;
            }
            else
            {
                airborneRotator.enabled = false;
            }

            transform.parent = null;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            AudioManager.Instance.PlayJumpSound();
            wasAirborne = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.GetContact(0).normal.y < -0.5f)
        {
            PlatformSegmentController segment = collision.gameObject.GetComponent<PlatformSegmentController>();
            if (segment != null)
            {
                segment.Explode(explosionForce, explosionRadius);
                TimeManager.Instance.DoSlowmotion();
                AudioManager.Instance.PlayExplosionSound();
                transform.parent = null;
                if (GameSettings.Instance.InheritAirborneMomentum) { airborneRotator.enabled = true; }
            }
        }

        if (wasAirborne && collision.GetContact(0).normal.y > 0.5f)
        {
            AudioManager.Instance.PlayLandingSound();
            wasAirborne = false;
            RingRotator rotator = collision.gameObject.GetComponentInParent<RingRotator>();
            if (rotator != null)
            {
                airborneRotator.enabled = false;
                transform.parent = rotator.transform;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            UIManager.Instance.ShowGameOver();
            this.enabled = false;
            rb.isKinematic = true;
            transform.parent = null;
            airborneRotator.enabled = false;
        }
    }
}
