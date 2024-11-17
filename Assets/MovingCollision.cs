using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCollision : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] Transform platform;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            
            other.gameObject.transform.parent.SetParent(platform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag.Equals(playerTag))
        {
            other.gameObject.transform.parent.SetParent(null); 
        }
    }
}
