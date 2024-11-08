using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    [SerializeField]
    InputActionReference movement;
    [SerializeField]
    public Rigidbody2D body;
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

    private float moveSpeed, jumpHeight;
    private float baseGravity = 3f;
    private float maxFallSpeed = 18f;
    private float fallSpeedMultiplier = 4f;
    private float playerMass = 10f;

    [Header("Player Base Values")]
    [SerializeField]
    public float MmoveSpeed = 10f;
    public float MjumpHeight = 15;
    public float MmaxFallSpeed = 25f;
    public float MfallSpeedMultiplier = 1f;
    public float MplayerMass = 10f;


    [Header("Player Large Values")]
    [SerializeField]
    public float LmoveSpeed = 6f;
    public float LmaxFallSpeed = 12f;
    public float LfallSpeedMultiplier = 0.5f;
    public float LplayerMass = 0f;
    public float LfallSpeedFactor = 1.2f;
    public float LjumpHeight = 7f;


    [Header("Player Small Values")]
    [SerializeField]
    public float SmoveSpeed = 15f;
    public float SmaxFallSpeed = 35f;
    public float SfallSpeedMultiplier = 4f;
    public float SplayerMass = 30f;
    public float SjumpHeight = 5f;



    [Header("Player Ground Check Var")]
    Vector2 movementInput;
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask ground;

    [Header("Player Size Change Var")]
    public float growScaleFactor = 1.5f;
    public float shrinkScaleFactor = 0.5f;

    

    //public float maxSizeFloatForce = 35f; // Force applied when floating
    //private bool isInFanZone; // Check if player is in the fan zone
    //private bool isMaxSize = false;   // Check if player is at max size
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

    private PlayerSizeState _prevSizeState;
    private PlayerMoveState _playerMoveState;

    private Vector3 originalScale; // Store original size for shrinking back
    private float radius;
    private Vector3 lastCheckpointPosition; // Store the checkpoint position
    public Vector3 respawnOriginalPos = new Vector3(-14, 0, 0); // Store the checkpoint position
    private string lastCheckpointName; // Store the checkpoint name
    private bool hasCheckpoint;

    // Metrics
    private Dictionary<PlayerSizeState, int> sizeStateCounts = new Dictionary<PlayerSizeState, int>();
    private Dictionary<String, int> checkpointRespawnCounts = new Dictionary<String, int>();
    private Dictionary<PlayerSizeState, float> sizeStateTimeSpent = new Dictionary<PlayerSizeState, float>();
    private float lastStateChangeTime; // To store the time of the last state change
    private Dictionary<Vector2, int> positionCounts = new Dictionary<Vector2, int>();

    private Dictionary<String, Dictionary<PlayerSizeState, float>> stateSizeTimeSpentPerChkpt = new Dictionary<String, Dictionary<PlayerSizeState, float>>();
    private float lastStateChangeTimeperCheckpoint;
    private float recordInterval = 0.5f; // Record every 0.5 seconds
    private float timer = 0f;

    //variables for hinge reset
    // float resetDelay = 2.0f; // delay in seconds
    // float timer = 0f;
    // bool isInteracting = fa/lse;
    // float originalAngle;
    // public HingeJoint2D hingeJoint;

    // Start is called before the first frame update
    void Start()
    {
        _playerSizeState = PlayerSizeState.STATE_MED;
        originalScale = transform.localScale; // Save the ball's original size
        radius = circleCollider.radius;

        // Initialize the size state counts
        // Initialize the size state time tracker
        // sizeStateTimeSpent[PlayerSizeState.STATE_SMALL] = 0f;
        // sizeStateTimeSpent[PlayerSizeState.STATE_MED] = 0f;
        // sizeStateTimeSpent[PlayerSizeState.STATE_LARGE] = 0f;
        lastCheckpointName = "Start";
        InitializeSizeStateCounts(sizeStateTimeSpent);

        // TODO: Obtain number of checkpoints dynamically
        stateSizeTimeSpentPerChkpt["Start"] = new Dictionary<PlayerSizeState, float>();
        stateSizeTimeSpentPerChkpt["Checkpoint 1"] = new Dictionary<PlayerSizeState, float>();
        stateSizeTimeSpentPerChkpt["Checkpoint 2"] = new Dictionary<PlayerSizeState, float>();
        stateSizeTimeSpentPerChkpt["Checkpoint 3"] = new Dictionary<PlayerSizeState, float>();
        stateSizeTimeSpentPerChkpt["Checkpoint 4"] = new Dictionary<PlayerSizeState, float>();

        InitializeSizeStateCounts(stateSizeTimeSpentPerChkpt["Start"]);
        InitializeSizeStateCounts(stateSizeTimeSpentPerChkpt["Checkpoint 1"]);
        InitializeSizeStateCounts(stateSizeTimeSpentPerChkpt["Checkpoint 2"]);
        InitializeSizeStateCounts(stateSizeTimeSpentPerChkpt["Checkpoint 3"]);
        InitializeSizeStateCounts(stateSizeTimeSpentPerChkpt["Checkpoint 4"]);


        lastStateChangeTime = Time.time; // Record the start time
        lastStateChangeTimeperCheckpoint = Time.time; // Record the start time

        moveSpeed = MmoveSpeed;
        jumpHeight = MjumpHeight;

        // hingeJoint = GetComponent<HingeJoint2D>();
        // originalAngle = hingeJoint.jointAngle;  
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

        //// Check if player is at max size and set isMaxSize accordingly
        //isMaxSize = (_playerSizeState == PlayerSizeState.STATE_LARGE);

        ////Apply upward force when in the fan zone and player is at max size
        //if (isInFanZone && isMaxSize)
        //{   Debug.Log("Fan Zone");
        //    body.AddForce(Vector2.up * maxSizeFloatForce, ForceMode2D.Force);
        //}
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (other.CompareTag("FanZone"))
        //{
        //    isInFanZone = false;
        //    Debug.Log("Exited Fan Zone");
        //}
    }

    public PlayerSizeState getPlayerSize()
    {
        return _playerSizeState;
    }

    private void HandleMovement()
    {
        if (isGrounded)
        {
            if (_playerSizeState == PlayerSizeState.STATE_LARGE)
            {
                ShrinkBall();
            }
            if (!isOnSlope && !isJumping)
            {
                body.velocity = new Vector2(movementInput.x * moveSpeed, body.velocity.y);
            }
            else if (isOnSlope && !isJumping)
            {

                body.velocity = new Vector2(-movementInput.x * slopeNormalPerp.x * moveSpeed, -movementInput.x * slopeNormalPerp.y * moveSpeed);
            }
        }
        else
        {
            body.velocity = new Vector2(movementInput.x * moveSpeed * 0.7f, body.velocity.y);
        }
    }

    private void Update()
    {
        HandleMovement();
        timer += Time.deltaTime;
        if (timer >= recordInterval)
        {
            Vector2 position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            if (positionCounts.ContainsKey(position))
            {
                positionCounts[position]++;
            }
            else
            {
                positionCounts[position] = 1;
            }
            timer = 0f;
        }

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
                _prevSizeState = _playerSizeState;
                _playerSizeState = PlayerSizeState.STATE_LARGE;
            }
            else
            {
                _prevSizeState = _playerSizeState;
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
                _prevSizeState = _playerSizeState;
                _playerSizeState = PlayerSizeState.STATE_SMALL;
            }
            else
            {
                _prevSizeState = _playerSizeState;
                _playerSizeState = PlayerSizeState.STATE_MED;
            }
            ChangePlayerSizeState();
        }
    }

    private void ChangePlayerSizeState()
    {
        // Before switching the state, calculate time spent in the current state
        Debug.Log("Current chkpt: " + lastCheckpointName);
        Debug.Log("Time spent in the current state: " + (Time.time - lastStateChangeTime));
        Debug.Log("Current state before switching: " + _playerSizeState);
        Debug.Log("Prev state before switching: " + _prevSizeState);
        float currentStateTime = Time.time - lastStateChangeTime; // Time spent in the current state
        float currentStateTimePerCheckpoint = Time.time - lastStateChangeTimeperCheckpoint; // Time spent in the current state
        sizeStateTimeSpent[_prevSizeState] += currentStateTime;
        Debug.Log("Updated sizeStateTimeSpent");
        stateSizeTimeSpentPerChkpt[lastCheckpointName][_prevSizeState] += currentStateTimePerCheckpoint;
        Debug.Log("Updated stateSizeTimeSpentPerChkpt");

        // Perform the state change
        if (_playerSizeState == PlayerSizeState.STATE_MED)
        {
            transform.localScale = originalScale;
            jumpHeight = MjumpHeight;
            fallSpeedMultiplier = MfallSpeedMultiplier;
            moveSpeed = MmoveSpeed;
            playerMass = MplayerMass;
            maxFallSpeed = MmaxFallSpeed;
            radius = circleCollider.radius;
        }
        else if (_playerSizeState == PlayerSizeState.STATE_SMALL)
        {
            transform.localScale = originalScale * shrinkScaleFactor;
            jumpHeight = SjumpHeight;
            fallSpeedMultiplier = SfallSpeedMultiplier;
            moveSpeed = SmoveSpeed;
            playerMass = SplayerMass;
            maxFallSpeed = SmaxFallSpeed;
            radius = circleCollider.radius * shrinkScaleFactor;
        }
        else if (_playerSizeState == PlayerSizeState.STATE_LARGE && !isGrounded)
        {
            transform.localScale = originalScale * growScaleFactor;
            jumpHeight = LjumpHeight;
            fallSpeedMultiplier = LfallSpeedMultiplier;
            moveSpeed = LmoveSpeed;
            playerMass = LplayerMass;
            maxFallSpeed = LmaxFallSpeed;
            radius = circleCollider.radius * growScaleFactor;
        }

        // Reset last state change time to now
        lastStateChangeTime = Time.time;
        lastStateChangeTimeperCheckpoint = Time.time;

        // Update the size state count
        Debug.Log("Player size state: " + _playerSizeState);

    }

    public void SetCheckpoint(Vector3 position, string checkpointName)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        lastCheckpointName = checkpointName;
    }

    public void Respawn()
    {
        if (hasCheckpoint)
        {
            transform.position = lastCheckpointPosition;
            // Update the respawn count for the current checkpoint 
            if (!checkpointRespawnCounts.ContainsKey(lastCheckpointName))
            {
                // if the checkpoint position does not exist in the dictionary, add it
                Debug.Log("Checkpoint position added to the dictionary. Position: " + lastCheckpointPosition + " Name: " + lastCheckpointName);
                checkpointRespawnCounts[lastCheckpointName] = 1;
            }
            Debug.Log("Respawn count for checkpoint " + lastCheckpointPosition + ": " + checkpointRespawnCounts[lastCheckpointName]);
            checkpointRespawnCounts[lastCheckpointName]++;
        }
        else
        {
            transform.position = respawnOriginalPos;
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
    public GameObject googleMetricsSender;
    public DiamondManager dm;
    public GameObject DoorOpenText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Collided with: " + other.gameObject.name);
        //if (other.CompareTag("FanZone"))
        //{
        //    isInFanZone = true;
        //    Debug.Log("Entered the xFan Zone");
        //}

        if (other.CompareTag("Diamond_Tag"))
        {
            Debug.Log("Collected a diamond!");
            Destroy(other.gameObject);
            dm.diamondCount++;

            int totalDiamonds = dm.GetTotalDiamonds();
            int seventyPercentDiamonds = Mathf.FloorToInt(totalDiamonds * 0.75f);
            Debug.Log("75% of total diamonds: " + seventyPercentDiamonds);

            if (dm.diamondCount >= seventyPercentDiamonds)
            {
                Hex_KeyPlate.SetActive(false);  // Deactivate the Hex_KeyPlate
                Debug.Log("Hex KeyPlate deactivated as 75% of diamonds were collected.");
                DoorOpenText.SetActive(true);
            }
        }

        if (other.CompareTag("Spike_Tag"))
        {
            Debug.Log("Hit a spike! Respawning...");
            Respawn();
        }

        if (other.CompareTag("Checkpoint_Tag"))
        {
            Debug.Log("Checkpoint reached!");
            float currentStateTimePerCheckpoint = Time.time - lastStateChangeTimeperCheckpoint;
            Debug.Log("Time spent in the current state (" + _playerSizeState + "): " + currentStateTimePerCheckpoint);

            stateSizeTimeSpentPerChkpt[lastCheckpointName][_playerSizeState] += currentStateTimePerCheckpoint;
            Debug.Log("Updated stateSizeTimeSpentPerChkpt");
            lastStateChangeTimeperCheckpoint = Time.time;
            Debug.Log("Checkpoint position added to the dictionary. Position: " + other.transform.position + " Name: " + other.GameObject().name);
            SetCheckpoint(other.transform.position, other.GameObject().name);

        }

        if (other.CompareTag("End_Plate"))
        {
            endText.SetActive(true);

            float finalStateTime = Time.time - lastStateChangeTime;
            Debug.Log("Time spent in the final state: " + finalStateTime);
            Debug.Log("Final state: " + _playerSizeState);
            sizeStateTimeSpent[_playerSizeState] += finalStateTime;
            Debug.Log("Updated sizeStateTimeSpent");
            stateSizeTimeSpentPerChkpt[lastCheckpointName][_playerSizeState] += finalStateTime;
            Debug.Log("Reached the end, sending metrics...");
            string mostCommonSizeState = GetLongestState().ToString(); // Convert enum to string
            // Checkpoint position added to the dictionary. Position: (-9.44, 16.89, 0.00)
            // Checkpoint position added to the dictionary. Position: (-2.14, 1.36, 0.00)
            // Checkpoint position added to the dictionary. Position: (25.45, 11.03, 0.00)
            // Checkpoint position added to the dictionary. Position: (51.34, 10.90, 0.00)
            // Checkpoint position added to the dictionary. Position: (9.17, -6.06, 0.00)



            // print key and value
            foreach (var kvp in checkpointRespawnCounts)
            {
                Debug.Log("Checkpoint: " + kvp.Key + " Respawn count: " + kvp.Value);
            }
            string checkpoint1 = "Checkpoint 1";
            string checkpoint2 = "Checkpoint 2";
            string checkpoint3 = "Checkpoint 3";
            string checkpoint4 = "Checkpoint 4";
            string checkpoint5 = "Checkpoint 5";
            string respawnCount1 = GetRespawnCount(checkpoint1).ToString(); // Example position for checkpoint 1
            string respawnCount2 = GetRespawnCount(checkpoint2).ToString(); // Example position for checkpoint 1
            string respawnCount3 = GetRespawnCount(checkpoint3).ToString(); // Example position for checkpoint 2
            string respawnCount4 = GetRespawnCount(checkpoint4).ToString(); // Example position for checkpoint 1
            string respawnCount5 = GetRespawnCount(checkpoint5).ToString(); // Example position for checkpoint 2
            Debug.Log("Most common size state: " + mostCommonSizeState);
            Debug.Log("Respawn count for checkpoint 1: " + respawnCount1);
            Debug.Log("Respawn count for checkpoint 2: " + respawnCount2);
            Debug.Log("Respawn count for checkpoint 3: " + respawnCount3);
            Debug.Log("Respawn count for checkpoint 4: " + respawnCount4);
            Debug.Log("Respawn count for checkpoint 5: " + respawnCount5);
            Debug.Log("Heatmap Coordinates: " + GetSerializedDataHeatmap());
            // Debug.Log("Data: " + stateSizeTimeSpentPerChkpt);
            foreach (var kvp in stateSizeTimeSpentPerChkpt)
            {
                Debug.Log("Checkpoint: " + kvp.Key);
                foreach (var kvp2 in kvp.Value)
                {
                    Debug.Log("Size state: " + kvp2.Key + " Time spent: " + kvp2.Value);
                }
            }
            List<string> jsonEntries = new List<string>();

            foreach (var checkpoint in stateSizeTimeSpentPerChkpt)
            {
                string checkpointKey = checkpoint.Key;
                List<string> stateEntries = new List<string>();

                foreach (var stateEntry in checkpoint.Value)
                {
                    stateEntries.Add($"\"{stateEntry.Key}\": {stateEntry.Value}");
                }

                string stateJson = "{" + string.Join(", ", stateEntries) + "}";
                jsonEntries.Add($"\"{checkpointKey}\": {stateJson}");
            }

            string finalJson = "{" + string.Join(", ", jsonEntries) + "}";
            Debug.Log(" JSON Data: " + finalJson);
            googleMetricsSender.GetComponent<SendToGoogle>().Send(mostCommonSizeState, respawnCount1, respawnCount2, respawnCount3, respawnCount4, respawnCount5, GetSerializedDataHeatmap());
            Debug.Log("Metrics sent!");

            StartCoroutine(LoadSceneAfterDelay());

            IEnumerator LoadSceneAfterDelay()
            {
                // Wait for 1 seconds
                yield return new WaitForSeconds(1f);

                // Load the scene
                SceneManager.LoadScene("LevelSelectionScene");
            }

        }
    }

    public GameObject fiftyPercentText;

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
                    if (UnityEngine.ColorUtility.TryParseHtmlString("#9D4649", out newColor))
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
            else
            {
                Debug.Log("Brick collision count: " + brick);
            }
        }

        if (other.gameObject.CompareTag("Hex_Key_Tag")) // Check for the HEXKey
        {
            HEXKey.SetActive(false);
            //Hex_KeyPlate.SetActive(false); // Destroy hexkey_plate object
            fiftyPercentText.SetActive(true);
            Debug.Log("Touched Hex_Key_Tag");
        }
    }

    // Get the most common size state
    public PlayerSizeState GetMostCommonSizeState()
    {
        PlayerSizeState mostCommonState = PlayerSizeState.STATE_SMALL;
        int maxCount = 0;
        foreach (var state in sizeStateCounts)
        {
            if (state.Value > maxCount)
            {
                maxCount = state.Value;
                mostCommonState = state.Key;
            }
        }
        return mostCommonState;
    }

    // Get respawn count for a specific checkpoint
    public int GetRespawnCount(string lastCheckpointPosition)
    {
        if (checkpointRespawnCounts.ContainsKey(lastCheckpointPosition))
        {
            return checkpointRespawnCounts[lastCheckpointPosition];
        }
        return 0;
    }

    public PlayerSizeState GetLongestState()
    {
        PlayerSizeState longestState = PlayerSizeState.STATE_SMALL;
        float maxTime = 0f;

        foreach (var kvp in sizeStateTimeSpent)
        {
            // print the key and value
            Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value);
            if (kvp.Value > maxTime)
            {
                maxTime = kvp.Value;
                longestState = kvp.Key;
            }
        }

        return longestState;
    }
    public string GetSerializedDataHeatmap()
    {
        return JsonUtility.ToJson(new SerializableDictionaryHeatmap(positionCounts));
    }


    private void InitializeSizeStateCounts(Dictionary<PlayerSizeState, float> stateDict)
    {
        stateDict[PlayerSizeState.STATE_SMALL] = 0f;
        stateDict[PlayerSizeState.STATE_MED] = 0f;
        stateDict[PlayerSizeState.STATE_LARGE] = 0f;
    }

}

[System.Serializable]
public class SerializableDictionaryHeatmap
{
    public List<Vector2> keys = new List<Vector2>();
    public List<int> values = new List<int>();

    public SerializableDictionaryHeatmap(Dictionary<Vector2, int> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }
}

[System.Serializable]
public class NestedDictionaryEntry
{
    public string state;
    public float timeSpent;

    public NestedDictionaryEntry(string state, float timeSpent)
    {
        this.state = state;
        this.timeSpent = timeSpent;
    }
}


