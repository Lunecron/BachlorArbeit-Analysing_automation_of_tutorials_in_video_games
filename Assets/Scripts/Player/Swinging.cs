using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip;
    public Transform cam;
    public Transform player;
    public LayerMask whatIsGrappleable;
    private LayerMask allLayer = -1;
    public PlayerMovement pm;

    [Header("Swinging")]
    public float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimer = 0;

    [Header("AirMovement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;


    void Start()
    {
        
    }


    void Update()
    {
        if (coyoteTimer > 0)
        {
            coyoteTimer -= Time.deltaTime;
            StartSwing();
        }
        else if (Input.GetKeyDown(swingKey))
        {
            coyoteTimer = coyoteTime;
            StartSwing();
        }
        else if (Input.GetKeyUp(swingKey) || (!Input.GetKey(swingKey) && pm.swinging == true))
        {
            StopSwing();
        }



    }

    private void FixedUpdate()
    {
        //air movement on hook
        if (joint != null)
        {
            AirMovement();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartSwing()
    {
        if (!pm.CanSwing())
        {
            return;
        }
        
        RaycastHit hit;
        if(Physics.Raycast(cam.position,cam.forward , out hit , maxSwingDistance, allLayer, QueryTriggerInteraction.Ignore))
        {
            if (((1 << hit.collider.gameObject.layer) & whatIsGrappleable) != 0)
            {
                //It matched one

                pm.swinging = true;
                swingPoint = hit.point;
                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = swingPoint;

                float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

                //distance grapple will try to keep from grapplepoint
                joint.maxDistance = distanceFromPoint * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;

                //curstomized values
                joint.spring = 4.5f;
                joint.damper = 7f;
                joint.massScale = 4.5f;

                lr.enabled = true;
                lr.positionCount = 2;
                currentGrapplePosition = gunTip.position;
            }
            
            
        }
    }

    void StopSwing()
    {
        pm.swinging = false;
        lr.positionCount = 0;
        lr.enabled = false;
        Destroy(joint);
    }

    void DrawRope()
    {
        if (!joint)
        {
            return;
        }

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    private void AirMovement()
    {
        // right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        // left
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        // forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-orientation.forward * horizontalThrustForce * Time.deltaTime);

            //float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            //joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            //joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }


}
