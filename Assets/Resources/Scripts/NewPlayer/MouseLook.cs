using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform orientation;
    float rightRotation = 0f;
    float upRotation = 0f;

    Vector3 px = new Vector3(1.0f, 0.0f, 0.0f);
    Vector3 py = new Vector3(0.0f, 1.0f, 0.0f);
    Vector3 pz = new Vector3(0.0f, 0.0f, 1.0f);

    Vector3 nx = new Vector3(-1.0f, 0.0f, 0.0f);
    Vector3 ny = new Vector3(0.0f, -1.0f, 0.0f);
    Vector3 nz = new Vector3(0.0f, 0.0f, -1.0f);

    public Vector3 cameraUp;
    public Vector3 cameraForward;

    // Start is called before the first frame update
    void Start()
    {
        // hide and lock curser to center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // In Unity Project settings Input, it's named Mouse X.
        // Rotation should be independent of framerate (update)
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;

        upRotation += mouseX;
        rightRotation -= mouseY;

        //yRotation = mouseX;
        //xRotation = mouseY;

        rightRotation = Mathf.Clamp(rightRotation, -90f, 90f);

        if (cameraUp == px || cameraUp == nx)
        {
            transform.localRotation = Quaternion.Euler(upRotation, rightRotation, 0);
            this.orientation.rotation = Quaternion.Euler(upRotation, 0, 0);
        }
        else if (cameraUp == py || cameraUp == ny)
        {

            transform.localRotation = Quaternion.Euler(rightRotation, upRotation, 0);
            this.orientation.rotation = Quaternion.Euler(0, upRotation, 0);
        }
        else if (cameraUp == pz || cameraUp == nz)
        {
            transform.localRotation = Quaternion.Euler(rightRotation, 0, upRotation);
            this.orientation.rotation = Quaternion.Euler(0, 0, upRotation);
        }
    }
}
