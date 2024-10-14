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
    private Vector3 checkpointPosition; // Store the checkpoint position
    private bool hasCheckpoint; 

    // Start is called before the first frame update
    void Start()
    {
        _playerSizeState = PlayerSizeState.STATE_MED;
        originalScale = transform.localScale; // Save the ball's original size
        minSize = originalScale * shrinkScaleFactor;
        maxSize = originalScale * growScaleFactor;
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
    private void Update()
    {
        if (isGrounded && !isOnSlope && !isJumping)
        {
            body.velocity = new Vector2(movementInput.x * moveSpeed, 0);
        }
        else if (isGrounded && isOnSlope && !isJumping)
        {
            body.velocity = new Vector2(-movementInput.x * slopeNormalPerp.x * moveSpeed, -movementInput.x * slopeNormalPerp.y * moveSpeed);
        }
        else if (!isGrounded)
        {
            body.velocity = new Vector2(movementInput.x * moveSpeed, body.velocity.y);
        }
        contSizeChange();
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, circleCollider.radius);
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

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
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
    public void ContGrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            growing = true;
        }
        else if (context.canceled)
        {
            growing = false;
        }
    }

    public void ContShrink(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            shrinking = true;
        }
        else if (context.canceled)
        {
            shrinking = false;
        }
    }
    private void contSizeChange()
    {
        if (growing)
        {
            t += 0.01f;
            if (t > 1)
            {
                t = 1;
                return;
            }
            transform.localScale = Vector3.Lerp(minSize, maxSize, t);
            jumpHeight = Mathf.Lerp(minJump, maxJump, t);
            fallSpeedMultiplier = Mathf.Lerp(maxfSpeedMult, minfSpeedMult, t);
            moveSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
            playerMass = Mathf.Lerp(maxMass, minMass, t);
            maxFallSpeed = Mathf.Lerp(maxFSpeed, minFSpeed, t);
        }
        else if (shrinking)
        {
            t -= 0.01f;
            if (t < 0)
            {
                t = 0;
                return;
            }
            transform.localScale = Vector3.Lerp(minSize, maxSize, t);
            jumpHeight = Mathf.Lerp(minJump, maxJump, t);
            fallSpeedMultiplier = Mathf.Lerp(maxfSpeedMult, minfSpeedMult, t);
            moveSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
            playerMass = Mathf.Lerp(maxMass, minMass, t);
            maxFallSpeed = Mathf.Lerp(maxFSpeed, minFSpeed, t);
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
        }
        else if (_playerSizeState == PlayerSizeState.STATE_SMALL)
        {
            transform.localScale = originalScale * shrinkScaleFactor;
            jumpHeight = 5;
            fallSpeedMultiplier = 2.5f;
            moveSpeed = 15;
            playerMass = 30f;
            maxFallSpeed = 35;
        }
        else if (_playerSizeState == PlayerSizeState.STATE_LARGE)
        {
            transform.localScale = originalScale * growScaleFactor;
            jumpHeight = 14;
            fallSpeedMultiplier = 0.1f;
            moveSpeed = 6;
            playerMass = 0f;
            maxFallSpeed = 8f;
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
        if (other.gameObject.CompareTag("mario_brick"))
        {
            brick++; // Increment the counter

            // Make the special object visible only after 4 collisions
            if (brick >= 4)
            {
                HEXKey.SetActive(true); // Make the object visible
                Debug.Log("Special object is now visible!");
            }
            else
            {
                Debug.Log("Brick collision count: " + brick);
            }
        }

        if (other.gameObject.CompareTag("Hex_Key_Tag")) // Check for the HEXKey
        {
            HEXKey.SetActive(false);
            Hex_KeyPlate.SetActive(false); // Destroy hexkey_plate object
            Debug.Log("Touched Hex_Key_Tag and destroyed Hex_Key_Plate!");
        }
    }

}