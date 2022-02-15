using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalEntity : MonoBehaviour
{
    public Vector3 prevOffsetFromPortal { get; set; }

    public virtual void Teleport (Transform entrancePortal, Transform exitPortal, Vector3 pos, Quaternion rot)
    {
        Debug.Log("Teleported");
        transform.position = pos;
        transform.rotation = rot;
    }

    public virtual void EnterPortalThreshold()
    {

    }

    public virtual void ExitPortalThreshold()
    {

    }
}
