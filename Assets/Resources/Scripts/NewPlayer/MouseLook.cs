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

    public PlayerMovement playerMove;

    // Start is called before the first frame update
    void Start()
    {
        // hide and lock curser to center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        this.playerMove = GameObject.Find("NewFPSChar").GetComponent<PlayerMovement>();
    }

    // Only called externally
    public void RotateTowards(float val)
    {
        //Debug.Log("Rotate Towards");
        upRotation = val;
    }

    private bool vectorEquals(Vector3 veca, Vector3 vecb)
    {
        return (Vector3.Distance(veca, vecb) < 0.1f);
    }

    // Update is called once per frame
    public void Update()
    {
        // In Unity Project settings Input, it's named Mouse X.
        // Rotation should be independent of framerate (update)
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;

        if (playerMove.escherPhysicsMode)
        {
            upRotation = mouseX;
            rightRotation -= mouseY;

            rightRotation = Mathf.Clamp(rightRotation, -90f, 90f);

            transform.localRotation = this.orientation.rotation * Quaternion.Euler(rightRotation, upRotation, 0);
            this.orientation.rotation *= Quaternion.Euler(0f, upRotation, 0f);
        } else
        {
            upRotation += mouseX;
            rightRotation -= mouseY;

            rightRotation = Mathf.Clamp(rightRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(rightRotation, upRotation, 0);
            this.orientation.rotation = Quaternion.Euler(0, upRotation, 0);
        }
    }
}
