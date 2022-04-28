using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private Vector3 trueRight;

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool isJumpReady;

    public Vector3 gravityDir = new Vector3(0.0f, 1.0f, 0.0f);

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    bool isGrounded;

    public Transform orientation;
    public GameObject cameraHolder;

    MouseLook mouseLook;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Physics Mode")]
    public bool riemannPhysicsMode;
    public bool escherPhysicsMode;

    // Escher Physics Mode
    Ray RayOrigin;
    RaycastHit HitInfo;

    private void Start()
    {
        //Debug.Log("FORWARD: " + this.transform.forward);
        //Debug.Log("UP: " + this.transform.up);
        //Debug.Log("RIGHT: " + this.transform.right);

        rb = this.GetComponent<Rigidbody>();
        cameraHolder = GameObject.Find("PlayerCam");
        mouseLook = cameraHolder.GetComponent<MouseLook>();
        mouseLook.cameraUp = this.orientation.up;
        mouseLook.cameraForward = this.orientation.forward;


        rb.freezeRotation = true;
        this.isJumpReady = true;

        this.escherPhysicsMode = true;
    }

    private void Update()
    {
        // Perform ground check
        isGrounded = Physics.Raycast(transform.position, -this.transform.up, playerHeight * 0.5f + 0.2f, groundMask);
        MyInput();

        Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward * 10.0f, Color.red);
        Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.up * 10.0f, Color.blue);
        Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.right * 10.0f, Color.green);

        // apply drag
        if (isGrounded)
        {
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        this.horizontalInput = Input.GetAxis("Horizontal");
        this.verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && isJumpReady && isGrounded)
        {
            isJumpReady = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Raycast to check for local up
        if (Input.GetKeyDown(KeyCode.E))
        {
            RayOrigin = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 10.0f))
            {
                Transform cameraTransform = Camera.main.transform;
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 10.0f, Color.yellow);
                this.Reground(HitInfo);
            }
        }
    }

    private bool dontMove;

    public void SitTight()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        dontMove = true;
    }

    public void MoveAgain()
    {
        dontMove = false;
    }

    private void MovePlayer()
    {
        if (dontMove)
        {
            return;
        }

        this.moveDirection = this.orientation.forward * this.verticalInput + this.orientation.right * this.horizontalInput;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        isJumpReady = true;
    }

    // When the player leaves the Escher room, their gravity and rotation should be reset.
    public void ResetRotation()
    {
        this.escherPhysicsMode = false;
        this.transform.rotation = Quaternion.identity;
        Physics.gravity = new Vector3(0.0f, -9.81f, 0.0f);
    }

    //implementing walking up to a wall and changing your plane of gravity.
    private void Reground(RaycastHit HitInfo)
    {
        rb.freezeRotation = false;
        Debug.Log("HIT INFO");
        Debug.Log(HitInfo.normal);
        // Insert rotat player local Up Logic

        Vector3 realRight = -1 * Vector3.Cross(this.transform.up, HitInfo.normal);


        if (HitInfo.normal == new Vector3(0.0f, 0.0f, -1.0f))
        {
            this.transform.RotateAround(this.transform.position, realRight, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, realRight, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, 0.0f, 9.81f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, 0.0f, 1.0f))
        {

            this.transform.RotateAround(this.transform.position, realRight, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, realRight, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, 0.0f, -9.81f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, 1.0f, 0.0f))
        {

            this.transform.RotateAround(this.transform.position, realRight, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, realRight, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, -9.81f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, -1.0f, 0.0f))
        {

            this.transform.RotateAround(this.transform.position, realRight, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, realRight, -180);

            Debug.Log("UP: " + this.gameObject.transform.up);
            Debug.Log("FORWARD: " + this.gameObject.transform.forward);
            Debug.Log("RIGHT: " + this.gameObject.transform.right);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, 9.81f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(1.0f, 0.0f, 0.0f))
        {
            this.transform.RotateAround(this.transform.position, realRight, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, realRight, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(-9.81f, 0.0f, 0.0f);

        }
        else if (HitInfo.normal == new Vector3(-1.0f, 0.0f, 0.0f))
        {

            this.transform.RotateAround(this.transform.position, realRight, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, realRight, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(9.81f, 0.0f, 0.0f);
        }

        rb.freezeRotation = true;
    }
}
