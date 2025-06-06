using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform playerTransform; // Assign your Player
    [SerializeField] private Transform pivotTransform;  // Assign your Pivot

    [Header("Camera Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 4f, -10f);

    private float highestY;

    // We check for the pivotTransform here to avoid errors if it's not assigned
    void Start()
    {
        if (pivotTransform == null)
        {
            Debug.LogError("Pivot Transform not assigned to Camera Controller!");
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null || pivotTransform == null)
        {
            return;
        }

        // --- Vertical Follow ---
        if (playerTransform.position.y > highestY)
        {
            highestY = playerTransform.position.y;
        }

        // --- Rotational Follow ---
        // Calculate the desired position based on the player's location...
        // ...but rotate the offset by the PIVOT's rotation. This is the key fix.
        Vector3 desiredPosition = playerTransform.position + (pivotTransform.rotation * offset);

        // Override the Y position with our "highest only" value
        desiredPosition.y = highestY + offset.y;

        // --- Smooth Movement ---
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // --- Look At Player ---
        transform.LookAt(playerTransform);
    }
}
