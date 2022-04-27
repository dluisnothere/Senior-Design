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
    }

    private void Update()
    {
        // Perform ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        MyInput();

        Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward * 10.0f, Color.red);
        Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.up * 10.0f, Color.blue);
        Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.right * 10.0f, Color.green);

        //Debug.Log("CURR UP: " + this.transform.up);
        //Debug.Log("CURR RIGHt: " + this.transform.right);

        //Debug.DrawRay(this.transform.position, this.orientation.forward * 10.0f, Color.red);
        //Debug.DrawRay(this.transform.position, this.orientation.up * 10.0f, Color.blue);
        //Debug.DrawRay(this.transform.position, this.orientation.right * 10.0f, Color.green);

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
        // calculate moveemetn direction
        //Debug.Log("this forward: " + this.orientation.forward);
        this.moveDirection = this.orientation.forward * this.verticalInput + this.orientation.right * this.horizontalInput;
        //Debug.Log("ORIENTATION FORWARD");
        //Debug.Log(orientation.forward);
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

    //implementing walking up to a wall and changing your plane of gravity.
    private void Reground(RaycastHit HitInfo)
    {
        rb.freezeRotation = false;
        Debug.Log("HIT INFO");
        Debug.Log(HitInfo.normal);
        // Insert rotat player local Up Logic

        if (HitInfo.normal == new Vector3(0.0f, 0.0f, -1.0f))
        {
            this.transform.RotateAround(this.transform.position, this.transform.right, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, this.cameraHolder.transform.right, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, 0.0f, 9.81f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, 0.0f, 1.0f))
        {
            this.transform.RotateAround(this.transform.position, this.transform.right, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, this.cameraHolder.transform.right, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, 0.0f, -9.81f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, 1.0f, 0.0f))
        {
            this.transform.RotateAround(this.transform.position, this.transform.right, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, this.cameraHolder.transform.right, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, -9.81f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(0.0f, -1.0f, 0.0f))
        {
            this.transform.RotateAround(this.transform.position, this.transform.right, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, this.cameraHolder.transform.right, -180);

            Debug.Log("UP: " + this.gameObject.transform.up);
            Debug.Log("FORWARD: " + this.gameObject.transform.forward);
            Debug.Log("RIGHT: " + this.gameObject.transform.right);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(0.0f, 9.81f, 0.0f);
        }
        else if (HitInfo.normal == new Vector3(1.0f, 0.0f, 0.0f))
        {

            //Debug.Log("BEFORE UP: " + this.gameObject.transform.up);
            //Debug.Log("BEFORE FORWARD: " + this.gameObject.transform.forward);
            //Debug.Log("BEFORE RIGHT: " + this.gameObject.transform.right);

            this.transform.RotateAround(this.transform.position, this.transform.right, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, this.cameraHolder.transform.right, -90);

            //Debug.Log("UP: " + this.gameObject.transform.up);
            //Debug.Log("FORWARD: " + this.gameObject.transform.forward);
            //Debug.Log("RIGHT: " + this.gameObject.transform.right);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(-9.81f, 0.0f, 0.0f);

        }
        else if (HitInfo.normal == new Vector3(-1.0f, 0.0f, 0.0f))
        {
            this.transform.RotateAround(this.transform.position, this.transform.right, -90);
            this.cameraHolder.transform.RotateAround(this.cameraHolder.transform.position, this.cameraHolder.transform.right, -90);

            this.mouseLook.cameraUp = this.orientation.up;
            this.mouseLook.cameraForward = this.orientation.forward;

            Physics.gravity = new Vector3(9.81f, 0.0f, 0.0f);
        }

        rb.freezeRotation = true;
    }
}

//public class PlayerMovement : MonoBehaviour
//{
//    // BASIC First Person Player Logic
//    // TODO: FIX VELOCITY ISSUE
//    //public CharacterController controller;
//    public Rigidbody rb; 

//    private float speed = 12f;
//    private float gravity = -9.81f;
//    private float jumpForce = 1.0f;

//    public Vector3 gravityDir = new Vector3(0.0f, 1.0f, 0.0f);

//    public Transform groundCheck;
//    private float groundDistance = 0.4f;
//    public LayerMask groundMask;

//    public Vector3 velocity;
//    private bool isGrounded;

//    public Camera playerCam;
//    public LayerMask excludePlayer;

//    // PHYSICS MODE
//    public bool riemannPhysicsMode;
//    public bool escherPhysicsMode;

//    // ESCHER PLAYER LOGIC
//    // Ray casting for changing local up
//    Ray RayOrigin;
//    RaycastHit HitInfo;

//    private void Start()
//    {
//        this.GetComponent<RiemannPhysics>().enabled = true;
//        this.rb = this.GetComponent<Rigidbody>();

//        // TODO: PUT THIS IN A DIFFERENT CLASS AND FIX
//        this.escherPhysicsMode = false;
//    }


//    // Update is called once per frame
//    void Update()
//    {
//        // Check for layers in the list of groundMask
//        this.isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

//        if (this.isGrounded && this.velocity.y < 0)
//        {
//            Debug.Log("Is grounded");
//            //velocity.y = -2f;
//        }

//        float x = Input.GetAxis("Horizontal");
//        float z = Input.GetAxis("Vertical");

//        // Movement is all local to the player's facing direction.
//        Vector3 move = transform.right * x + transform.forward * z;
//        Debug.Log(move);

//        // make it framerate independent
//        // this.controller.Move(move * speed * Time.deltaTime);
//        this.rb.MovePosition(move * speed * Time.deltaTime);

//        // this.velocity

//        this.velocity.y += this.gravityDir.y * this.gravity * Time.deltaTime;
//        this.velocity.x += this.gravityDir.x * this.gravity * Time.deltaTime;
//        this.velocity.z += this.gravityDir.z * this.gravity * Time.deltaTime;

//        // given that deltaY = 1/2 * gravity * t^2 = 1/2 v * t
//        // controller.Move(velocity * Time.deltaTime);
//        this.rb.MovePosition(this.velocity * Time.deltaTime);

//        // Raycast to check for local up
//        if (Input.GetKey(KeyCode.E))
//        {
//            RayOrigin = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
//            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 10.0f))
//            {
//                Transform cameraTransform = Camera.main.transform;
//                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 10.0f, Color.yellow);
//                this.reground(HitInfo);
//            }
//        }

//    }

//    // implementing walking up to a wall and changing your plane of gravity. 
//    void reground(RaycastHit HitInfo)
//    {
//        // Insert rotat player local Up Logic
//        if (HitInfo.normal == new Vector3(0.0f, 0.0f, -1.0f))
//        {
//            this.gameObject.transform.up = new Vector3(0.0f, 0.0f, -1.0f);
//            this.gravityDir = new Vector3(0.0f, 0.0f, -1.0f);
//        }
//        else if (HitInfo.normal == new Vector3(0.0f, 0.0f, 1.0f))
//        {
//            this.gameObject.transform.up = new Vector3(0.0f, 0.0f, 1.0f);
//            this.gravityDir = new Vector3(0.0f, 0.0f, 1.0f);
//        }
//        else if (HitInfo.normal == new Vector3(0.0f, 1.0f, 0.0f))
//        {
//            this.gameObject.transform.up = new Vector3(0.0f, 1.0f, 0.0f);
//            this.gravityDir = new Vector3(0.0f, 1.0f, 0.0f);
//        }
//        else if (HitInfo.normal == new Vector3(0.0f, -1.0f, 0.0f))
//        {
//            this.gameObject.transform.up = new Vector3(0.0f, -1.0f, 0.0f);
//            this.gravityDir = new Vector3(0.0f, -1.0f, 0.0f);
//        }
//        else if (HitInfo.normal == new Vector3(1.0f, 0.0f, 0.0f))
//        {
//            this.gameObject.transform.up = new Vector3(1.0f, 0.0f, 0.0f);
//            this.gravityDir = new Vector3(1.0f, 0.0f, 0.0f);
//        }
//        else if (HitInfo.normal == new Vector3(-1.0f, 0.0f, 0.0f))
//        {
//            this.gameObject.transform.up = new Vector3(-1.0f, 0.0f, 0.0f);
//            this.gravityDir = new Vector3(-1.0f, 0.0f, 0.0f);
//        }
//        Debug.Log(this.gameObject.transform.right);
//        Debug.Log(this.gameObject.transform.up);
//        Debug.Log(this.gameObject.transform.forward);
//    }
//}
