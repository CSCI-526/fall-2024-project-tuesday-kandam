using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color activatedColor = new Color32(83, 105, 213, 255);
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Collision detected with: " + collision.name); // Log the name of the object that collided

        // Get the root GameObject of the collision object
        GameObject rootObject = collision.transform.root.gameObject;

        // Check if the root object is tagged as "Player"
        if (rootObject.CompareTag("Player"))
        {
            // Debug.Log("Collided with Player"); // Log when the collision is with the Player

            if (!isActivated)
            {
                isActivated = true; // Mark the checkpoint as activated
                // Debug.Log("Checkpoint is now activated"); // Confirm activation
                ChangeColorOfChildren(); // Change the color of child objects

                // Find the player and set the checkpoint
                Player player = rootObject.GetComponent<Player>();
                if (player != null)
                {
                    // Debug.Log("Player component found on root object"); // Confirm Player component
                    player.SetCheckpoint(transform.position, gameObject.name); // Pass the position and the name of this checkpoint
                }
            }
        }
    }

    private void ChangeColorOfChildren()
    {
        // Get all child SpriteRenderer components and update their color
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            // Debug.LogWarning("No SpriteRenderer components found in children of " + gameObject.name); // Warn if no SpriteRenderers are found
            return;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                // Debug.Log("Changing color of " + spriteRenderer.gameObject.name + " to " + activatedColor); // Log each color change
                spriteRenderer.color = activatedColor;
            }
            else
            {
                Debug.LogWarning("SpriteRenderer is null for one of the children of " + gameObject.name); // Warn if a SpriteRenderer is null
            }
        }
    }
}
