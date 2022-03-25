using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform destination;

    void OnMouseDown()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        RiemannPhysics ph = GetComponent<RiemannPhysics>();

        rb.useGravity = false;
        rb.freezeRotation = true;
        //rb.velocity = new Vector3(0, 0, 0);

        //ph.SetPickedUp(true);

        this.transform.position = destination.position;
        this.transform.parent = GameObject.Find("Destination").transform;

    }

    void OnMouseUp()
    {
        this.transform.parent = null;

        Rigidbody rb = GetComponent<Rigidbody>();
        RiemannPhysics ph = GetComponent<RiemannPhysics>();

        rb.useGravity = true;
        rb.freezeRotation = false;

        //ph.SetPickedUp(false);
    }
}
