using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color activatedColor;
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            GetComponent<SpriteRenderer>().color = activatedColor;
            FindObjectOfType<Player>().SetCheckpoint(transform.position);
        }
    }
}
