using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovement : MonoBehaviour
{
    private float speed = 12f;
    private float axisSpeed = 0.05f;
    private float rotateSpeed = 1.0f;
    private float strafeSpeed = 1.0f;

    private float gravity = -9.81f;
    private float jumpForce = 0.2f;
    public Vector3 gravityDir = new Vector3(0.0f, 1.0f, 0.0f);

    //public Transform groundCheck;
    //private float groundDistance = 0.4f;
    //public LayerMask groundMask;

    public Vector3 sensorLocal = new Vector3(0.0f, -0.56f, 0.0f);
    public float surfaceSlideSpeed = 1;
    public float slopeClimbSpeed = 1;
    public float slopeDescendSpeed = 1;

    private bool isGrounded;
    private float curGrav;

    public Camera playerCam;
    public LayerMask excludePlayer;

    Ray RayOrigin;
    RaycastHit HitInfo;

    private void OnDrawGizmos()
    {
        Vector3 capsulePos = this.transform.TransformPoint(sensorLocal);
        Gizmos.DrawWireSphere(capsulePos, 0.2f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        Debug.Log(this.gameObject.transform.right);
        Debug.Log(this.gameObject.transform.up);
        Debug.Log(this.gameObject.transform.forward);
    }
}
