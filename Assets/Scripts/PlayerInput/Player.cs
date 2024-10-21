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
    private CircleCollider2D circleCollider;
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [Header("Slope Var")]
    [SerializeField]
    private float slopeCheckDistance;
    private float slopeDownAngle;
    private Vector2 slopeNormalPerp;
    private bool isOnSlope;
    private float slopeDownAngleOld;
    private float slopeSideAngle;

    private bool isJumping;
    private bool isGrounded;

    [Header("Player Base Values")]
    [SerializeField]
    private float moveSpeed, jumpHeight;
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 4f;
    public float playerMass = 10f;
    public float fallSpeedFactor = 1.2f;
    public float jumpHeightFactor = 3f;

    [Header("Player Ground Check Var")]
    Vector2 movementInput;
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask ground;

    [Header("Player Size Change Var")]
    public float growScaleFactor = 1.5f;
    public float shrinkScaleFactor = 0.5f;

    [Header("Cont Ball Size Change Var")]
    public Vector3 maxSize;
    public Vector3 minSize;
    public float maxSpeed;
    public float minSpeed;
    public float maxMass;
    public float minMass;
    public float maxJump;
    public float minJump;
    public float maxFSpeed;
    public float minFSpeed;
    public float maxfSpeedMult;
    public float minfSpeedMult;
    private float t = 0.5f;
    private bool growing = false;
    private bool shrinking = false;

    public enum GroundState
    {
        STATE_STANDING,
        STATE_JUMPING,
    };

    public enum PlayerSizeState
    {
        STATE_SMALL,
        STATE_MED,
        STATE_LARGE
    };

    public enum PlayerMoveState
    {
        STATE_JUMPING,
        STATE_MOVING
    }

    public GroundState _groundState;
    public GroundState _prevState;
    private PlayerSizeState _playerSizeState;
    private PlayerMoveState _playerMoveState;

    private Vector3 originalScale; // Store original size for shrinking back
    private float radius;
    private Vector3 checkpointPosition; // Store the checkpoint position
    private bool hasCheckpoint; 

    // Start is called before the first frame update
    void Start()
    {
        _playerSizeState = PlayerSizeState.STATE_MED;
        originalScale = transform.localScale; // Save the ball's original size
        minSize = originalScale * shrinkScaleFactor;
        maxSize = originalScale * growScaleFactor;
        radius = circleCollider.radius;
    }

    private void OnEnable()
    {
        if (movement != null && movement.action != null)
        {
            movement.action.performed += Move;
            movement.action.canceled += Move;
        }
        else
        {
            Debug.LogWarning("Movement input action reference is not set in Player script.");
        }
    }

    private void OnDisable()
    {
        if (movement != null && movement.action != null)
        {
            movement.action.performed -= Move;
            movement.action.canceled -= Move;
        }
    }

    private void FixedUpdate()
    {
        _prevState = _groundState;
        Grounded();
        SlopeCheck();
        Gravity();
    }

    public PlayerSizeState getPlayerSize()
    {
        return _playerSizeState;
    }

    private void HandleMovement()
    {
        
        if (isGrounded && !isOnSlope && !isJumping)
        {
            body.velocity = new Vector2(movementInput.x * moveSpeed, body.velocity.y);
        }
        else if (isGrounded && isOnSlope && !isJumping)
        {
            body.velocity = new Vector2(-movementInput.x * slopeNormalPerp.x * moveSpeed, -movementInput.x * slopeNormalPerp.y * moveSpeed);
        }
        else if (!isGrounded)
        {
            body.velocity = new Vector2(movementInput.x * moveSpeed, body.velocity.y);
        }
    }
    private void Update()
    {
       HandleMovement();
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, radius);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, ground);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, ground);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

        Debug.DrawRay(checkPos, transform.right, Color.red);
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, ground);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
            slopeDownAngleOld = slopeDownAngle;

        }
    }

    private void Gravity()
    {
        if (body.velocity.y < 0)
        {
            body.gravityScale = baseGravity * fallSpeedMultiplier;
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -maxFallSpeed));
        }
        else
        {
            body.gravityScale = baseGravity;
        }
    }

    private bool Grounded()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, ground);

        if (body.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping)
        {
            _groundState = GroundState.STATE_STANDING;
            return true;
        }
        else
        {
            _groundState = GroundState.STATE_JUMPING;
        }

        return false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isJumping = true;
            if (context.performed)
            {
                body.velocity = new Vector2(body.velocity.x, jumpHeight);
            }
            else if (context.canceled)
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
            }
        }
    }
    public void OnGrow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            GrowBall();
        }
    }

    public void OnShrink(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ShrinkBall();
        }
    }
    private void GrowBall()
    {
        if (_playerSizeState != PlayerSizeState.STATE_LARGE)
        {
            if (_playerSizeState == PlayerSizeState.STATE_MED)
            {
                _playerSizeState = PlayerSizeState.STATE_LARGE;
            }
            else
            {
                _playerSizeState = PlayerSizeState.STATE_MED;
            }
            ChangePlayerSizeState();
        }
    }
    private void ShrinkBall()
    {
        if (_playerSizeState != PlayerSizeState.STATE_SMALL)
        {
            if (_playerSizeState == PlayerSizeState.STATE_MED)
            {
                _playerSizeState = PlayerSizeState.STATE_SMALL;
            }
            else
            {
                _playerSizeState = PlayerSizeState.STATE_MED;
            }
            ChangePlayerSizeState();
        }
    }
    private void ChangePlayerSizeState()
    {
        if (_playerSizeState == PlayerSizeState.STATE_MED)
        {
            transform.localScale = originalScale;
            jumpHeight = 5;
            fallSpeedMultiplier = 1;
            moveSpeed = 10;
            playerMass = 10f;
            maxFallSpeed = 25;
            radius = circleCollider.radius;
        }
        else if (_playerSizeState == PlayerSizeState.STATE_SMALL)
        {
            transform.localScale = originalScale * shrinkScaleFactor;
            jumpHeight = 5;
            fallSpeedMultiplier = 2.5f;
            moveSpeed = 15;
            playerMass = 30f;
            maxFallSpeed = 35;
            radius = circleCollider.radius * shrinkScaleFactor;
        }
        else if (_playerSizeState == PlayerSizeState.STATE_LARGE)
        {
            transform.localScale = originalScale * growScaleFactor;
            jumpHeight = 14;
            fallSpeedMultiplier = 0.1f;
            moveSpeed = 6;
            playerMass = 0f;
            maxFallSpeed = 8f;
            radius = circleCollider.radius * growScaleFactor;
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        hasCheckpoint = true; 
    }

    public void Respawn()
    {
        if (hasCheckpoint)
        {
            transform.position = checkpointPosition;
        }
        else
        {
            transform.position = new Vector3(-14, 0, 0);
        }

        // Reset the Rigidbody2D velocity to stop the player's movement.
        if (body != null)
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    public GameObject endText;
    private int brick = 0;
    public GameObject HEXKey;
    public GameObject Hex_KeyPlate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);

        if (other.CompareTag("Diamond_Tag"))
        {
            Debug.Log("Collected a diamond!");
            Destroy(other.gameObject); 
        }
        
        if (other.CompareTag("Spike_Tag"))
        {

            Debug.Log("Hit a spike! Respawning...");
            Respawn(); 
        }
        
        if (other.CompareTag("Checkpoint_Tag"))
        {
            Debug.Log("Checkpoint reached!");
            SetCheckpoint(transform.position); 
        }
        
        if (other.CompareTag("End_Plate"))
        {
            endText.SetActive(true);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Breakable_Brick"))
        {

            // Destroy the brick on collision
            Destroy(other.gameObject);
            Debug.Log("Brick destroyed!");
        }

        // Check if the object has the tag 'mario_brick'
        if (other.gameObject.CompareTag("mario_brick"))
        {
            brick++;  // Increment the counter

            // Get the SpriteRenderer component of the brick
            SpriteRenderer brickRenderer = other.gameObject.GetComponent<SpriteRenderer>();

            if (brick == 1)
            {
                // Change the color of the brick to the specified hex color
                if (brickRenderer != null)
                {
                    Color newColor;
                    if (ColorUtility.TryParseHtmlString("#9D4649", out newColor))
                    {
                        brickRenderer.color = newColor;  // Change to the specified hex color
                        Debug.Log("Brick color changed to #9D4649!");
                    }
                }
            }
            else if (brick >= 2)
            {
                // Destroy the brick after the second collision
                Destroy(other.gameObject);
                Debug.Log("Brick destroyed!");

                HEXKey.SetActive(true); // Make the object visible
                Debug.Log("Special object is now visible!");
            }
        }

        // Check if the object has the tag 'Hex_Key_Tag'
        if (other.gameObject.CompareTag("Hex_Key_Tag"))
        {
            // Destroy HEXKey and Hex_KeyPlate
            if (HEXKey != null)
            {
                Destroy(HEXKey);
                Debug.Log("HEXKey destroyed!");
            }

            if (Hex_KeyPlate != null)
            {
                Destroy(Hex_KeyPlate);
                Debug.Log("Hex_KeyPlate destroyed!");
            }
        }
    }


}