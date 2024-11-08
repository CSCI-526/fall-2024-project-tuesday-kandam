using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinePlatform : MonoBehaviour
{
    [SerializeField] GameObject platform;
    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] float speed = 4;
    [SerializeField] float delay = 1f;
    [SerializeField] float frequency = 1.0f;
    [SerializeField] float amplitude = 1.0f;
    [SerializeField] int platformIndex;
    private float phaseOffset;

    private Vector2 targetPosition;
    private Vector2 movementDirection;

    void Start()
    {
        movementDirection = (end.transform.position - start.transform.position).normalized;
        targetPosition = platform.transform.position;
        phaseOffset = platformIndex * Mathf.PI / 8; // Adjust the offset for each platform
    }

    void Update()
    {
        float time = Time.time;

        // Calculate the sine wave movement
        float wavePosition = amplitude * Mathf.Sin(frequency * time + phaseOffset);

        // Move platform along the line between startPosition and endPosition
        platform.transform.position = targetPosition + movementDirection * wavePosition;
    }
}
