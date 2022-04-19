using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal targetPortalLeft;
    public Portal targetPortalRight;
    public Portal activePortal;
    public Portal inactivePortal;

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
        }
        else if (dotProd <= 0)
        {
            this.activePortal = this.targetPortalLeft;
            this.inactivePortal = this.targetPortalRight;
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
        //Debug.Log(entity.name);

        Vector3 originalForward = entity.transform.forward;

        // See if it's a riemann physics object
        RiemannPhysics ph = entity.GetComponent<RiemannPhysics>();

        if (activePortal == null)
        {
            Debug.Log("Houston, we have a problem");
        }
        else if (GameObject.ReferenceEquals(activePortal, targetPortalRight))
        {
            var m = activePortal.transform.localToWorldMatrix * this.transform.worldToLocalMatrix * entity.transform.localToWorldMatrix;

            entity.transform.position = activePortal.transform.position + 1.5f * (-activePortal.transform.up);

            float currX = this.transform.rotation.eulerAngles.x;
            float activePortalY = activePortal.transform.rotation.eulerAngles.y;
            float currZ = this.transform.rotation.eulerAngles.z;

            //entity.transform.rotation = Quaternion.Euler(currX, -activePortalY, currZ);
            //entity.transform.position = m.GetColumn(3) + Vector4(1.5f * (-activePortal.transform.up), 0.0f);
            entity.transform.rotation = m.rotation;
        }
        else
        {
            var m = activePortal.transform.localToWorldMatrix * this.transform.worldToLocalMatrix * entity.transform.localToWorldMatrix;

            entity.transform.position = activePortal.transform.position + 1.5f * (activePortal.transform.up);
            float currX = this.transform.rotation.eulerAngles.x;
            float activePortalY = activePortal.transform.rotation.eulerAngles.y;
            float currZ = this.transform.rotation.eulerAngles.z;

            //entity.transform.rotation = Quaternion.Euler(currX, activePortalY, currZ);
            //entity.transform.forward = activePortal.transform.up;
            entity.transform.rotation = m.rotation;
        }
    }

    // trigger collider
    private void OnTriggerEnter(Collider other)
    {
        GameObject entity = other.gameObject;
        if (entity)
        {
            EntityEnteredPortal(entity);
        }
    }
}
