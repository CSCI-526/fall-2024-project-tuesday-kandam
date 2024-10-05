using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    InputActionReference movement;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private float moveSpeed, jumpHeight;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [Range(0f, 1f)]
    public float groundDecay;

    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 4f;
    private Vector2 lastVelocity;

    private float newSpeed;
    Vector2 movementInput;
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask ground;

    // New variables for growing and shrinking
    public float growScaleFactor = 1.25f; // Each step grows the ball by 25%
    public int growStep = 0; // Track how many times the ball has grown currently
    private int maxGrowSteps = 3; // Maximum number of growth steps
    public float fallSpeedFactor = 1.2f;
    public float jumpHeightFactor = 3f;

    public enum State
    {
        STATE_STANDING,
        STATE_JUMPING,
    };

    public State _state;
    public State _prevState;

    private Vector3 originalScale; // Store original size for shrinking back

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale; // Save the ball's original size
    }

    private void FixedUpdate()
    {
        _prevState = _state;
        ApplyFriction();
        Gravity();
    }

    private void Update()
    {
        body.velocity = new Vector2(movementInput.x * moveSpeed, body.velocity.y);
    }

    private void Gravity()
    {
        if(body.velocity.y < 0)
        {
            body.gravityScale = baseGravity * fallSpeedMultiplier;
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -maxFallSpeed));
        }
        else
        {
            body.gravityScale = baseGravity;
        }
    }
    private bool isGrounded()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, ground))
        {
            _state = State.STATE_STANDING;
            return true;
        }
        _state = State.STATE_JUMPING;
        return false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded())
        {
            if(context.performed)
            {
                body.velocity = new Vector2(body.velocity.x, jumpHeight);
            }
            else if( context.canceled)
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
            }
        }
        
    }

    public void ApplyFriction()
    {
        if(isGrounded() && movementInput.x == 0 && movementInput.y == 0)
        {
            body.velocity *= groundDecay;
        }
    }
    public void OnGrow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && growStep < maxGrowSteps)
        {
            GrowBall();
        }
    }

    public void OnShrink(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && growStep > 0)
        {
            ShrinkBall();
        }
    }

    // Method to grow the ball
    private void GrowBall()
    {
        if (growStep < maxGrowSteps)
        {
            transform.localScale *= growScaleFactor; // Increase the ball's size
            growStep++; // Increment grow step count

            // Adjust jump height and fall speed based on size
            jumpHeight += jumpHeightFactor; // Increase jump height
            fallSpeedMultiplier -= fallSpeedFactor; // Slow down fall
        }
    }

    // Method to shrink the ball
    private void ShrinkBall()
    {
        if (growStep > 0)
        {
            transform.localScale /= growScaleFactor; // Decrease the ball's size
            growStep--; // Decrement grow step count
            
            // Adjust jump height and fall speed based on size
            jumpHeight -= jumpHeightFactor; // Decrease jump height
            fallSpeedMultiplier += fallSpeedFactor; // Speed up fall
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    public GameObject endText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log the name of the collided object for debugging
        Debug.Log("Collided with: " + other.gameObject.name);

        // Check if the collided object has the correct tag
        if (other.CompareTag("Diamond_Tag")) 
        {
            Destroy(other.gameObject); // Destroy the diamond object
            Debug.Log("Diamond destroyed!");
        }

        if (other.CompareTag("End_Plate"))  // Check if the player collides with the platform
        {
            endText.SetActive(true); // Activate the text when the player reaches the platform
            Debug.Log("You Win!");
        }
    }



}
