using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsionPad : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {

        //float newDir = Mathf.Abs(collision.rigidbody.velocity.yf);
        Debug.Log(collision.relativeVelocity);
        collision.rigidbody.velocity =  new Vector2(0, -collision.relativeVelocity.y);
    }
}
