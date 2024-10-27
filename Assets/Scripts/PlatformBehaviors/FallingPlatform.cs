using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private float destroyDelay = 2f;
    private float fallDelay = 0.2f;
    [SerializeField]
    private Rigidbody2D rb;

    // Minimum impact
    public float impactImpulseThreshold = 20f; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null && player.getPlayerSize() == Player.PlayerSizeState.STATE_SMALL)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (playerRb != null)
                {
                    float totalImpulse = 0f;
                    foreach (ContactPoint2D contact in collision.contacts)
                    {
                        totalImpulse += contact.normalImpulse; // impulse for each contact point
                    }

                    Debug.Log("Total impact impulse: " + totalImpulse);

                    // Check if the totalImpulse meets the threshold BEFORE checking the contact normals
                    if (totalImpulse > impactImpulseThreshold)
                    {
                        // Check if the player is landing on top of the platform
                        foreach (ContactPoint2D contact in collision.contacts)
                        {
                            if (contact.normal.y < -0.5f) // Ensure it's negative to indicate downward
                            {
                                Debug.Log("Player has sufficient impact impulse, starting fall, normal: " + contact.normal.y + ", totalImp: " + totalImpulse + ", curr tresh: "+impactImpulseThreshold);
                                StartCoroutine(Fall());
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Impact impulse is not high enough (impulse: " + totalImpulse + ")");
                    }
                }
            }
        }
    }


    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);        
    }
}
