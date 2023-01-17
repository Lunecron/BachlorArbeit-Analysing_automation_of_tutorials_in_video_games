using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 200f;
    public float wallClimbSpeed;
    public float maxWallRunTime = 1f;
    public float wallRunTimer;
    public float minWallRunSpeed;

    [Header("Walljump")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 12f;
    private bool wallRemembered;
    private Transform lastWall;

    private int wallJumpsDone;

    [Header("Limitations")]
    public bool doJumpOnEndOfTimer = false;
    public bool resetDoubleJumpsOnNewWall = true;
    public bool resetDoubleJumpsOnEveryWall = false;
    public int allowedWallJumps = 1;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime = 0.2f;
    public float exitWallTimer;

    [Header("Input")]
    public KeyCode wallJumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;

    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance = 0.7f;
    public float minJumpHeight = 2f;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private float forwardSpeed;
    private bool wallLeft;
    private bool wallRight;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Camera Adjustment While Wallrunning")]
    public float FOV = 90f;
    private float startFOV;
    public float TiltAmount = 5f;
    

    [Header("Referecnes")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        if (whatIsWall.value == 0)
        {
            whatIsWall = LayerMask.GetMask("Default");
        }
           

        if (whatIsGround.value == 0)
        {
            whatIsGround = LayerMask.GetMask("Default");
        }
            
        startFOV = cam.GetComponent<Camera>().fieldOfView;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();


    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        if (pm.grounded && lastWall != null)
        {
            lastWall = null;
        }
            
    }

    private void FixedUpdate()
    {
        forwardSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        if (pm.wallrunning)
        {
            
            if (forwardSpeed >= minWallRunSpeed)
            {
                WallRunningMovement();
            }

        }
    }
    #region StateMachine

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall && forwardSpeed >= minWallRunSpeed)
        {
            // start wallrun
            if (!pm.wallrunning)
            {
                StartWallRun();
            }
            // wallrun timer
            wallRunTimer -= Time.deltaTime;

            if (wallRunTimer < 0 && pm.wallrunning)
            {
                if (doJumpOnEndOfTimer)
                    WallJump();

                else
                {
                    exitingWall = true;
                    exitWallTimer = exitWallTime;
                }
            }

            // wall jump
            if (Input.GetKeyDown(wallJumpKey)) WallJump();
        }

        // State 2 - Exiting
        else if (exitingWall)
        {
            pm.restricted = true;

            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }

        if (!exitingWall && pm.restricted)
            pm.restricted = false;
    }

    #endregion

    //check if wall is next to player
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -1 *orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);

        // reset readyToClimb and wallJumps whenever player hits a new wall
        if ((wallLeft || wallRight) && NewWallHit())
        {
            wallJumpsDone = 0;
            wallRunTimer = maxWallRunTime;
        }

    }

    private void RememberLastWall()
    {
        if (wallLeft)
        {
            lastWall = leftWallhit.transform;
        }
            

        if (wallRight)
        {
            lastWall = rightWallhit.transform;
        }
            
    }

    private bool NewWallHit()
    {
        if (lastWall == null)
        {
            return true;
        }
           

        else if (wallLeft && leftWallhit.transform != lastWall)
        {
            return true;
        }
            

        else if (wallRight && rightWallhit.transform != lastWall)
        {
            return true;
        }
            

        return false;
    }

    //Check if player does not stand on the ground
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.useGravity = useGravity;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        wallRemembered = false;

        //camera adjustments
        cam.DoFov(90f);

        if (wallLeft)
        {
            cam.DoTilt(-TiltAmount);
        }

        if (wallRight)
        {
            cam.DoTilt(TiltAmount);
        }
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;
        

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - (-wallForward)).magnitude)
        {
            wallForward = -wallForward;
        }

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //move up or down
        if (upwardsRunning && !downwardsRunning)
        {
            pm.wallclimbing = true;
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        else if(downwardsRunning && !upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }
        else
        {
            pm.wallclimbing = false;
        }



        //Push to wall
        if(!(wallLeft &&horizontalInput>0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        // remember the last wall

        if (!wallRemembered)
        {
            RememberLastWall();
            wallRemembered = true;
        }

        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
        
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        pm.wallclimbing = false;
        rb.useGravity = true;

        //reset camera effects
        cam.DoFov(startFOV);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        //exiting wall
        pm.walljumping = true;
        Invoke("EndOfWalljump",0.5f);
        RememberLastWall();
        exitingWall = true;        
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //reset y velocity and add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void EndOfWalljump()
    {
        pm.walljumping = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, orientation.right * wallCheckDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -orientation.right * wallCheckDistance);
    }


}
