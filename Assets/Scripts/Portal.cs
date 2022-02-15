using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal targetPortal;
    public MeshRenderer screen;
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;

    List<TraversalEntity> knownEntities;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
        knownEntities = new List<TraversalEntity>();
    }

    private void GenerateViewTexture()
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
            // send view texture to target portal's screen
            targetPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    public void Render()
    {
        // TODO: Do not render if not visible to the player's camera. Courtesy of Sebastian Lague
        

        screen.enabled = false;
        GenerateViewTexture();

        // portal camera position and rotation are the same relative to this portal as the player relative to linked portal. ?? not right logically.
        var matrix = transform.localToWorldMatrix * targetPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(matrix.GetColumn(3), matrix.rotation);

        // Render camera
        portalCam.Render();

        screen.enabled = true;
    }


    void EntityEnteredPortal(TraversalEntity entity)
    {
        if (!knownEntities.Contains(entity))
        {
            entity.EnterPortalThreshold();
            entity.prevOffsetFromPortal = entity.transform.position - this.transform.position;
            knownEntities.Add(entity);
        }
    }

    // trigger collider
    private void OnTriggerEnter(Collider other)
    {
        TraversalEntity entity = other.GetComponent<TraversalEntity>();
        if (entity)
        {
            EntityEnteredPortal(entity);
        }
    }

    // remove entity from knownEntities
    private void OnTriggerExit(Collider other)
    {
        TraversalEntity entity = other.GetComponent<TraversalEntity>();
        if (entity && knownEntities.Contains(entity))
        {
            entity.ExitPortalThreshold();
            knownEntities.Remove(entity);
        }
    }

    private void LateUpdate()
    {
        // For each entity, want to find whether it is on the complete opposite side of the portal.
        // Solve this by using dot product. If the angle is > 90, then yes.
        for (int i = 0; i < knownEntities.Count; i++)
        {
            TraversalEntity entity = knownEntities[i];
            Vector3 vecFromEntity = this.transform.position - entity.gameObject.transform.position;
            Vector3 vecFromEntityPrev = this.transform.position - entity.prevOffsetFromPortal;

            float angle = Vector3.Angle(vecFromEntity, vecFromEntityPrev);
            if (angle > 90)
            {
                var matrix = transform.localToWorldMatrix * targetPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
                entity.Teleport(transform, targetPortal.transform, matrix.GetColumn(3), matrix.rotation);

                // Cannot rely on OnTriggerEnter/Exit to be called next frame because FixedUpdate runs separately.
                // The portal will teleport you multiple times.
                targetPortal.EntityEnteredPortal(entity);

                // Remove immediately after teleporting it.
                knownEntities.RemoveAt(i);
                i--;
            }

            entity.prevOffsetFromPortal = vecFromEntity;
        }
    }
}
