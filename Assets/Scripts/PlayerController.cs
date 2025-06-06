using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f; // The force applied when jumping
    private Rigidbody rb; // Caches the Rigidbody component

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Gets the Rigidbody component from this GameObject
    }

    // Update is called once per frame
    void Update()
    {
        // Check for a left mouse click or a screen tap
        if (Input.GetMouseButtonDown(0))
        {
            // Apply an immediate upward force to the Rigidbody
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
