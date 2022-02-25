using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal targetPortalLeft;
    public Portal targetPortalRight;
    public Portal activePortal;

    public MeshRenderer screen;
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;

    public bool isLeftPortal;

    //List<TraversalEntity> knownEntities;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
        //knownEntities = new List<TraversalEntity>();
    }

    private void GenerateViewTexture(Portal targetPortal)
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
            {
                viewTexture.Release();
            }

            // Render view from the portal camera to view texture
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            portalCam.targetTexture = viewTexture;
            // send view texture to starting portal's screen
            targetPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    public void Render()
    {
        screen.enabled = false;

        Vector2 xzPlayer = new Vector2(playerCam.transform.position.x, playerCam.transform.position.z);
        Vector2 xzPortal = new Vector2(this.transform.position.x, this.transform.position.z);

        Vector2 playerToPortal = xzPlayer - xzPortal;

        // After rotation, portal's forward is actually its up vector.
        Vector2 portalUp = new Vector2(this.transform.up.x, this.transform.up.z);

        float dotProd = Vector2.Dot(portalUp, playerToPortal);
        Debug.Log("dotProd: " + dotProd);

        if (dotProd > 0)
        {
            GenerateViewTexture(this.targetPortalRight);
            Matrix4x4 playerCamNoY = playerCam.transform.localToWorldMatrix;

            var y = portalCam.transform.position.y;

            var matrix = transform.localToWorldMatrix * targetPortalRight.transform.worldToLocalMatrix * playerCamNoY;

            // how to apply transform instead of setting the transform. How to keep y constant?
            portalCam.transform.SetPositionAndRotation(matrix.GetColumn(3), matrix.rotation);
            portalCam.transform.position = new Vector3(portalCam.transform.position.x, y, portalCam.transform.position.z);

            // Render camera
            portalCam.Render();

            screen.enabled = true;
            this.activePortal = this.targetPortalRight;

        } else if (dotProd < 0)
        {
            GenerateViewTexture(this.targetPortalLeft);

            Matrix4x4 playerCamNoY = playerCam.transform.localToWorldMatrix;

            var y = portalCam.transform.position.y;

            var matrix = transform.localToWorldMatrix * targetPortalLeft.transform.worldToLocalMatrix * playerCamNoY;

            // how to apply transform instead of setting the transform. How to keep y constant?
            portalCam.transform.SetPositionAndRotation(matrix.GetColumn(3), matrix.rotation);
            portalCam.transform.position = new Vector3(portalCam.transform.position.x, y, portalCam.transform.position.z);

            // Render camera
            portalCam.Render();

            screen.enabled = true;

            this.activePortal = this.targetPortalLeft;
        }

        //Debug.Log("Self Portal: " + this.name + " Active Portal: " + activePortal.name);

        // If viewing from one side, generate one target portal texture. 

        // portal camera position and rotation are the same relative to this portal as the player relative to linked portal. ?? not right logically.
    }


    void EntityEnteredPortal(TraversalEntity entity)
    {
        
        //entity.EnterPortalThreshold();
        entity.prevOffsetFromPortal = entity.transform.position - this.transform.position;
        //knownEntities.Add(entity);
        if (activePortal == null)
        {
            Debug.Log("Houston, we have a problem");
        }
        else if (GameObject.ReferenceEquals(activePortal, targetPortalLeft))
        {
            Debug.Log("Enter Room for: " + activePortal.name);
            entity.transform.position = activePortal.transform.position + (-1.2f * activePortal.transform.up);
            entity.transform.rotation = Quaternion.identity;
            entity.transform.forward = -activePortal.transform.up;
        }
        else
        {
            Debug.Log("Enter Room for: " + activePortal.name);
            entity.transform.position = activePortal.transform.position + (activePortal.transform.up);
            entity.transform.rotation = Quaternion.identity;
            entity.transform.forward = activePortal.transform.up;
        }
    }

    // trigger collider
    private void OnTriggerEnter(Collider other)
    {
        
    }

    // remove entity from knownEntities
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Left portal " + this.gameObject.name);
        //TraversalEntity entity = other.GetComponent<TraversalEntity>();
        //if (entity && knownEntities.Contains(entity))
        //{
        //    //entity.ExitPortalThreshold();
        //    knownEntities.Remove(entity);
        //}
        TraversalEntity entity = other.GetComponent<TraversalEntity>();
        if (entity)
        {
            EntityEnteredPortal(entity);
        }
    }

    /*static bool PortalIsVisible(Renderer renderer, Camera cam)
    {
        Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(frustum, renderer.bounds);

    }
    private void LateUpdate()
    {
        // For each entity, want to find whether it is on the complete opposite side of the portal.
        // Solve this by using dot product. If the angle is > 90, then yes.
        for (int i = 0; i < knownEntities.Count; i++)
        {
            TraversalEntity entity = knownEntities[i];
            Vector3 vecFromEntity = this.transform.position - entity.transform.position;
            Vector3 vecFromEntityPrev = this.transform.position - entity.prevOffsetFromPortal;

            int currPortalSide = System.Math.Sign(Vector3.Dot(vecFromEntity, transform.forward));
            int prevPortalSide = System.Math.Sign(Vector3.Dot(vecFromEntityPrev, transform.forward));

            //Debug.Log("curr Portal side: " + currPortalSide);
            //Debug.Log("prev Portal side: " + prevPortalSide);

            //CharacterController control = entity.gameObject.GetComponent<CharacterController>();
            //Debug.Log("velocity: " + control.velocity);
         
            if ((control.velocity.z > 0 && this.isLeftPortal) || (control.velocity.z < 0 && !this.isLeftPortal))
            {
                Debug.Log("want to teleport");
                var matrix = targetPortal.transform.localToWorldMatrix * this.transform.worldToLocalMatrix * entity.transform.localToWorldMatrix;
                entity.Teleport(transform, targetPortal.transform, matrix.GetColumn(3), matrix.rotation);
                //Debug.Log("From Portal" + this.gameObject.name);
                //Debug.Log("new position" + entity.transform.position);

                // Cannot rely on OnTriggerEnter/Exit to be called next frame because FixedUpdate runs separately.
                // The portal will teleport you multiple times.
                targetPortal.EntityEnteredPortal(entity);

                // Remove immediately after teleporting it.
                knownEntities.RemoveAt(i);
                i--;
            } 
            else
            {
                entity.prevOffsetFromPortal = entity.transform.position - this.transform.position;
                //Debug.Log("enemy prev offset SEt");
                //Debug.Log(entity.prevOffsetFromPortal);
            }
        }
    }*/
}
