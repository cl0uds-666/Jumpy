using UnityEngine;

public class PivotController : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 50f; // Speed of rotation

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // spins player around the pivot point
    }
}
