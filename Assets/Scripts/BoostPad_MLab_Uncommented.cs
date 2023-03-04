using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad_MLab_Uncommented : MonoBehaviour
{
    [Header("Boosting")]
    public bool normalBoosting = true;
    public Vector3 boostDirection;
    public float boostForce;

    public bool localBoosting = false;
    public float boostLocalForwardForce;
    public float boostLocalUpwardForce;

    public float boostDuration = 1f;

    [SerializeField] bool disablePlayerInput = false;
    [SerializeField] float disableTime = 1f;
    private float disableTimer = 0;

    private PlayerMovement pm = null;

    private void Awake()
    {
        pm = FindObjectOfType<PlayerMovement>();
    }
    private void Update()
    {
        if (disablePlayerInput)
        {
            if (disableTimer > 0)
            {
                disableTimer -= Time.deltaTime;
                if(disableTimer <= 0)
                {
                    EnablePlayerInput();
                }
            }

        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AddForce(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        AddForce(collision.collider);
    }

    private void DisablePlayerInput()
    {
        pm.disableMovement = true;
    }

    private void EnablePlayerInput()
    {
        pm.disableMovement = false;
    }

    private void AddForce(Collider other)
    {
       

        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
                 

            pm = other.GetComponentInParent<PlayerMovement>();
            if (disablePlayerInput)
            {
                if (pm.gameObject.GetComponent<Sliding>())
                {
                    pm.gameObject.GetComponent<Sliding>().StopSlide();
                }
                    DisablePlayerInput();
                disableTimer = disableTime;
            }

            Rigidbody rb = pm.GetComponent<Rigidbody>();
            rb.useGravity = true;

            if (normalBoosting)
                rb.velocity = Vector3.zero;
                rb.AddForce(boostDirection.normalized * boostForce, ForceMode.Impulse);

            if (localBoosting)
            {
                Vector3 localBoostedDirection = pm.orientation.forward * boostLocalForwardForce + pm.orientation.up * boostLocalUpwardForce;
                rb.AddForce(localBoostedDirection, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!normalBoosting) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + boostDirection);
    }
}
