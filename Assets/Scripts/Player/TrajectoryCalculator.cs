using UnityEngine;

public class TrajectoryCalculator : MonoBehaviour
{
    /// <summary>
    /// Calculates how long a Rigidbody will be in the air given a jump.
    /// </summary>
    /// <returns>The time in seconds.</returns>
    public float CalculateAirTime(Rigidbody rb, float jumpForce, float height)
    {
        // Calculate initial vertical velocity based on an impulse force
        float initialVelocity = jumpForce / rb.mass;
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Using the kinematic equation: v_final^2 = v_initial^2 + 2*a*d
        // We can find the velocity at the peak of the jump.
        float velocityAtPeak = Mathf.Sqrt(Mathf.Max(0, (initialVelocity * initialVelocity) - (2 * gravity * height)));

        // Time to reach peak: t = (v_final - v_initial) / a
        float timeToPeak = (velocityAtPeak - initialVelocity) / -gravity;

        return timeToPeak;
    }
}
