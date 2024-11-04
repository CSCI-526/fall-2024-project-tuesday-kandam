using UnityEngine;

public class SeesawSpringEffect : MonoBehaviour
{
    private float springStrength = 3f;     // Controls how strong the "spring" effect is
    private float dampingFactor = 2f;      // Controls the damping to avoid too much oscillation
    private float maxAngle = 90f;           // Maximum tilt angle (matches hinge joint limits)

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Calculate the angle difference from the horizontal position (0 degrees)
        float currentAngle = transform.localEulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;  // Adjust angle to -180 to 180 range

        // Only apply torque within the max angle limits
        if (Mathf.Abs(currentAngle) < maxAngle)
        {
            // Calculate spring force to return to horizontal (0 degrees)
            float torque = -currentAngle * springStrength - rb.angularVelocity * dampingFactor;
            rb.AddTorque(torque);
        }
    }
}
