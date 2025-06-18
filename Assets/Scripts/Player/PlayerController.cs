using UnityEngine;

// We now also require the PlayerSoundManager to be on the same GameObject.
[RequireComponent(typeof(AirborneRotator), typeof(PlayerSoundManager))]
public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Gameplay Toggles")]
    [SerializeField] private bool inheritAirborneMomentum = true; // SerializedField allows for changing this in the inspector without making it public

    [Header("Explosion on Hit")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;

    // --- Private State ---
    private Rigidbody rb;
    private AirborneRotator airborneRotator;
    private ModelRotator modelRotator;
    private PlayerSoundManager soundManager; // Reference to the new sound manager

    private bool isGrounded;
    private bool jumpRequested;
    private float lastGroundedPlatformSpeed = 0f;
    private float jumpCooldown;
    private const float JUMP_COOLDOWN_TIME = 0.2f;
    private bool isFalling = false;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        airborneRotator = GetComponent<AirborneRotator>();
        modelRotator = GetComponentInChildren<ModelRotator>();
        soundManager = GetComponent<PlayerSoundManager>(); // Get the new component
        airborneRotator.enabled = false;
    }

    void Update()
    {
        if (isGrounded && Input.GetMouseButtonDown(0))
        {
            jumpRequested = true;
        }

        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (jumpCooldown <= 0)
        {
            GroundCheck();
        }

        if (jumpRequested)
        {
            jumpRequested = false;
            PerformJump();
        }
    }

    private void GroundCheck()
    {
        if (Physics.SphereCast(transform.position, 0.2f, Vector3.down, out RaycastHit hit, 0.5f, groundLayer))
        {
            if (!isGrounded)
            {
                HandleLanding(hit.transform);
            }
            isGrounded = true;
            HandleParenting(hit.transform);
        }
        else
        {
            if (isGrounded)
            {
                HandleLeavingGround();
            }
            isGrounded = false;

            if (rb.velocity.y < -1.5f && !isFalling)
            {
                isFalling = true;
                // Assuming AudioManager is a Singleton for global sounds like wind
                AudioManager.Instance.StartFallingWind();
            }
        }
    }

    private void PerformJump()
    {
        jumpCooldown = JUMP_COOLDOWN_TIME;
        transform.parent = null;
        isGrounded = false;

        // Note: The public 'inheritAirborneMomentum' from the first script was likely meant
        // to be controlled by a settings manager. The second script references GameSettings.Instance.
        // I will use GameSettings.Instance as it seems to be the intended final implementation.
        if (GameSettings.Instance.InheritAirborneMomentum)
        {
            airborneRotator.rotationSpeed = lastGroundedPlatformSpeed;
            airborneRotator.enabled = true;
        }

        if (modelRotator != null)
        {
            modelRotator.JumpedOffPlatform(lastGroundedPlatformSpeed);
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // We now call our local PlayerSoundManager for the jump sound.
        soundManager.PlayJumpSound();
    }

    private void HandleLanding(Transform platform)
    {
        isFalling = false;
        AudioManager.Instance.StopFallingWind();

        // We call our local PlayerSoundManager for the landing sound.
        soundManager.PlayLandingSound();

        RingRotator rotator = platform.GetComponentInParent<RingRotator>();
        if (rotator != null)
        {
            lastGroundedPlatformSpeed = rotator.rotationSpeed;
            if (modelRotator != null) { modelRotator.LandedOnPlatform(lastGroundedPlatformSpeed); }
        }
        else
        {
            lastGroundedPlatformSpeed = 0;
            if (modelRotator != null) { modelRotator.LandedOnPlatform(0); }
        }
    }

    private void HandleParenting(Transform platform)
    {
        // Only parent if we aren't already parented to something.
        if (transform.parent == null)
        {
            RingRotator rotator = platform.GetComponentInParent<RingRotator>();
            if (rotator != null)
            {
                airborneRotator.enabled = false;
                transform.parent = rotator.transform;
            }
        }
    }

    private void HandleLeavingGround()
    {
        if (GameSettings.Instance.InheritAirborneMomentum)
        {
            airborneRotator.rotationSpeed = lastGroundedPlatformSpeed;
            airborneRotator.enabled = true;
        }
        transform.parent = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is from below (hitting the top of a segment)
        if (collision.GetContact(0).normal.y < -0.5f)
        {
            PlatformSegmentController segment = collision.gameObject.GetComponent<PlatformSegmentController>();
            if (segment != null)
            {
                segment.Explode(explosionForce, explosionRadius);
                TimeManager.Instance.DoSlowmotion();

                // --- THIS IS THE FIX from the second script ---
                // We now call the local PlayerSoundManager for the explosion sound.
                soundManager.PlayExplosionSound();

                transform.parent = null;
                isGrounded = false;
                if (GameSettings.Instance.InheritAirborneMomentum) { airborneRotator.enabled = true; }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            isFalling = false;
            AudioManager.Instance.StopFallingWind();

            // We use the player's specific landing sound on impact.
            soundManager.PlayLandingSound();

            UIManager.Instance.ShowGameOver();
            this.enabled = false;
            rb.isKinematic = true;
            transform.parent = null;
            airborneRotator.enabled = false;
            if (modelRotator != null) { modelRotator.StopAllRotation(); }
        }
    }
}