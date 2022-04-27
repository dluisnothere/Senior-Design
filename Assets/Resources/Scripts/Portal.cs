using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal targetPortalLeft;
    public Portal targetPortalRight;
    public Portal activePortal;
    public Portal inactivePortal;
    private Portal enteredPortal;

    bool isRightPort;

    public MeshRenderer screen;
    Camera playerCam;
    GameObject cameraHolder;
    Transform camOrientation;
    Camera portalCam;
    RenderTexture viewTexture;

    // TODO: Desperately need to clean up the logic in this class with regards to how portals work.

    private void Awake()
    {
        playerCam = Camera.main;
        cameraHolder = GameObject.Find("CameraHolder");
        camOrientation = GameObject.Find("NewFPSChar").transform;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = true;
        
    }

    private void GenerateViewTexture(Portal targetPortal)
    {
        if (this.viewTexture != null)
        {
            targetPortal.screen.material.SetTexture("_MainTex", viewTexture);
            return;
        }

        screen.enabled = false;

        // Render view from the portal camera to view texture
        this.viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

        this.portalCam.targetTexture = viewTexture;

        Matrix4x4 playerCamNoY = playerCam.transform.localToWorldMatrix;
        var y = portalCam.transform.position.y;

        var matrix = this.transform.localToWorldMatrix * targetPortal.transform.worldToLocalMatrix * playerCamNoY;

        portalCam.transform.SetPositionAndRotation(matrix.GetColumn(3), matrix.rotation);
        portalCam.transform.position = new Vector3(portalCam.transform.position.x, y, portalCam.transform.position.z);

        targetPortal.screen.material.SetTexture("_MainTex", viewTexture);

        if (this.portalCam.targetTexture == null)
        {
            Debug.Log(this.gameObject.name + " targetTexture is null for sending to targetPortal: " + targetPortal.gameObject.name);
        } else
        {
            this.portalCam.Render();
        }
        screen.enabled = true;
    }

    public void PreRender(Portal targetPortal)
    {
        this.viewTexture = null;
    }

    public void PostRender(Portal targetPortal)
    {
        if (this.viewTexture != null)
        {
            // manually release texture memory.
            this.viewTexture.Release();
            this.viewTexture = null;
        }
    }

    public void PreRender()
    {
        this.activePortal?.PreRender(this);
    }

    public void PostRender()
    {
        this.activePortal?.PostRender(this);
    }

    public void Render()
    {

        //screen.enabled = false;

        Vector2 xzPlayer = new Vector2(playerCam.transform.position.x, playerCam.transform.position.z);
        Vector2 xzPortal = new Vector2(this.transform.position.x, this.transform.position.z);

        Vector2 playerToPortal = xzPlayer - xzPortal;

        // After rotation, portal's forward is actually its up vector.
        Vector2 portalUp = new Vector2(this.transform.up.x, this.transform.up.z);

        float dotProd = Vector2.Dot(portalUp, playerToPortal);
        //Debug.Log("dotProd: " + dotProd);


        if (dotProd > 0)
        {
            this.activePortal = this.targetPortalRight;
            this.inactivePortal = this.targetPortalLeft;
            this.isRightPort = true;
        }
        else if (dotProd <= 0)
        {
            this.activePortal = this.targetPortalLeft;
            this.inactivePortal = this.targetPortalRight;
            this.isRightPort = false;
        }

        this.activePortal.GenerateViewTexture(this);

        if (this.gameObject.name == "5PortalL")
        {
            //Debug.Log(dotProd);
        }
        // Render camera
        //screen.enabled = true;

        // If viewing from one side, generate one target portal texture. 

        // portal camera position and rotation are the same relative to this portal as the player relative to linked portal. ?? not right logically.
    }


    void EntityEnteredPortal(GameObject entity)
    {
        if (activePortal == null)
        {
            Debug.Log("Houston, we have a problem");
        }
        else if (isRightPort)
        {
            PlayerMovement p = entity.GetComponent<PlayerMovement>();
            p.SitTight();

            MouseLook look = GameObject.Find("PlayerCam").GetComponent<MouseLook>();

            // New stuff.
            float angChange = Vector3.SignedAngle(Vector3.forward, -this.targetPortalRight.transform.up, Vector3.up);
            //Debug.Log("angle change: " + angChange);

            float enterAngle = 180 - Vector3.SignedAngle(this.transform.up, look.orientation.forward, Vector3.up);
            //Debug.Log("enter angle diff: " + enterAngle);
            angChange -= enterAngle;
            //Debug.Log("angle change new: " + angChange);

            //Debug.Log("teleportRight:" + isRightPort);

            if (p)
            {
                look.RotateTowards(angChange);
                look.Update();
                entity.transform.position = activePortal.transform.position + 1.5f * look.orientation.forward;

                var groundMask = LayerMask.GetMask("Ground");
                if (Physics.Raycast(entity.transform.position, Vector3.down, out RaycastHit myHit, 100f, groundMask))
                {
                    entity.transform.position = new Vector3(entity.transform.position.x, myHit.point.y + 0.5f,
                        entity.transform.position.z);
                }

                p.MoveAgain();
            }
        }
        else
        {
            PlayerMovement p = entity.GetComponent<PlayerMovement>();
            p.SitTight();

            MouseLook look = GameObject.Find("PlayerCam").GetComponent<MouseLook>();

            // New stuff.
            float angChange = Vector3.SignedAngle(Vector3.forward, this.targetPortalLeft.transform.up, Vector3.up);
            //Debug.Log("angle change: " + angChange);

            float enterAngle = Vector3.SignedAngle(this.transform.up, look.orientation.forward, Vector3.up);
            //Debug.Log("enter angle diff: " + enterAngle);
            angChange -= enterAngle;
            //Debug.Log("angle change new: " + angChange);

            if (p)
            {
                look.RotateTowards(angChange);
                look.Update();
                entity.transform.position = activePortal.transform.position + 1.5f * look.orientation.forward;

                var groundMask = LayerMask.GetMask("Ground");
                if (Physics.Raycast(entity.transform.position, Vector3.down, out RaycastHit myHit, 100f, groundMask))
                {
                    entity.transform.position = new Vector3(entity.transform.position.x, myHit.point.y + 0.5f,
                        entity.transform.position.z);
                }

                p.MoveAgain();
            }
        }
    }

    private void OnDrawGizmos()
    {
        var pos = this.transform.position;
        Gizmos.DrawLine(pos, pos + this.transform.up * 5);
    }

    // trigger collider
    private void OnTriggerEnter(Collider other)
    {
        GameObject entity = other.gameObject;
        if (entity)
        {
            this.enteredPortal = this.activePortal;
            //Debug.Log(this.gameObject.name + " isRight: " + this.isRightPort);
            EntityEnteredPortal(entity);
        }
    }
}
