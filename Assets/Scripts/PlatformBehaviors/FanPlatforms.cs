using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanPlatforms : MonoBehaviour
{
    public float fanStrength = 35;
    void Start()
    {
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            Player player = other.transform.root.gameObject.GetComponent<Player>();
            if (player != null && player.getPlayerSize() == Player.PlayerSizeState.STATE_LARGE)
            {
                Debug.Log("Fan Zone");
                player.body.AddForce(Vector2.up * fanStrength, ForceMode2D.Force);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
