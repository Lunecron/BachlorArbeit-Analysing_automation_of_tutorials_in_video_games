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

    public Image[] crosshairImages;

    private Color defaultColor;
    public Color swingableColor;
    public Color grappableColor;


    private void Start()
    {
        defaultColor = crosshairImages[0].color;
        whatIsGrappleable = player.GetComponent<Grappling>().whatIsGrappleable;
        whatIsSwingable = player.GetComponent<Swinging>().whatIsGappleable;
    }

    private void Update()
    {
        CheckForGrappableWall();
        
    }

    private void CheckForGrappableWall()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position , cam.forward , out hit , player.GetComponent<Grappling>().grappleDistance ,whatIsGrappleable) )
        {
            foreach(Image crosshairImage in crosshairImages)
            {
                crosshairImage.color = grappableColor;
            }
        }
        else if (Physics.Raycast(cam.position, cam.forward, out hit, player.GetComponent<Grappling>().grappleDistance, whatIsSwingable))
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
