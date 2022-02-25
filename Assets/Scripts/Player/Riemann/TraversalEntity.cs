using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalEntity : MonoBehaviour
{
    public Vector3 prevOffsetFromPortal { get; set; }
    public GameObject player;

    public virtual void Teleport (Transform entrancePortal, Transform exitPortal, Vector3 pos, Quaternion rot)
    {
        // add a slight offset
        Vector3 offset = pos + 1.2f * exitPortal.transform.forward;

        this.player.transform.position = offset;
        Debug.Log("Teleported: " + player.transform.position);
        this.player.transform.rotation = rot;
    }

    //public virtual void EnterPortalThreshold()
    //{

    //}

    //public virtual void ExitPortalThreshold()
    //{

    //}
}
