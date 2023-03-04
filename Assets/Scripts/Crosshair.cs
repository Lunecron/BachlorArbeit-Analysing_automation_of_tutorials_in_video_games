using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private RectTransform crosshair;

    public GameObject player;
    public Transform cam;
    private LayerMask whatIsGrappleable;
    private LayerMask whatIsSwingable;
    private float grappleDistance;
    private float swingDistance;


    public Image[] crosshairImages;


    private Color defaultColor;
    public Color swingableColor;
    public Color grappableColor;


    private void Start()
    {
        defaultColor = crosshairImages[0].color;
        whatIsGrappleable = player.GetComponent<Grappling>().whatIsGrappleable;
        whatIsSwingable = player.GetComponent<Swinging>().whatIsGrappleable;
        grappleDistance = player.GetComponent<Grappling>().grappleDistance;
        swingDistance = player.GetComponent<Swinging>().maxSwingDistance;
    }

    private void Update()
    {
        CheckForGrappableWall();
        
    }

    private void CheckForGrappableWall()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position , cam.forward , out hit ,grappleDistance ) && (((1 << hit.collider.gameObject.layer) & whatIsGrappleable) != 0))
        {
                foreach (Image crosshairImage in crosshairImages)
                {
                    crosshairImage.color = grappableColor;
                }

        }
        else if (Physics.Raycast(cam.position, cam.forward, out hit, swingDistance) && (((1 << hit.collider.gameObject.layer) & whatIsSwingable) != 0))
        {
                foreach (Image crosshairImage in crosshairImages)
                {
                    crosshairImage.color = swingableColor;
                }            
        }
        else
        {
            foreach (Image crosshairImage in crosshairImages)
            {
                crosshairImage.color = defaultColor;
            }
        }
    }
}
