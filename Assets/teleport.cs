using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class teleport : MonoBehaviour
{
    public Transform destination;
    public GameObject player;

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        Debug.Log("Player enter");
        if (collision.CompareTag("Player"))
        {
            player.SetActive(false);
            // Teleport the player to the destination
            player.transform.position = destination.position;
            player.SetActive(true);
        }
    }
}
