using UnityEngine;

// This script requires a Trail Renderer and a Rigidbody to be on the same GameObject.
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class TrailController : MonoBehaviour
{
    [Header("Trail Settings")]
    [Tooltip("The trail will only appear when the player's speed is above this value.")]
    [SerializeField] private float minSpeedToShowTrail = 1f;

    // --- Private component references ---
    private TrailRenderer trailRenderer;
    private Rigidbody rb;

    void Awake()
    {
        // Get the components from this GameObject.
        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody>();

        // Start with the trail not emitting.
        trailRenderer.emitting = false;
    }

    void Update()
    {
        // We check the magnitude (overall speed) of the Rigidbody's velocity.
        if (rb.velocity.magnitude > minSpeedToShowTrail)
        {
            // If the player is moving fast enough, turn the trail on.
            trailRenderer.emitting = true;
        }
        else
        {
            // If the player is moving slowly or is stationary, turn the trail off.
            trailRenderer.emitting = false;
        }
    }
}
