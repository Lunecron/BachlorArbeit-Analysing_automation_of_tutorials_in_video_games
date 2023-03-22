using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    //Handle Mouse Movement and LockMouse

    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation = 0f;
    float yRotation = 0f;

    [Header("Camera Transition on Movement")]
    public float transitionTime = 0.25f;


    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;



    }

    // Update is called once per frame
    void Update()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.smoothDeltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.smoothDeltaTime * sensY;

        yRotation += mouseX;
        yRotation = yRotation % 360;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        //rotate cam and orientation
            camHolder.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        


    }

    public void LookAt(Vector3 lookPosition)
    {
        Vector3 normalDirection = new Vector3(0, 0, 1);

        float angle = Vector3.Angle(normalDirection, lookPosition.normalized);
        if(lookPosition.x < 0)
        {
            angle = -angle;
        }
        if (lookPosition.z < 0)
        {
            angle = -angle;
        }

        yRotation = angle;
        xRotation = 0;

        camHolder.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, transitionTime);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), transitionTime);
    }
}
