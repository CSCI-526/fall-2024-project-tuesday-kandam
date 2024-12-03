using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanImageRotate : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second

    void Update()
    {
        // Rotate the image around the Z-axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
