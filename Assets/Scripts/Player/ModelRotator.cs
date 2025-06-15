using UnityEngine;

// This script now has controls for faster, outward airborne spinning.
public class ModelRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How quickly the model snaps to its new facing direction when landing.")]
    [SerializeField] private float snapSpeed = 15f;
    [Tooltip("Multiplies the speed of the spin while in the air. >1 is faster, <1 is slower.")]
    [SerializeField] private float airborneSpinMultiplier = 2.5f; // Increased for a faster spin

    private float airborneSpinSpeed = 0f;
    private bool isSpinningInAir = false;
    private Quaternion targetRotation = Quaternion.identity;

    void Update()
    {
        if (isSpinningInAir)
        {
            // Apply the continuous airborne spin.
            transform.Rotate(Vector3.up, airborneSpinSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            // Smoothly turn towards the target direction when on a platform.
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, snapSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Called by the PlayerController when landing on a platform.
    /// </summary>
    public void LandedOnPlatform(float platformSpeed)
    {
        isSpinningInAir = false;

        // Your corrected values for facing the direction of travel.
        if (platformSpeed > 0)
        {
            targetRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (platformSpeed < 0)
        {
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            targetRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Called by the PlayerController when jumping off a platform.
    /// </summary>
    public void JumpedOffPlatform(float platformSpeed)
    {
        isSpinningInAir = true;

        // --- THE FIX FOR BOTH ISSUES ---
        // 1. We multiply by -1 to reverse the direction, making it spin "outwards".
        // 2. We multiply by our new airborneSpinMultiplier to control the speed.
        airborneSpinSpeed = -platformSpeed * airborneSpinMultiplier;
    }

    /// <summary>
    /// Called on Game Over to stop all rotation.
    /// </summary>
    public void StopAllRotation()
    {
        isSpinningInAir = false;
        this.enabled = false;
    }
}
