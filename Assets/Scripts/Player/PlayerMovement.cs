using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // First Person Player Logic
    // TODO: FIX VELOCITY ISSUE
    public CharacterController controller;

    private float speed = 12f;
    private float gravity = -9.81f;

    public Vector3 gravityDir = new Vector3(0.0f, 1.0f, 0.0f);

    public Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Vector3 velocity;
    private bool isGrounded;

    // Ray casting for changing local up
    Ray RayOrigin;
    RaycastHit HitInfo;


    // Update is called once per frame
    void Update()
    {
        // Check for layers in the list of groundMask
        this.isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (this.isGrounded && this.velocity.y < 0)
        {
            Debug.Log("Is grounded");
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Movement is all local to the player's facing direction.
        Vector3 move = transform.right * x + transform.forward * z;

        // make it framerate independent
        this.controller.Move(move * speed * Time.deltaTime);

        this.velocity.y += this.gravityDir.y * this.gravity * Time.deltaTime;
        this.velocity.x += this.gravityDir.x * this.gravity * Time.deltaTime;
        this.velocity.z += this.gravityDir.z * this.gravity * Time.deltaTime;

        // given that deltaY = 1/2 * gravity * t^2 = 1/2 v * t
        controller.Move(velocity * Time.deltaTime);

        // Raycast to check for local up
        if (Input.GetKey(KeyCode.E))
        {
            RayOrigin = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 10.0f))
            {
                Transform cameraTransform = Camera.main.transform;
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 10.0f, Color.yellow);
                this.reground(HitInfo);
            }
        }

    }

    // implementing walking up to a wall and changing your plane of gravity. 
    void reground(RaycastHit HitInfo)
    {
        // Insert rotat player local Up Logic
        if (HitInfo.normal == new Vector3(0.0f, 0.0f, -1.0f))
        {
            this.gameObject.transform.up = new Vector3(0.0f, 0.0f, -1.0f);
            this.gravityDir = new Vector3(0.0f, 0.0f, -1.0f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, 0.0f, 1.0f))
        {
            this.gameObject.transform.up = new Vector3(0.0f, 0.0f, 1.0f);
            this.gravityDir = new Vector3(0.0f, 0.0f, 1.0f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, 1.0f, 0.0f))
        {
            this.gameObject.transform.up = new Vector3(0.0f, 1.0f, 0.0f);
            this.gravityDir = new Vector3(0.0f, 1.0f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, -1.0f, 0.0f))
        {
            this.gameObject.transform.up = new Vector3(0.0f, -1.0f, 0.0f);
            this.gravityDir = new Vector3(0.0f, -1.0f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(1.0f, 0.0f, 0.0f))
        {
            this.gameObject.transform.up = new Vector3(1.0f, 0.0f, 0.0f);
            this.gravityDir = new Vector3(1.0f, 0.0f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(-1.0f, 0.0f, 0.0f))
        {
            this.gameObject.transform.up = new Vector3(-1.0f, 0.0f, 0.0f);
            this.gravityDir = new Vector3(-1.0f, 0.0f, 0.0f);
        }
    }
}
