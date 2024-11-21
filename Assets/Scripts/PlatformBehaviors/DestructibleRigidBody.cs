using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleRigidBody : MonoBehaviour
{
    [SerializeField]
    Vector2 forceDirection;

    [SerializeField]
    float torque;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        float randTorque = UnityEngine.Random.Range(-50, 50);
        float randForceX = UnityEngine.Random.Range(forceDirection.x - 50, forceDirection.x + 50);
        float randForceY = UnityEngine.Random.Range(forceDirection.y - 50, forceDirection.y + 50);

        forceDirection.x = randForceX;
        forceDirection.y = randForceY;
        
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(forceDirection);
        rb.AddTorque(randTorque);
        Invoke("DestroySelf", UnityEngine.Random.Range(2.5f, 3.5f));
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    
    
}
