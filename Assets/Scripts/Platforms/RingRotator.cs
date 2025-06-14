using UnityEngine;

// This script continuously rotates the GameObject it is attached to.
public class RingRotator : MonoBehaviour
{
    // The speed is public so our spawner can change it for each ring.
    public float rotationSpeed = 20f;

    void Update()
    {
        // Rotate the ring around the world's Y-axis (up).
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
