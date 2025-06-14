using UnityEngine;

// This script rotates the player around the world origin (0,0,0)
// ONLY when it is enabled.
public class AirborneRotator : MonoBehaviour
{
    // This will be set by the PlayerController when the player jumps.
    public float rotationSpeed = 0f;

    void Update()
    {
        // We rotate the player around the world's center point.
        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
