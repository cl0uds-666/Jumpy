using UnityEngine;

// This is the definitive camera for the game.
// It orbits the center of the world, keeping the tower centered.
// It smoothly follows the player's angle and height.
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The player's Transform to follow.")]
    [SerializeField] private Transform playerTransform;

    [Header("Camera Positioning")]
    [Tooltip("The camera's fixed distance from the center of the tower.")]
    [SerializeField] private float distance = 15f;
    [Tooltip("The camera's fixed height above the player.")]
    [SerializeField] private float heightOffset = 7f;

    [Header("Smoothing - TUNE THESE")]
    [Tooltip("How quickly the camera rotates to follow the player's angle. A smaller value is slower and feels more 'lazy'.")]
    [SerializeField] private float rotationSmoothTime = 3f;
    [Tooltip("How quickly the camera moves up to follow the player's height. A smaller value is faster.")]
    [SerializeField] private float heightSmoothTime = 0.5f;

    // --- Private variables for internal state ---
    private float highestYReached = -Mathf.Infinity; // Tracks the highest point for vertical movement.
    private float currentCameraAngle = 0f;           // The camera's current angle around the tower.
    private float heightVelocity = 0f;               // Used by SmoothDamp for vertical movement.

    void LateUpdate()
    {
        // Failsafe in case the player object is missing.
        if (playerTransform == null)
        {
            return;
        }

        // --- Step 1: Calculate the Target Vertical Position ---

        // We only ever want the camera to move up, so we track the highest point the player has reached.
        if (playerTransform.position.y > highestYReached)
        {
            highestYReached = playerTransform.position.y;
        }
        // The camera's target height is based on this high watermark, plus an offset.
        float targetHeight = highestYReached + heightOffset;

        // --- Step 2: Calculate the Target Rotational Position ---

        // We get the player's current angle around the world's Y-axis.
        // Atan2 gives an angle in radians, so we convert it to degrees.
        float playerAngle = Mathf.Atan2(playerTransform.position.x, playerTransform.position.z) * Mathf.Rad2Deg;

        // We use LerpAngle to smoothly move our camera's current angle towards the player's angle.
        // This creates the smooth, "lazy" follow effect.
        currentCameraAngle = Mathf.LerpAngle(currentCameraAngle, playerAngle, rotationSmoothTime * Time.deltaTime);

        // --- Step 3: Combine and Apply ---

        // Convert our final smoothed angle into a rotation.
        Quaternion finalRotation = Quaternion.Euler(0, currentCameraAngle, 0);

        // Calculate the camera's final position by starting at the center of the world,
        // applying the rotation, and moving back by the desired distance.
        Vector3 finalPosition = Vector3.zero - (finalRotation * Vector3.forward * distance);

        // Now, we smoothly adjust the camera's height to the target height.
        finalPosition.y = Mathf.SmoothDamp(transform.position.y, targetHeight, ref heightVelocity, heightSmoothTime);

        // Apply the final calculated position.
        transform.position = finalPosition;

        // Finally, make the camera always look at the center of the tower at the player's current height.
        // This keeps the view stable and focused on the action.
        transform.LookAt(new Vector3(0, playerTransform.position.y, 0));
    }
}
