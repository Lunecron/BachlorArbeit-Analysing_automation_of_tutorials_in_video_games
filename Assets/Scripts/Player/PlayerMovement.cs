using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float swingSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float drasticalMoveSpeedChangeAmount = 4f;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private RaycastHit headHit;

    public float standUpBufferDistance = 0.01f;

    //Hookdelay for Grappling Script;
    public float hookDelay = 0.1f;


    [Header("Ground Check")]
    [SerializeField] float groundBufferDistance = 0.2f;
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopAngle;
    [SerializeField] float slopeBufferDistance = 0.3f;
    private RaycastHit slopeHit;
    private bool exitingSlope = true;

    [Header("Ground Check")]
    private Vector3 velocityToSet;

    [Header("StateChecks")]
    public bool restricted;
    public bool sliding;
    public bool wallrunning;
    public bool crouching;
    public bool tryStandUp;
    public bool freeze;
    public bool activeGrapple;
    public bool enableMovementOnNextTouch;
    public bool swinging;

    public bool walljumping;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    public Transform orientation;
    public Transform PlayerObject;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    Swinging playerSwing;

    public MovementState state;

    public enum MovementState
    {
        walking,
        restricted,
        sprinting,
        crouching,
        sliding,
        wallrunning,
        freeze,
        air,
        swinging
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerSwing = GetComponent<Swinging>();

        rb.freezeRotation = true;
        readyToJump = true;
        sliding = false;

        startYScale = PlayerObject.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndHandleGround();

        MyInput();
        SpeedControl();
        StateHandler();

        if (tryStandUp)
        {
            StandUp();
        }
    }

    private void FixedUpdate()
    {
        if (state != MovementState.restricted)
        {
            MovePlayer();
        }
            
    }

    #region StateHandler
    //Change State depending on Player Input / Location
    private void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 0f;
            rb.velocity = Vector3.zero;
        }

        else if (restricted)
        {
            state = MovementState.restricted;
        }
        
        else if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
        }

        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }

        }


        // Mode - Crouching
        else if (Input.GetKey(crouchKey) || tryStandUp )
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }


        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        // Mode - Walking
        else if(grounded){
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

        }

        // check if desiredMoveSpeed has changed drastically
        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > drasticalMoveSpeedChangeAmount && movementSpeed != 0 && !Input.GetKey(crouchKey))
        {
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMoveSpeed());
        }
        else
        {
            movementSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;

    }

    #endregion


    

    //Check all inputs and execute responding movement
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCD);
        }

        //start crouching
        if (Input.GetKeyDown(crouchKey) && !sliding)
        {
            PlayerObject.localScale = new Vector3(PlayerObject.localScale.x, crouchYScale, PlayerObject.localScale.z);
            crouching = true;

            // Add force to push player on ground
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        }
        else if (Input.GetKeyDown(crouchKey) && sliding)
        {
            crouching = true;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //stop crouching
        if (Input.GetKeyUp(crouchKey) && !sliding)
        {
            StandUp();
        }
    }




    //Movement Handling for Player
    private void MovePlayer()
    {
        //calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Disable movement while grapple
        if (activeGrapple)
        {
            return;
        }

        //Disable movement while swing
        if (swinging)
        {
            return;
        }

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * movementSpeed * 20f, ForceMode.Force);

            // AddForce to avoid bouncing while jumping on a slope
            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        // on ground
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        // in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //turn gravity off while on slope (avoid sliding down)
        if (!wallrunning)
        {
            
            rb.useGravity = !OnSlope();
        }
    }

    #region Check for Movement
    //Check if Player is Grounded and Add Drag
    private void CheckAndHandleGround()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundBufferDistance, whatIsGround);

        //handle drag
        if (grounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private bool CanStandUp()
    {
        if (Physics.Raycast(transform.position, Vector3.up, out headHit, playerHeight + standUpBufferDistance))
        {
            if (headHit.transform.tag == "Player")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        return true;
    }

    //Trying to Stand Up if Possible
    public void StandUp()
    {
        if (CanStandUp() && !Input.GetKey(crouchKey))
        {
            PlayerObject.localScale = new Vector3(PlayerObject.localScale.x, startYScale, PlayerObject.localScale.z);
            crouching = false;
            tryStandUp = false;
        }
        else
        {
            tryStandUp = true;
        }

    }


    // Check if Player on a slope
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + slopeBufferDistance))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopAngle && angle != 0;
        }
        return false;
    }

    public bool CanSwing()
    {
        if (activeGrapple || swinging)
        {
            return false;
        }
        return true;
    }

    public bool CanGrapple()
    {
        if (activeGrapple || swinging )
        {
            return false;
        }
        return true;
    }

    #endregion

    #region Speed adjustments
    //Limit Speed of Player which is added trough AddForce
    private void SpeedControl()
    {
        //Disable speedControl while grapple
        if (activeGrapple)
        {
            return;
        }

        //limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > movementSpeed)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
        }

        //limit speed on ground/air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //limit velocity if needed
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }


        
    }


    private IEnumerator SmoothLerpMoveSpeed()
    {
        // smooth lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - movementSpeed);
        float startValue = movementSpeed;

        while (time < difference)
        {
            movementSpeed = Mathf.Lerp(startValue, movementSpeed, time / difference);
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            yield return null;
        }

        movementSpeed = desiredMoveSpeed;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public void SetVelocity()
    {
        rb.velocity = velocityToSet;
        enableMovementOnNextTouch = true;
    }

    #endregion

    #region Jumping
    // Execute Jump
    // Neutralice YForce first and add jumpForce
    public void Jump()
    {
        exitingSlope = true;
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    public void ResetJump()
    {
        
        readyToJump = true;
        exitingSlope = false;
    }

    public void JumpToPosition(Vector3 tragetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, tragetPosition, trajectoryHeight);

        Invoke(nameof(SetVelocity), hookDelay);
    }



    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;

            activeGrapple = false;
            GetComponent<Grappling>().StopGrapple();
        }
    }

    

    // Calculate Forward Direction On Slopes/Ground and return forward direction for use in AddForce
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
