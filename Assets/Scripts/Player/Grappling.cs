using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{

    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lineRenderer;

    [Header("Grappling")]
    public float grappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    public float overshootXZAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCD;
    private float grapplingCDTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    private bool grappling;


    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(grapplingKey) && !grappling)
        {
            StartGrapple();
        }
        else if(Input.GetKeyUp(grapplingKey) && grappling)
        {
            StopGrapple();
        }

        if(grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }
        
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if(grapplingCDTimer > 0 || !pm.CanGrapple())
        {
            return;
        }

        

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward , out hit, grappleDistance, whatIsGrappleable))
        {
            pm.freeze = true;
            grappling = true;

            grapplePoint = hit.point;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, grapplePoint);

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * grappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        

    }

    private void ExecuteGrapple()
    {
        //use gravity if it was deactivated before
        if(pm.gameObject.GetComponent<Rigidbody>().useGravity == false)
        {
            pm.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - pm.playerHeight / 2, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if(grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        Vector3 overShootDirection = (grapplePoint -transform.position).normalized;
        Vector3 overShootGrapplePoint = new Vector3(grapplePoint.x + overShootDirection.x* overshootXZAxis ,grapplePoint.y,grapplePoint.z + overShootDirection.z *overshootXZAxis);

        pm.JumpToPosition(overShootGrapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;
        pm.activeGrapple = false;
        grappling = false;
        grapplingCDTimer = grapplingCD;

        lineRenderer.positionCount = 0;

        lineRenderer.enabled = false;

    }


}
