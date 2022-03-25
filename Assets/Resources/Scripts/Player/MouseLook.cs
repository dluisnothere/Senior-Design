using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform playerBody;
    float xRotation = 0f;

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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(this.xRotation, 0, 0);
        this.playerBody.Rotate(Vector3.up * mouseX);
    }
}
