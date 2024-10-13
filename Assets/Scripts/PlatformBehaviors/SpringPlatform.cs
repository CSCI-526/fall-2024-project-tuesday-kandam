using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    private SpringJoint2D springJoint;
    private Rigidbody2D rb;
    private float force = 0f;
    public float maxForce = 90f;
    public bool isHeavy = false;
    // Will add logic for checking if the balls actually on the platform
    public bool isTouching = false;
    // Might be used for timing mechanic
    private int powerIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        springJoint = GetComponent<SpringJoint2D>();
        springJoint.distance = 1f;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && player.getPlayerSize() == Player.PlayerSizeState.STATE_SMALL)
            {
                isHeavy = true;
            }
            else
            {
                isHeavy = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHeavy)
        {
            force = powerIndex * maxForce;
        }
        
    }

    private void FixedUpdate()
    {
        // When force is not 0
        if (force != 0)
        {

            springJoint.distance = 1.5f;
            rb.AddForce(Vector3.up * force);
            force = 0;
        }

        // When the plunger is held down
        if (isHeavy)
        {
            springJoint.distance = 1f;
            rb.AddForce(Vector3.down * 200);
        }
    }
}
