using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private int sensitivity;
    [SerializeField] private int lockVertMin, lockVertMax;
    [SerializeField] private bool invertY;

    private float rotX;
    
    // Start is called before the first frame update
    void Start()
    {
        // hide cursor
        Cursor.visible = false;
        
        // lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        cameraMovement();
    }
    
    // handles camera movement
    void cameraMovement()
    {
        // get input 
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // invert Y camera
        if (invertY)
        {
            rotX += mouseY; 
        }
        else
        {
            rotX -= mouseY;
        }

        // clamp the rotX on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate the player on the y-axis
        transform.parent.parent.Rotate(Vector3.up * mouseX);
    }

}
