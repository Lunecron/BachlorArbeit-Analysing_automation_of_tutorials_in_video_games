using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    // Script Combination with PlayerCam
    // Handle Camera in an Extra Object and use this script to set camera to correct position

    public Transform cameraPosition;


    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
