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

    public Image[] crosshairImages;

    private Color defaultColor;
    public Color hookableColor;


    private void Start()
    {
        defaultColor = crosshairImages[0].color;
        whatIsGrappleable = player.GetComponent<Grappling>().whatIsGrappleable;
    }

    private void Update()
    {
        CheckForGrappableWall();
        
    }

    private void CheckForGrappableWall()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position , cam.forward , out hit , player.GetComponent<Grappling>().grappleDistance ,whatIsGrappleable))
        {
            foreach(Image crosshairImage in crosshairImages)
            {
                crosshairImage.color = hookableColor;
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
